using SCRCommon.Viewmodels;
using SCRCommon.WpfStyles;
using SCRDialogEditor.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        /// Command for MouseMove event
        /// </summary>
        public RelayCommand<MouseEventArgs> Cmd_MouseMove 
            => new(MouseMove);

        /// <summary>
        /// Command for MouseLeave event
        /// </summary>
        public RelayCommand<MouseEventArgs> Cmd_MouseLeave 
            => new(MouseLeave);

        /// <summary>
        /// Command for MouseDown event
        /// </summary>
        public RelayCommand<MouseButtonEventArgs> Cmd_MouseDown 
            => new(MouseDown);

        /// <summary>
        /// Command for MouseUp event
        /// </summary>
        public RelayCommand<MouseButtonEventArgs> Cmd_MouseUp 
            => new(MouseUp);

        /// <summary>
        /// Gets initiated once the grid container is loaded
        /// </summary>
        public RelayCommand<Grid> Cmd_ContainerLoaded 
            => new(ContainerLoaded);

        /// <summary>
        /// Adds a node to the grid
        /// </summary>
        public RelayCommand<KeyEventArgs> Cmd_AddNode 
            => new(AddNode);

        public RelayCommand Cmd_DeleteNode 
            => new(DeleteNode);

        public RelayCommand Cmd_Focus
            => new(() => _container?.Focus());

        #endregion

        public VmMain VmMain { get; }

        /// <summary>
        /// Dialog data
        /// </summary>
        public Dialog Data { get; private set; }

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
        /// Border object to capture mouse position
        /// </summary>
        private Grid _container;

        /// <summary>
        /// Last mouse position (null = mouse no longer inside window
        /// </summary>
        private Point? _mousePos;

        /// <summary>
        /// Grabbed Node
        /// </summary>
        public VmNode Grabbed { get; set; }

        /// <summary>
        /// Selected Node to edit
        /// </summary>
        public VmNode Active { get; set; }


        private VmNodeOutput _connecting;

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

        public Point DragPosition { get; set; }



        public VmGrid(VmMain mainVM)
        {
            VmMain = mainVM;
            Data = new Dialog();

            Outputs = new();
            Position = new();
            Nodes = new();

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

        public void SetDialog(Dialog dialog)
        {
            foreach(VmNode vmnode in Nodes)
            {
                foreach(VmNodeOutput vmout in vmnode.Outputs)
                {
                    vmout.VmOutput = null;
                    vmout.Displaying = false;
                }
            }
            Nodes.Clear();
            Active = null;

            // create new data
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
                        vmout.Displaying = true;
                    }

            Data = dialog;
        }

        /// <summary>
        /// Updates the background brush
        /// </summary>
        public void UpdateBackground()
        {
            Background.Viewport = new Rect(Position.X, Position.Y, brushDim, brushDim);
        }

        /// <summary>
        /// Gets position of a node socket by using its border
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public Point GetPosition(FrameworkElement element)
        {
            try
            {
                Point p = element?.TransformToAncestor(_container).Transform(default) ?? default;
                p.X -= Position.X;
                p.Y -= Position.Y;
                return p;
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// Adds a new node at Mouse position
        /// </summary>
        public void AddNode(KeyEventArgs t)
        {
            t.Handled = true;
            if(!_mousePos.HasValue || t.IsRepeat)
                return;

            Node node = new Node(Data);
            Data.Nodes.Add(node);

            int posX = (int)(_mousePos?.X - Position.X);
            int posY = (int)(_mousePos?.Y - Position.Y);

            VmNode vmnode = new VmNode(this, node)
            {
                PositionX = posX + halfBrushDim * (posX < 0 ? -1 : 1),
                PositionY = posY + halfBrushDim * (posY < 0 ? -1 : 1)
            };
            Nodes.Add(vmnode);
        }

        public void DeleteNode()
        {
            Active?.Delete();
            Active = null;
        }

        #region Eventmethods

        /// <summary>
        /// Sets the container once its loaded
        /// </summary>
        /// <param name="container"></param>
        private void ContainerLoaded(Grid container) => _container = container;

        /// <summary>
        /// MouseLeave event method <br/>
        /// Force-ends grabbing
        /// </summary>
        /// <param name="args"></param>
        private void MouseLeave(MouseEventArgs args)
        {
            _mousePos = null;
            if(args.MiddleButton == MouseButtonState.Pressed)
                Mouse.OverrideCursor = null;
            Grabbed?.EndGrab();
        }

        /// <summary>
        /// MouseDown event method <br/>
        /// Initiates Grabbing or Moving
        /// </summary>
        /// <param name="args"></param>
        public void MouseDown(MouseButtonEventArgs args)
        {
            if(args.MiddleButton == MouseButtonState.Pressed)
                Mouse.OverrideCursor = Cursors.SizeAll;
            if(args.LeftButton != MouseButtonState.Pressed)
                Grabbed?.EndGrab();
        }

        /// <summary>
        /// MouseUp event method <br/>
        /// Ends grabbing 
        /// </summary>
        /// <param name="args"></param>
        public void MouseUp(MouseButtonEventArgs args)
        {
            if(args.MiddleButton != MouseButtonState.Pressed)
                Mouse.OverrideCursor = null;
            if(args.LeftButton != MouseButtonState.Pressed)
            {
                Grabbed?.EndGrab();
                Connecting = null;
            }
        }

        /// <summary>
        /// MouseMove event method <br/>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public void MouseMove(MouseEventArgs args)
        {
            Point t = args.GetPosition(_container);

            int difX = (int)(t.X - (_mousePos?.X) ?? 0);
            int difY = (int)(t.Y - (_mousePos?.Y) ?? 0);

            if(args.MiddleButton == MouseButtonState.Pressed)
            {
                if(!_mousePos.HasValue)
                    Mouse.OverrideCursor = Cursors.SizeAll;
                else
                {
                    Position.X += difX;
                    Position.Y += difY;
                    OnPropertyChanged(nameof(Position));

                    UpdateBackground();
                }
            }
            else if(args.LeftButton == MouseButtonState.Pressed)
            {
                if(!_mousePos.HasValue)
                    Mouse.OverrideCursor = Cursors.SizeAll;
                else if (Grabbed != null)
                {
                    int newX = (int)DragPosition.X + difX;
                    int newY = (int)DragPosition.Y + difY;
                    DragPosition = new Point(newX, newY);
                    Grabbed?.RefreshPosition();
                }
                else if (Connecting != null)
                {
                    DragPosition = new Point((_mousePos?.X ?? 0) - Position.X, (_mousePos?.Y ?? 0) - Position.Y);
                    Connecting?.RefreshDisplay();
                }

            }

            _mousePos = t;
        }

        #endregion
    }
}

