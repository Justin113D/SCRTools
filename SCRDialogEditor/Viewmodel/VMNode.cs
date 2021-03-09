using SCRCommon.Viewmodels;
using SCRDialogEditor.Data;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using System.Collections.Generic;

namespace SCRDialogEditor.Viewmodel
{
    public class VmNode : BaseViewModel
    {
        #region Commands
        
        /// <summary>
        /// Add Output Command <br/>
        /// Adds a new node output
        /// </summary>
        public RelayCommand Cmd_AddOutput 
            => new(AddOutput);

        /// <summary>
        /// MouseUp event command
        /// </summary>
        public RelayCommand Cmd_MouseUp 
            => new(MouseUp);

        /// <summary>
        /// Mouse down event
        /// </summary>
        public RelayCommand<MouseButtonEventArgs> Cmd_MouseDown 
            => new(MouseDown);

        /// <summary>
        /// Layout Updated event command
        /// </summary>
        public RelayCommand<Canvas> Cmd_LayoutLoaded 
            => new(LayoutLoaded);

        #endregion

        /// <summary>
        /// Grid parent
        /// </summary>
        public VmGrid VmGrid { get; }

        /// <summary>
        /// Data object
        /// </summary>
        public Node Data { get; }

        #region Properties

        /// <summary>
        /// Element that represents the node
        /// </summary>
        private FrameworkElement _element;

        /// <summary>
        /// Last element position
        /// </summary>
        private Point _elementPosition;

        /// <summary>
        /// Viewmodel objects for the outputs
        /// </summary>
        public ObservableCollection<VmNodeOutput> Outputs { get; }

        /// <summary>
        /// Attached inputs (only for updating the lines)
        /// </summary>
        private readonly List<VmNodeOutput> _inputs;

        /// <summary>
        /// Whether this Node is selected
        /// </summary>
        public bool IsActive 
            => VmGrid.Active == this;

        /// <summary>
        /// X Position
        /// </summary>
        public int PositionX
        {
            get
            {
                if(VmGrid.Grabbed == this)
                    return (int)VmGrid.DragPosition.X;
                else
                    return Data.LocationX * VmGrid.brushDim + VmGrid.halfBrushDim;
            }
            set => Data.LocationX = (value - VmGrid.halfBrushDim) / VmGrid.brushDim;
        }

        /// <summary>
        /// Y Position
        /// </summary>
        public int PositionY
        {
            get
            {
                if(VmGrid.Grabbed == this)
                    return (int)VmGrid.DragPosition.Y;
                else
                    return Data.LocationY * VmGrid.brushDim + VmGrid.halfBrushDim;
            }
            set => Data.LocationY = (value - VmGrid.halfBrushDim) / VmGrid.brushDim;
        }

        /// <summary>
        /// Header Character Color
        /// </summary>
        public SolidColorBrush CharacterColor 
            => new(Outputs[0].Character?.Color ?? Colors.Gray);

        /// <summary>
        /// Header Expression Color
        /// </summary>
        public SolidColorBrush ExpressionColor 
            => new(Outputs[0].Expression?.Color ?? Colors.Gray);

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="node"></param>
        public VmNode(VmGrid grid, Node node)
        {
            VmGrid = grid;
            Data = node;
            _inputs = new();
            Outputs = new();
            foreach(NodeOutput no in node.Outputs)
            {
                VmNodeOutput VmOutput = new(no, this);
                Outputs.Add(VmOutput);
            }

        }

        #region Methods
        /// <summary>
        /// Delets the node
        /// </summary>
        public void Delete()
        {
            foreach(VmNodeOutput no in Outputs)
                no.VmOutput = null;

            foreach(VmNodeOutput no in _inputs.ToArray())
                no.VmOutput = null;

            VmGrid.Data.Nodes.Remove(Data);
            VmGrid.Nodes.Remove(this);
        }


