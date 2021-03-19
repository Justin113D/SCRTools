using SCRCommon.Viewmodels;
using SCRCommon.WpfStyles;
using SCRDialogEditor.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SCRDialogEditor.Viewmodel
{
    public class VmGrid : BaseViewModel
    {
        #region constants

        /// <summary>
        /// Grid background cell width
        /// </summary>
        public const int brushDim = 50;

        /// <summary>
        /// Half the width of a background cell
        /// </summary>
        public const int halfBrushDim = brushDim / 2;

        #endregion

        #region Commands

        /// <summary>
        /// Deletes the active node from the grid
        /// </summary>
        public RelayCommand Cmd_DeleteNode
            => new(DeleteActiveNode);

        public RelayCommand Cmd_RecenterContents
            => new(RecenterContents);

        public RelayCommand Cmd_RecenterView
            => new(RecenterView);

        #endregion

        #region Private Fields

        private VmNode _grabbed;

        private VmNodeOutput _connecting;

        #endregion

        public VmMain Main { get; }

        /// <summary>
        /// Dialog data
        /// </summary>
        public Dialog Data { get; }

        /// <summary>
        /// Nodes to display
        /// </summary>
        public ObservableCollection<VmNode> Nodes { get; }

        /// <summary>
        /// All note outputs
        /// </summary>
        public ObservableCollection<VmNodeOutput> Outputs { get; }


        /// <summary>
        /// Background grid brush
        /// </summary>
        public VisualBrush Background { get; private set; }

        /// <summary>
        /// Grid translation
        /// </summary>
        public TranslateTransform Position { get; private set; }

        public Color BackgroundColor
        {
            get => ((SolidColorBrush)((Path)Background.Visual).Fill).Color;
            set => ((SolidColorBrush)((Path)Background.Visual).Fill).Color = value;
        }

        /// <summary>
        /// Grabbed Node
        /// </summary>
        public VmNode Grabbed
        {
            get => _grabbed;
            set
            {
                if(_grabbed == value)
                    return;

                _grabbed?.EndGrab();
                _grabbed = value;
            }
        }

        /// <summary>
        /// Selected Node to edit
        /// </summary>
        public VmNode Active { get; set; }

        /// <summary>
        /// The nodeoutput that is currently being "dragged"
        /// </summary>
        public VmNodeOutput Connecting
        {
            get => _connecting;
            set
            {
                if(value == _connecting)
                    return;

                if(value != null)
                    value.Displaying = true;

                if(_connecting != null && _connecting.VmOutput == null)
                    _connecting.Displaying = false;

                _connecting = value;
            }
        }


        public VmGrid(VmMain mainVM, Dialog dialog)
        {
            Main = mainVM;
            Data = dialog;

            Outputs = new();
            Position = new();
            Nodes = new();

            Dictionary<Node, VmNode> viewmodelPairs = new();
            foreach(Node node in dialog.Nodes)
            {
                VmNode vmnode = new(this, node);
                Nodes.Add(vmnode);
                viewmodelPairs.Add(node, vmnode);
            }

            foreach(VmNode vmnode in Nodes)
                foreach(VmNodeOutput vmout in vmnode.Outputs)
                    if(vmout.Data.Output != null)
                    {
                        vmout.VmOutput = viewmodelPairs[vmout.Data.Output];
                    }

            Background = new()
            {
                TileMode = TileMode.Tile,
                ViewportUnits = BrushMappingMode.Absolute,
                ViewboxUnits = BrushMappingMode.Absolute,
                Viewbox = new(0, 0, brushDim, brushDim),
                Visual = new Path
                {
                    Fill = new SolidColorBrush((Color)BaseWindowStyle.currentTheme["BGCol2"]),
                    Data = new RectangleGeometry(new(1, 1, brushDim - 2, brushDim - 2))
                }
            };

            UpdateBackground();
        }

        /// <summary>
        /// Updates the background brush
        /// </summary>
        public void UpdateBackground()
        {
            Background.Viewport = new Rect(Position.X, Position.Y, brushDim, brushDim);
        }

        /// <summary>
        /// Lets go of the grabbed connection/node
        /// </summary>
        public void LetGo()
        {
            Grabbed = null;
            Connecting = null;
        }

        /// <summary>
        /// Moves the grid by a difference
        /// </summary>
        /// <param name="dif"></param>
        public void MoveGrid(Point dif)
        {
            Position.X += dif.X;
            Position.Y += dif.Y;
            OnPropertyChanged(nameof(Position));
            UpdateBackground();
        }

        /// <summary>
        /// Moves whatever is grabbed <br/>
        /// nodes require difference, while connections the absolute position
        /// </summary>
        /// <param name="dif"></param>
        /// <param name="absolute"></param>
        public void MoveGrabbed(Point dif, Point absolute)
        {
            Grabbed?.Move(dif);

            Connecting?.UpdateEndPosition(
                new Point(absolute.X - Position.X, absolute.Y - Position.Y)
            );

        }

        /// <summary>
        /// Adds a new node at Mouse position
        /// </summary>
        public void CreateNode(Point position)
        {
            Node node = Data.CreateNode();

            VmNode vmnode = new(this, node, position);

            Nodes.Add(vmnode);
        }

        /// <summary>
        /// Deletes a node from the grid
        /// </summary>
        /// <param name="node"></param>
        public void DeleteNode(VmNode node)
        {
            node.Disconnect();
            Nodes.Remove(node);
            Data.RemoveNode(node.Data);
        }

        /// <summary>
        /// Deletes the active noe
        /// </summary>
        private void DeleteActiveNode()
        {
            if(Active == null)
                return;

            DeleteNode(Active);
            Active = null;
        }

        /// <summary>
        /// Recenters the contents by the start of the node structure
        /// </summary>
        private void RecenterContents()
        {
            if(Outputs.Count == 0)
                return;

            // find the starter node
            VmNode start = Nodes.First(x => x.Data == Data.StartNode);
            Point offset = new(-start.Position.X, -start.Position.Y);

            foreach(var n in Nodes)
            {
                n.Move(offset);
                n.UpdateDataPosition();
            }
        }

        /// <summary>
        /// Recenters the view
        /// </summary>
        private void RecenterView()
        {
            Position.X = 0;
            Position.Y = 0;
            UpdateBackground();
        }
    }
}

