using SCRCommon.Viewmodels;
using SCRDialogEditor.Data;
using SCRDialogEditor.XAML;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

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

        #endregion

        /// <summary>
        /// Grid parent
        /// </summary>
        public VmGrid Grid { get; }

        /// <summary>
        /// Data object
        /// </summary>
        public Node Data { get; }

        #region Properties

        /// <summary>
        /// Viewmodel objects for the outputs
        /// </summary>
        public ObservableCollection<VmNodeOutput> Outputs { get; }

        /// <summary>
        /// Attached inputs (only for updating the lines)
        /// </summary>
        private readonly List<VmNodeOutput> _inputs;

        /// <summary>
        /// Outputs attached to this node
        /// </summary>
        public ReadOnlyCollection<VmNodeOutput> Inputs { get; }

        public bool RightPortrait
        {
            get => Data.RightPortrait;
            set => Data.RightPortrait = value;
        }

        /// <summary>
        /// Header Character Color
        /// </summary>
        public SolidColorBrush CharacterColor
            => Outputs[0].CharacterColor;

        /// <summary>
        /// Header Expression Color
        /// </summary>
        public SolidColorBrush ExpressionColor
            => Outputs[0].ExpressionColor;

        public string Name
            => Outputs[0].Name;
        public string InOutInfo
            => $"[ {Inputs.Count} ; {Outputs.Count} ]";

        public Point Position { get; private set; }

        /// <summary>
        /// Whether this Node is selected
        /// </summary>
        public bool IsActive
            => Grid.Active == this;

        public int UpdatePositionCounter { get; private set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="node"></param>
        public VmNode(VmGrid grid, Node node, Point position = default)
        {
            Grid = grid;
            Data = node;

            _inputs = new();
            Inputs = new ReadOnlyCollection<VmNodeOutput>(_inputs);

            Outputs = new();
            foreach(NodeOutput no in node.Outputs)
            {
                VmNodeOutput VmOutput = new(no, this);
                Outputs.Add(VmOutput);
            }

            if(position != default)
            {
                Position = position;
                UpdateDataPosition();
            }
            else
            {
                Position = UcGridEditor.FromGridSpace(
                    new(Data.LocationX, Data.LocationY)
                );
            }
        }

        #region Methods
        public void Move(Point dif)
        {
            Position = new Point(Position.X + dif.X, Position.Y + dif.Y);
        }

        /// <summary>
        /// Delets the node
        /// </summary>
        public void Disconnect()
        {
            foreach(VmNodeOutput no in Outputs)
                no.Disconnect();

            foreach(VmNodeOutput no in _inputs.ToArray())
                no.Disconnect();
        }

        #region Collection methods

        /// <summary>
        /// Adds a new node output
        /// </summary>
        public void AddOutput()
        {
            NodeOutput output = Data.CreateOutput();
            VmNodeOutput vmOutput = new(output, this);
            Outputs.Add(vmOutput);
            OnPropertyChanged(nameof(InOutInfo));
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
                Data.RemoveOutput(vmOutput.Data);
            }
            RefreshColor();
            OnPropertyChanged(nameof(InOutInfo));
        }


        /// <summary>
        /// Adds a new input to update on movement
        /// </summary>
        /// <param name="vmInput"></param>
        public void AddInput(VmNodeOutput vmInput)
        {
            _inputs.Add(vmInput);
            UpdatePositionCounter++;
            OnPropertyChanged(nameof(InOutInfo));
        }

        /// <summary>
        /// Removes an input
        /// </summary>
        /// <param name="vmInput"></param>
        public void RemoveInput(VmNodeOutput vmInput)
        {
            _inputs.Remove(vmInput);
            UpdatePositionCounter--;
            OnPropertyChanged(nameof(InOutInfo));
        }


        /// <summary>
        /// Displays an output connection
        /// </summary>
        /// <param name="vmOutput"></param>
        public void DisplayOutput(VmNodeOutput vmOutput)
        {
            if(!Outputs.Contains(vmOutput) || vmOutput.Displaying)
                return;

            Grid.Outputs.Add(vmOutput);
            UpdatePositionCounter++;
        }

        /// <summary>
        /// Hides an output connection
        /// </summary>
        /// <param name="vmOutput"></param>
        public void HideOutput(VmNodeOutput vmOutput)
        {
            if(!Outputs.Contains(vmOutput) || !vmOutput.Displaying)
                return;

            Grid.Outputs.Remove(vmOutput);
            UpdatePositionCounter--;
        }

        #endregion

        /// <summary>
        /// Refreshes the Color properties
        /// </summary>
        public void RefreshColor()
        {
            OnPropertyChanged(nameof(CharacterColor));
            OnPropertyChanged(nameof(ExpressionColor));
            OnPropertyChanged(nameof(Name));
        }

        public void RefreshActive()
            => OnPropertyChanged(nameof(IsActive));

        /// <summary>
        /// Ends the grab cycle
        /// </summary>
        public void EndGrab()
        {
            Point start = UcGridEditor.FromGridSpace(
                new(Data.LocationX, Data.LocationY)
            );

            int difX = (int)Math.Abs(start.X - Position.X);
            int difY = (int)Math.Abs(start.Y - Position.Y);

            if(difX < 2 && difY < 2)
                Grid.Active = this;
            else
                UpdateDataPosition();
        }

        public void UpdateDataPosition()
        {
            Point dataLoc = UcGridEditor.ToGridSpace(Position);
            Data.LocationX = (int)dataLoc.X;
            Data.LocationY = (int)dataLoc.Y;

            Position = UcGridEditor.FromGridSpace(dataLoc);
        }

        #endregion

    }
}

