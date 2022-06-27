using SCR.Tools.Dialog.Editor.Viewmodeling;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SCR.Tools.Dialog.Editor.WPF.UserControls.GridView
{
    /// <summary>
    /// Interaction logic for UcNodeOutputSocket.xaml
    /// </summary>
    public partial class UcNodeOutputSocket : UserControl
    {
        private UcNode? _parent;

        private UcNode? _connectedNode;

        private bool _subscribedEvent;

        private readonly UcNodeConnection _connectionControl;

        private Point _NodeOffset = default;

        private Point _CanvasOffset = default;

        private bool _clickCheck;

        private VmNodeOutput Viewmodel
            => (VmNodeOutput)DataContext;

        private VmNode? Connected
            => Viewmodel.Connected;

        public UcNodeOutputSocket()
        {
            DataContextChanged += OnDataContextChanged;
            Unloaded += OnUnloaded;
            InitializeComponent();
            _connectionControl = new();
        }

        #region Control Event handlers

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(e.OldValue != null)
            {
                // unexpected behavior
                throw new InvalidOperationException();
            }

            if (e.NewValue is INotifyPropertyChanged newINotify)
            {
                newINotify.PropertyChanged += OnPropertyChanged;
            }
        }

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // not using direct bindings to prevent access after unloading
            // (because apparently that can happen and caused me a huge headache)

            if (e.PropertyName == nameof(VmNodeOutput.Connected))
            {
                UpdateConnection();
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Viewmodel.PropertyChanged -= OnPropertyChanged;
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);

            if(_parent != null)
            {
                throw new InvalidOperationException();
            }

            DependencyObject element = VisualParent;
            while(element is not UcNode)
            {
                element = VisualTreeHelper.GetParent(element);
            }
            _parent = (UcNode)element;
            UpdateConnection();
        }
        
        #endregion

        public void RecalculateNodeOffset(UcNode relativeTo)
        {
            _parent = relativeTo;

            Point center = new(Socket.ActualWidth / 2, Socket.ActualHeight / 2);
            _NodeOffset = Socket.TranslatePoint(center, _parent);
            UpdateStartPosition();
        }

        public void SetCanvasPosition(Point canvasPos)
        {
            _CanvasOffset = canvasPos;
            UpdateStartPosition();
        }

        public void SetEndPosition(Point canvasPos, bool addOffset = true)
        {
            // 38 is the exact y offset at which the center of the input socket lies
            if(addOffset)
            {
                canvasPos.Y += 38;
            }

            _connectionControl.SetEndPosition(canvasPos);
        }

        private void UpdateConnection()
        {
            if (_parent == null || _parent.GridView == null)
            {
                throw new InvalidOperationException();
            }

            bool inConnections = _parent.GridView.NodeConnections.Items.Contains(_connectionControl);

            if (inConnections && Connected == null)
            {
                _parent.GridView.NodeConnections.Items.Remove(_connectionControl);
            }
            else if(!inConnections && Connected != null)
            {
                _parent.GridView.NodeConnections.Items.Add(_connectionControl);
            }

            _connectedNode?.ConnectedSockets.Remove(this);

            if (Connected == null)
            {
                _connectedNode = null;
            }
            else
            {
                try
                {
                    _connectedNode = _parent.GridView.GetNodeControl(Connected);
                    _connectedNode.ConnectedSockets.Add(this);
                    SetEndPosition(new(_connectedNode.CanvasX, _connectedNode.CanvasY));
                }
                catch(NodeControlNotGeneratedException e)
                {
                    if(_subscribedEvent)
                    {
                        throw;
                    }

                    e.Content.Loaded += Content_Loaded;
                    _connectedNode = null;
                    _subscribedEvent = true;
                }
            }

        }

        private void Content_Loaded(object sender, RoutedEventArgs e)
        {
            if(_parent == null || _parent.GridView == null || Connected == null)
            {
                throw new InvalidOperationException();
            }

            _connectedNode = _parent.GridView.GetNodeControl(Connected);
            _connectedNode.ConnectedSockets.Add(this);
            SetEndPosition(new(_connectedNode.CanvasX, _connectedNode.CanvasY));

            ((ContentPresenter)sender).Loaded -= Content_Loaded;
            _subscribedEvent = false;
        }

        private void UpdateStartPosition()
        {
            Point absolutePosition = new(
                _CanvasOffset.X + _NodeOffset.X,
                _CanvasOffset.Y + _NodeOffset.Y);

            _connectionControl.SetStartPosition(absolutePosition);
        }

        public override string ToString()
            => ((VmNodeOutput)DataContext).Name;

        private void Socket_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_parent?.GridView == null || _parent.GridView.ConnectingSocket != null)
            {
                throw new InvalidOperationException();
            }

            if(e.ChangedButton == MouseButton.Left)
            {
                _clickCheck = true;
            }
        }

        private void Socket_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_parent?.GridView == null)
            {
                throw new InvalidOperationException();
            }

            if (!_clickCheck)
            {
                return;
            }

            _clickCheck = false;
            _parent.GridView.SetDraggedConnection(this);

            if(Connected == null)
            {
                _parent.GridView.NodeConnections.Items.Add(_connectionControl);
            }
        }
    
        public void DropConnection(VmNode? node)
        {
            if (_parent?.GridView == null || _parent.GridView.ConnectingSocket != this)
            {
                throw new InvalidOperationException();
            }

            _parent.GridView.SetDraggedConnection(null);

            if (node == null)
            {
                if (Connected == null)
                {
                    _parent.GridView.NodeConnections.Items.Remove(_connectionControl);
                }
                else
                { 
                    Viewmodel.Disconnect();
                }
            }
            else
            {
                Viewmodel.Connected = node;
            }

        }
    }
}
