﻿using SCRCommon.Viewmodels;
using SCRDialogEditor.Data;
using System.Collections.ObjectModel;
using System.Linq;
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

        #region private fields
        private int _updateCounter;
        #endregion

        /// <summary>
        /// Grid parent
        /// </summary>
        public VmGrid Grid { get; }

        /// <summary>
        /// Data object
        /// </summary>
        public Node Data { get; }

        private ChangeTracker Tracker => Grid?.Tracker;

        #region Properties

        private readonly ObservableCollection<VmNodeOutput> _outputs;

        /// <summary>
        /// Viewmodel objects for the outputs
        /// </summary>
        public ReadOnlyObservableCollection<VmNodeOutput> Outputs { get; }

        private readonly ObservableCollection<VmNodeOutput> _inputs;

        /// <summary>
        /// Outputs attached to this node
        /// </summary>
        public ReadOnlyObservableCollection<VmNodeOutput> Inputs { get; }

        public bool RightPortrait
        {
            get => Data.RightPortrait;
            set
            {
                Tracker.BeginGroup();
                Data.RightPortrait = value;
                Tracker.PostGroupAction(() => OnPropertyChanged(nameof(RightPortrait)));
                Tracker.EndGroup();
            }
        }

        /// <summary>
        /// Header Character Color
        /// </summary>
        public SolidColorBrush CharacterColor
            => _outputs[0].CharacterColor;

        /// <summary>
        /// Header Expression Color
        /// </summary>
        public SolidColorBrush ExpressionColor
            => _outputs[0].ExpressionColor;

        public string Name
            => _outputs[0].Name;

        public string InOutInfo
            => $"[ {_inputs.Count} ; {_outputs.Count} ]";

        public Point Position { get; private set; }

        /// <summary>
        /// Whether this Node is selected
        /// </summary>
        public bool IsActive
            => Grid?.Active == this;

        public bool IsSelected
            => Grid?.Selected.Contains(this) == true;

        public int UpdatePositionCounter
        {
            get => _updateCounter;
            set
            {
                Tracker.BeginGroup();

                int oldValue = _updateCounter;

                Tracker.TrackChange(new ChangedValue<int>(
                    (v) => _updateCounter = v,
                    oldValue,
                    value
                ));

                Tracker.PostGroupAction(() => OnPropertyChanged(nameof(UpdatePositionCounter)));
                Tracker.EndGroup();
            }
        }

        public bool HasBackwardsInput
            => _inputs.Any(x => x.IsLineBackwards);
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
            Inputs = new(_inputs);

            
            _outputs = new();
            foreach(NodeOutput no in node.Outputs)
            {
                VmNodeOutput VmOutput = new(no, this);
                _outputs.Add(VmOutput);
            }
            Outputs = new(_outputs);

            if(position != default)
            {
                Position = position;
                UpdateDataPosition();
            }
            else
            {
                Position = VmGrid.FromGridSpace(
                    Data.LocationX,
                    Data.LocationY
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
            Tracker.BeginGroup();

            foreach(VmNodeOutput no in _outputs)
                no.Disconnect();

            foreach(VmNodeOutput no in _inputs.ToArray())
                no.Disconnect();

            Tracker.EndGroup();
        }

        #region Collection methods

        /// <summary>
        /// Adds a new node output
        /// </summary>
        public void AddOutput()
        {
            Tracker.BeginGroup();

            NodeOutput output = Data.CreateOutput();
            VmNodeOutput vmOutput = new(output, this);

            Tracker.TrackChange(new ChangedListSingleEntry<VmNodeOutput>(
                _outputs,
                vmOutput,
                _outputs.Count,
                null
            ));

            Tracker.PostGroupAction(() => OnPropertyChanged(nameof(InOutInfo)));

            Tracker.EndGroup();
        }

        /// <summary>
        /// Deletes this output from the node
        /// </summary>
        /// <param name="vmOutput"></param>
        public void DeleteOutput(VmNodeOutput vmOutput)
        {
            if(_outputs.Count < 2)
                return;

            Tracker.BeginGroup();

            vmOutput.Disconnect();

            Tracker.TrackChange(new ChangedListSingleEntry<VmNodeOutput>(
                _outputs,
                vmOutput,
                null,
                null
            ));

            Data.RemoveOutput(vmOutput.Data);

            Tracker.PostGroupAction(() =>
            {
                RefreshColor();
                OnPropertyChanged(nameof(InOutInfo));
            });

            Tracker.EndGroup();
        }


        /// <summary>
        /// Adds a new input to update on movement
        /// </summary>
        /// <param name="vmInput"></param>
        public void AddInput(VmNodeOutput vmInput)
        {
            Tracker.BeginGroup();

            Tracker.TrackChange(new ChangedListSingleEntry<VmNodeOutput>(
                _inputs,
                vmInput,
                _inputs.Count,
                null
            ));

            UpdatePositionCounter++;

            Tracker.PostGroupAction(() =>
            {
                OnPropertyChanged(nameof(InOutInfo));
                RefreshBackwardsConnection();
            });

            Tracker.EndGroup();
        }

        /// <summary>
        /// Removes an input
        /// </summary>
        /// <param name="vmInput"></param>
        public void RemoveInput(VmNodeOutput vmInput)
        {
            Tracker.BeginGroup();

            Tracker.TrackChange(new ChangedListSingleEntry<VmNodeOutput>(
                _inputs,
                vmInput,
                null,
                null
            ));

            UpdatePositionCounter--;

            Tracker.PostGroupAction(() =>
            {
                OnPropertyChanged(nameof(InOutInfo));
                RefreshBackwardsConnection();
            });

            Tracker.EndGroup();
        }


        /// <summary>
        /// Displays an output connection
        /// </summary>
        /// <param name="vmOutput"></param>
        public void DisplayOutput(VmNodeOutput vmOutput)
        {
            if(!_outputs.Contains(vmOutput) || vmOutput.Displaying)
                return;

            Tracker.BeginGroup();

            Grid?.RegisterOutput(vmOutput);
            UpdatePositionCounter++;

            Tracker.EndGroup();
        }

        /// <summary>
        /// Hides an output connection
        /// </summary>
        /// <param name="vmOutput"></param>
        public void HideOutput(VmNodeOutput vmOutput)
        {
            if(!_outputs.Contains(vmOutput) || !vmOutput.Displaying)
                return;

            Tracker.BeginGroup();

            Grid?.DeregisterOutput(vmOutput);
            UpdatePositionCounter--;

            Tracker.EndGroup();
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

        public void RefreshSelected()
            => OnPropertyChanged(nameof(IsSelected));

        public void RefreshBackwardsConnection()
            => OnPropertyChanged(nameof(HasBackwardsInput));

        public void UpdateDataPosition()
        {
            Tracker.BeginGroup();

            Point dataLoc = VmGrid.ToGridSpace(Position);
            Data.LocationX = (int)dataLoc.X;
            Data.LocationY = (int)dataLoc.Y;

            Tracker.PostGroupAction(() => Position = VmGrid.FromGridSpace(Data.LocationX, Data.LocationY));

            Tracker.EndGroup();
        }

        #endregion

        public override string ToString()
            => $"{Name} - {InOutInfo}";
    }
}

