using SCRCommon.Viewmodels;
using SCRDialogEditor.Data;
using SCRDialogEditor.XAML;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Linq;

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

        private ChangeTracker Tracker => Grid.Tracker;

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

        public bool IsSelected
            => Grid.Selected.Contains(this);

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

            foreach(VmNodeOutput no in Outputs)
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
                Outputs,
                vmOutput,
                Outputs.Count,
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
            if(Outputs.Count < 2)
                return;

            Tracker.BeginGroup();

            vmOutput.Disconnect();

            Tracker.TrackChange(new ChangedListSingleEntry<VmNodeOutput>(
                Outputs,
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
            if(!Outputs.Contains(vmOutput) || vmOutput.Displaying)
                return;

            Tracker.BeginGroup();

            Grid.RegisterOutput(vmOutput);
            UpdatePositionCounter++;

            Tracker.EndGroup();
        }

        /// <summary>
        /// Hides an output connection
        /// </summary>
        /// <param name="vmOutput"></param>
        public void HideOutput(VmNodeOutput vmOutput)
        {
            if(!Outputs.Contains(vmOutput) || !vmOutput.Displaying)
                return;

            Tracker.BeginGroup();

            Grid.DeregisterOutput(vmOutput);
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

            Point dataLoc = UcGridEditor.ToGridSpace(Position);
            Data.LocationX = (int)dataLoc.X;
            Data.LocationY = (int)dataLoc.Y;

            Tracker.PostGroupAction(() => Position = UcGridEditor.FromGridSpace(Data.LocationX, Data.LocationY));

            Tracker.EndGroup();
        }

        #endregion

        public override string ToString()
            => $"{Name} - {InOutInfo}";
    }
}

