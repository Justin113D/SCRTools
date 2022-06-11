using SCR.Tools.UndoRedo;
using SCR.Tools.Viewmodeling;
using SCR.Tools.DialogEditor.Data;
using System;
using System.Windows;
using System.Windows.Media;

namespace SCR.Tools.DialogEditor.Viewmodel
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
            => new(() => Parent.DeleteOutput(this));

        public RelayCommand Cmd_RemoveOutput
            => new(() => VmOutput = null);
        #endregion

        /// <summary>
        /// Parent Node Viewmodel
        /// </summary>
        public VmNode Parent { get; }

        /// <summary>
        /// Data model
        /// </summary>
        public NodeOutput Data { get; }

        private ChangeTracker Tracker => Parent.Grid.Tracker;

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
                    return Parent.Grid.Main.DialogOptions.VMCharacterOptions.GetOption(Data.Character);
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
                    Parent.RefreshColor();
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
                    return Parent.Grid.Main.DialogOptions.VMExpressionOptions.GetOption(Data.Expression);
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
                    Parent.RefreshColor();
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
                    return Parent.Grid.Main.DialogOptions.VMNodeIcons.GetIcon(Data.Icon);
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
                if(_vmOutput == value || _vmOutput == Parent)
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
            get => Parent.Grid.Outputs.Contains(this);
            set
            {
                if(value == Displaying)
                    return;

                if(value)
                    Parent.DisplayOutput(this);
                else
                    Parent.HideOutput(this);
            }
        }

        /// <summary>
        /// Connection Curve
        /// </summary>
        public PathGeometry Line { get; private set; }
        public readonly PathGeometry _line;

        /// <summary>
        /// Whether the line is backwards
        /// </summary>
        public bool IsLineBackwards { get; private set; }

        /// <summary>
        /// Global Line Start location
        /// </summary>
        public Point LineFrom { get; private set; }

        /// <summary>
        /// Global Line Target Location
        /// </summary>
        private Point _lineTo;

        #endregion

        public VmNodeOutput(NodeOutput nodeOutput, VmNode parent)
        {
            Data = nodeOutput;
            Parent = parent;

            _line = new()
            {
                Figures = new()
                {
                    new PathFigure()
                    {
                        StartPoint = default,
                        Segments = new()
                        {
                            new BezierSegment(default, default, default, true)
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Sets the starting position of the connection line
        /// </summary>
        public void UpdateStartPosition(Point newStartPosition)
        {
            LineFrom = newStartPosition;
            UpdateLine();
        }

        /// <summary>
        /// Sets the end position of the connection line
        /// </summary>
        /// <param name="Endposition"></param>
        public void UpdateEndPosition(Point Endposition)
        {
            _lineTo = Endposition;
            UpdateLine();
        }

        private void UpdateLine()
        {
            Point to = new(_lineTo.X - LineFrom.X, _lineTo.Y - LineFrom.Y);
            if(VmOutput != null)
            {
                if(IsLineBackwards != to.X < Math.Abs(to.Y * 0.2d))
                {
                    IsLineBackwards = !IsLineBackwards;
                    VmOutput.RefreshBackwardsConnection();
                }
            }
            else
            {
                IsLineBackwards = false;
            }

            if(!IsLineBackwards)
            {
                int bt = (int)(to.X / 3d);
                Point b1 = new(bt, 0);
                Point b2 = new(bt * 2, to.Y);

                BezierSegment bs = (BezierSegment)_line.Figures[0].Segments[0];
                bs.Point1 = b1;
                bs.Point2 = b2;
                bs.Point3 = to;
                Line = _line;
            }
            else
                Line = null;
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