        /// <summary>
        /// Adds a new node output
        /// </summary>
        public void AddOutput()
        {
            NodeOutput output = Data.AddOutput();
            VmNodeOutput vmOutput = new VmNodeOutput(output, this);
            Outputs.Add(vmOutput);
        }

        /// <summary>
        /// Deletes this output from the node
        /// </summary>
        /// <param name="vmOutput"></param>
        public void DeleteOutput(VmNodeOutput vmOutput)
        {
            if(Outputs.Count < 2)
                return;
            if(Outputs.Remove(vmOutput))
            {
                vmOutput.VmOutput = null;
                vmOutput.Displaying = false;
                Data.Outputs.Remove(vmOutput.Data);
            }
            RefreshColor();
        }


        /// <summary>
        /// Adds a new input to update on movement
        /// </summary>
        /// <param name="vmInput"></param>
        public void AddInput(VmNodeOutput vmInput)
        {
            if(_inputs.Count == 0 && _element != null)
                _element.LayoutUpdated += LayoutUpdated;
            _inputs.Add(vmInput);
        }

        /// <summary>
        /// Removes an input
        /// </summary>
        /// <param name="vmInput"></param>
        public void RemoveInput(VmNodeOutput vmInput)
        {
            _inputs.Remove(vmInput);
            if(_inputs.Count == 0)
                _element.LayoutUpdated -= LayoutUpdated;
        }


        /// <summary>
        /// Refreshes the position Properties
        /// </summary>
        public void RefreshPosition()
        {
            OnPropertyChanged(nameof(PositionX));
            OnPropertyChanged(nameof(PositionY));
        }

        /// <summary>
        /// Refreshes the Color properties
        /// </summary>
        public void RefreshColor()
        {
            OnPropertyChanged(nameof(CharacterColor));
            OnPropertyChanged(nameof(ExpressionColor));
        }

        /// <summary>
        /// Ends the grab cycle
        /// </summary>
        public void EndGrab()
        {
            VmGrid.Grabbed = null;

            int difX = Math.Abs(PositionX - (int)VmGrid.DragPosition.X);
            int difY = Math.Abs(PositionY - (int)VmGrid.DragPosition.Y);

            if(difX < 2 && difY < 2)
            {
                VmNode oldSelected = VmGrid.Active;
                VmGrid.Active = this;

                oldSelected?.OnPropertyChanged(nameof(IsActive));
                OnPropertyChanged(nameof(IsActive));
            }
            else
            {
                PositionX = (int)VmGrid.DragPosition.X + VmGrid.halfBrushDim * (VmGrid.DragPosition.X < 0 ? -1 : 1);
                PositionY = (int)VmGrid.DragPosition.Y + VmGrid.halfBrushDim * (VmGrid.DragPosition.Y < 0 ? -1 : 1);
            }
        }

        #endregion

        #region Event methods

        /// <summary>
        /// MouseDown event method <br/>
        /// Initiates moving the nodes
        /// </summary>
        private void MouseDown(MouseButtonEventArgs args)
        {
            if(args.LeftButton == MouseButtonState.Pressed)
            {
                VmGrid.DragPosition = new Point(PositionX, PositionY);
                VmGrid.Grabbed = this;
            }
            VmGrid.MouseDown(args);
        }

        /// <summary>
        /// MouseUp event method <br/>
        /// If a connection was being dragged, it will be connected to this node
        /// </summary>
        private void MouseUp()
        {
            if(VmGrid.Connecting == null)
                return;

            VmGrid.Connecting.VmOutput = this;
            VmGrid.Connecting = null;
        }

        private void LayoutLoaded(Canvas c)
        {
            _element = c;
            if(_inputs.Count > 0)
                _element.LayoutUpdated += LayoutUpdated;
        }

        private void LayoutUpdated(object sender, EventArgs args)
        {
            Point newPos = VmGrid.GetPosition(_element);

            if(newPos != _elementPosition)
            {
                _elementPosition = newPos;
                foreach(VmNodeOutput no in _inputs)
                    no.RefreshDisplay();
            }
        }

        #endregion

    }
}

