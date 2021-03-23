using SCRCommon.Viewmodels;
using SCRDialogEditor.Data;
using System;
using System.Windows;
using System.Windows.Media;

namespace SCRDialogEditor.Viewmodel
{
    /// <summary>
    /// Viewmodel for Node Output
    /// </summary>
    public class VmNodeOutput : BaseViewModel
    {
        #region Commands

        /// <summary>
        /// Command for deleting this node output
        /// </summary>
        public RelayCommand Cmd_Delete
            => new(() => _parent.DeleteOutput(this));

        #endregion

        /// <summary>
        /// Parent Node Viewmodel
        /// </summary>
        private readonly VmNode _parent;

        /// <summary>
        /// Data model
        /// </summary>
        public NodeOutput Data { get; }

        private ChangeTracker Tracker => _parent.Grid.Tracker;

        #region Wrapper Properties

        #region Character

        public string CharacterText
            => string.IsNullOrWhiteSpace(Data.Character) ? "<Character>" : Data.Character;

        public SolidColorBrush CharacterColor
            => Character?.Color ?? new(Colors.Gray);

        public VmNodeOption Character
        {
            get
            {
                if(string.IsNullOrWhiteSpace(Data.Character))
                    return null;

                try
                {
                    return _parent.Grid.Main.DialogOptions.VMCharacterOptions.GetOption(Data.Character);
                }
                catch(Exception) { }
                return null;
            }
            set
            {
                Tracker.BeginGroup();
                Data.Character = value.Name;

                Tracker.PostGroupAction(() =>
                {
                    OnPropertyChanged(nameof(Character));
                    OnPropertyChanged(nameof(CharacterText));
                    OnPropertyChanged(nameof(CharacterColor));
                    OnPropertyChanged(nameof(Name));
                    _parent.RefreshColor();
                });

                Tracker.EndGroup();
            }
        }

        #endregion

        #region Expression

        public string ExpressionText
            => string.IsNullOrWhiteSpace(Data.Expression) ? "<Expression>" : Data.Expression;

        public SolidColorBrush ExpressionColor
            => Expression?.Color ?? new(Colors.Gray);

        public VmNodeOption Expression
        {
            get
            {
                if(string.IsNullOrWhiteSpace(Data.Expression))
                    return null;

                try
                {
                    return _parent.Grid.Main.DialogOptions.VMExpressionOptions.GetOption(Data.Expression);
                }
                catch(Exception) { }
                return null;
            }
            set
            {
                Tracker.BeginGroup();
                Data.Expression = value.Name;

                Tracker.PostGroupAction(() =>
                {
                    OnPropertyChanged(nameof(Expression));
                    OnPropertyChanged(nameof(ExpressionText));
                    OnPropertyChanged(nameof(ExpressionColor));
                    OnPropertyChanged(nameof(Name));
                    _parent.RefreshColor();
                });

                Tracker.EndGroup();
            }
        }

        #endregion

        #region Node Icon

        public string NodeIconText
            => string.IsNullOrWhiteSpace(Data.Icon) ? "<No icon>" : Data.Icon;

        public string NodeIconPath
            => NodeIcon?.FullFilePath;

        public VmNodeIcon NodeIcon
        {
            get
            {
                if(string.IsNullOrWhiteSpace(Data.Icon))
                    return null;

                try
                {
                    return _parent.Grid.Main.DialogOptions.VMNodeIcons.GetIcon(Data.Icon);
                }
                catch(Exception) { }
                return null;
            }
            set
            {
                Tracker.BeginGroup();
                Data.Icon = value.Name;
                Tracker.PostGroupAction(() =>
                {
                    OnPropertyChanged(nameof(NodeIcon));
                    OnPropertyChanged(nameof(NodeIconPath));
                    OnPropertyChanged(nameof(NodeIconText));
                });
                Tracker.EndGroup();
            }
        }

        #endregion


        /// <summary>
        /// Output Text
        /// </summary>
        public string Text
        {
            get => Data.Text;
            set
            {

                Tracker.BeginGroup();
                Data.Text = value;
                Tracker.PostGroupAction(() => OnPropertyChanged(nameof(Text)));
                Tracker.EndGroup();
            }
        }

