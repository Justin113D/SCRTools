using SCRCommon.Viewmodels;
using SCRDialogEditor.Data;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SCRDialogEditor.Viewmodel
{
    /// <summary>
    /// Viewmodel for Node Output
    /// </summary>
    public class VmNodeOutput : BaseViewModel
    {
        #region Relay Command
        /// <summary>
        /// Command for deleting this node output
        /// </summary>
        public RelayCommand Cmd_Delete 
            => new(Delete);

        /// <summary>
        /// Mouse Leave Command
        /// </summary>
        public RelayCommand<MouseEventArgs> Cmd_MouseLeave 
            => new(MouseLeave);

        /// <summary>
        /// Socket layout updated command
        /// </summary>
        public RelayCommand<Border> Cmd_SocketLoaded 
            => new(SocketLayoutLoaded);
        #endregion

        /// <summary>
        /// Parent Node Viewmodel
        /// </summary>
        private readonly VmNode _parent;

        private FrameworkElement _socketElement;

        /// <summary>
        /// Last registered position of the socket control
        /// </summary>
        private Point _socketPosition;

        /// <summary>
        /// Viewmodel object for the current output
        /// </summary>
        private VmNode _vmOutput;

        /// <summary>
        /// Data model
        /// </summary>
        public NodeOutput Data { get; }


        #region Properties

        /// <summary>
        /// Name of the output
        /// </summary>
        public string Name 
            => $"{Data.Expression ?? "[None]"} {Data.Character ?? "[None]"}";

        /// <summary>
        /// Whether the output is expanded in the list
        /// </summary>
        public bool IsExpanded { get; set; }

        public bool KeepEnabled
        {
            get => Data.KeepEnabled;
            set => Data.KeepEnabled = value;
        }

        public bool Displaying
        {
            get => _parent.VmGrid.Outputs.Contains(this);
            set
            {
                if(value == Displaying)
                    return;
                if(value)
                {
                    if(_socketElement != null)
                        _socketElement.LayoutUpdated += SocketLayoutUpdated;
                    _parent.VmGrid.Outputs.Add(this);
                }
                else
                {
                    _parent.VmGrid.Outputs.Remove(this);
                    _socketElement.LayoutUpdated -= SocketLayoutUpdated;
                }
            }
        }

        #region Character

        public string CharacterText
            => string.IsNullOrWhiteSpace(Data.Character) ? "<Character>" : Data.Character;

        public SolidColorBrush CharacterColor 
            => new(Character?.Color ?? Colors.Gray);

        public NodeOption Character
        {
            get
            {
                if(string.IsNullOrWhiteSpace(Data.Character))
                    return null;

                try
                {
                    return _parent.VmGrid.VmMain.DialogOptions.VMCharacterOptions.RawOptions.Find(x => x.Name == Data.Character);
                }
                catch(Exception) { }
                return null;
            }
            set
            {
                Data.Character = value.Name;
                OnPropertyChanged(nameof(CharacterColor));
                OnPropertyChanged(nameof(Name));
                _parent.RefreshColor();
            }
        }

        #endregion

        #region Expression

        public string ExpressionText 
            => string.IsNullOrWhiteSpace(Data.Expression) ? "<Expression>" : Data.Expression;

        public SolidColorBrush ExpressionColor 
            => new(Expression?.Color ?? Colors.Gray);

        public NodeOption Expression
        {
            get
            {
                if(string.IsNullOrWhiteSpace(Data.Expression))
                    return null;

                try
                {
                    return _parent.VmGrid.VmMain.DialogOptions.VMExpressionOptions.RawOptions.Find(x => x.Name == Data.Expression);
                }
                catch(Exception) { }
                return null;
            }
            set
            {
                Data.Expression = value.Name;
                OnPropertyChanged(nameof(ExpressionColor));
                OnPropertyChanged(nameof(Name));
                _parent.RefreshColor();
            }
        }

        #endregion

        /// <summary>
        /// Output Text
        /// </summary>
        public string Text
        {
            get => Data.Text;
            set => Data.Text = value;
        }

        public string Condition
        {
            get => Data.Condition;
            set => Data.SetCondition(value);
        }

        /// <summary>
        /// Connection Curve
        /// </summary>
        public PathGeometry Line
        {
            get
            {
                if(VmOutput == null && _parent.VmGrid.Connecting != this)
                    return new PathGeometry();

                Point Start = _socketPosition;

                Point End;
                if(VmOutput != null)
                    End = new Point(VmOutput.PositionX, VmOutput.PositionY + 43);
                else
                    End = _parent.VmGrid.DragPosition;

                int bt = (int)((End.X - Start.X) / 3f);

                Point b1 = new(Start.X + bt, Start.Y);
                Point b2 = new(End.X - bt, End.Y);

                return new()
                {
                    Figures = new()
                    {
                        new PathFigure()
                        {
                            StartPoint = Start,
                            Segments = new()
                            {
                                new BezierSegment(b1, b2, End, true)
                            }
                        }
                    }
                };
            }
        }

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
                if(Data.SetOutput(value?.Data))
                {
                    _vmOutput?.RemoveInput(this);
                    _vmOutput = value;
                    _vmOutput?.AddInput(this);

                    RefreshDisplay();
                }
            }
        }

        #endregion

        public VmNodeOutput(NodeOutput nodeOutput, VmNode parent)
        {
            Data = nodeOutput;
            _parent = parent;
        }

        /// <summary>
        /// Reloads the connection curve
        /// </summary>
        public void RefreshDisplay() 
            => OnPropertyChanged(nameof(Line));

        #region Event / Command Methods

        /// <summary>
        /// Deletes the node output
        /// </summary>
        private void Delete()
        {
            _parent.DeleteOutput(this);
        }

        /// <summary>
        /// MouseLeave event method <br/>
        /// Initiates connection dragging
        /// </summary>
        private void MouseLeave(MouseEventArgs e)
        {
            e.Handled = true;
            if(e.LeftButton != MouseButtonState.Pressed)
                return;

            VmOutput = null;
            _parent.VmGrid.Connecting = this;
            RefreshDisplay();
        }

        private void SocketLayoutLoaded(Border b)
        {
            _socketElement = b;
            if(Displaying)
            {
                _socketElement.LayoutUpdated += SocketLayoutUpdated;
                SocketLayoutUpdated(null, null);
            }
        }

        /// <summary>
        /// LayoutUpdated event method <br/>
        /// Gets called whenever the socket controls layout is updated
        /// </summary>
        private void SocketLayoutUpdated(object sender, EventArgs args)
        {
            Point newPos = _parent.VmGrid.GetPosition(_socketElement);
            newPos.X += 9;
            newPos.Y += 9;

            if(newPos != _socketPosition)
            {
                _socketPosition = newPos;
                RefreshDisplay();
            }
        }

        #endregion
    }
}