        public string Condition
        {
            get => Data.Condition;
            set
            {
                Tracker.BeginGroup();
                Data.SetCondition(value);
                Tracker.PostGroupAction(() => OnPropertyChanged(nameof(Condition)));
                Tracker.EndGroup();
            }
        }

        public bool KeepEnabled
        {
            get => Data.KeepEnabled;
            set
            {
                Tracker.BeginGroup();
                Data.KeepEnabled = value;
                Tracker.PostGroupAction(() => OnPropertyChanged(nameof(KeepEnabled)));
                Tracker.EndGroup();
            }
        }

        /// <summary>
        /// Viewmodel object for the current output
        /// </summary>
        private VmNode _vmOutput;

        /// <summary>
        /// Viewmodel Output
        /// </summary>
        public VmNode VmOutput
        {
            get => _vmOutput;
            set
            {
                if(_vmOutput == value || _vmOutput == _parent)
                    return;
                Tracker.BeginGroup();

                if(Data.SetOutput(value?.Data))
                {
                    _vmOutput?.RemoveInput(this);

                    var oldValue = _vmOutput;

                    Tracker.TrackChange(new ChangedValue<VmNode>(
                        (v) => _vmOutput = v,
                        oldValue,
                        value
                    ));

                    Tracker.PostGroupAction(() => OnPropertyChanged(nameof(VmOutput)));

                    Displaying = _vmOutput != null;
                    _vmOutput?.AddInput(this);
                }

                Tracker.EndGroup();
            }
        }

        #endregion

        #region Display Properties

        /// <summary>
        /// Name of the output
        /// </summary>
        public string Name
            => $"{Data.Expression ?? "[None]"} {Data.Character ?? "[None]"}";

        /// <summary>
        /// Whether the output is expanded in the list
        /// </summary>
        public bool IsExpanded { get; set; }

        /// <summary>
        /// Whether the Socket Connection needs to be displayed
        /// </summary>
        public bool Displaying
        {
            get => _parent.Grid.Outputs.Contains(this);
            set
            {
                if(value == Displaying)
                    return;

                if(value)
                    _parent.DisplayOutput(this);
                else
                    _parent.HideOutput(this);
            }
        }

        private readonly PathFigure _figure;

        /// <summary>
        /// Connection Curve
        /// </summary>
        public PathGeometry Line { get; }

        #endregion

        public VmNodeOutput(NodeOutput nodeOutput, VmNode parent)
        {
            Data = nodeOutput;
            _parent = parent;

            _figure = new()
            {
                StartPoint = default,
                Segments = new()
                {
                    new BezierSegment(default, default, default, true)
                }
            };

            Line = new()
            {
                Figures = new()
                {
                    _figure
                }
            };
        }

        /// <summary>
        /// Sets the starting position of the connection line
        /// </summary>
        public void UpdateStartPosition(Point newStartPosition)
        {
            SetLinePosition(newStartPosition, ((BezierSegment)_figure.Segments[0]).Point3);
        }

        /// <summary>
        /// Sets the end position of the connection line
        /// </summary>
        /// <param name="Endposition"></param>
        public void UpdateEndPosition(Point Endposition)
        {
            SetLinePosition(_figure.StartPoint, Endposition);
        }

        private void SetLinePosition(Point start, Point end)
        {
            int bt = (int)((end.X - start.X) / 3f);

            Point b1 = new(start.X + bt, start.Y);
            Point b2 = new(end.X - bt, end.Y);

            _figure.StartPoint = start;
            BezierSegment bs = (BezierSegment)_figure.Segments[0];
            bs.Point1 = b1;
            bs.Point2 = b2;
            bs.Point3 = end;
            OnPropertyChanged(nameof(Line));
        }

        /// <summary>
        /// Disconnects the output from the node that its connected to
        /// </summary>
        public void Disconnect()
        {
            VmOutput = null;
        }
    }
}
