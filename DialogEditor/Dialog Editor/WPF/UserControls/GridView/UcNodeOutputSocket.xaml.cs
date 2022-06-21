using SCR.Tools.DialogEditor.Viewmodeling;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SCR.Tools.DialogEditor.WPF.UserControls.GridView
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
            if(e.PropertyName == nameof(VmNodeOutput.Connected))
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

        public void SetEndPosition(Point canvasPos)
        {
            // 38 is the exact y offset at which the center of the input socket lies
            canvasPos.Y += 38;
            _connectionControl.SetEndPosition(canvasPos);
        }

        private void UpdateConnection()
        {
            if (_parent == null)
            {
                throw new InvalidOperationException();
            }

            bool inConnections = _parent._gridView.NodeConnections.Items.Contains(_connectionControl);

            if (inConnections && Connected == null)
            {
                _parent._gridView.NodeConnections.Items.Remove(_connectionControl);
            }
            else if(!inConnections && Connected != null)
            {
                _parent._gridView.NodeConnections.Items.Add(_connectionControl);
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
                    _connectedNode = _parent._gridView.GetNodeControl(Connected);
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
            if(_parent == null || Connected == null)
            {
                throw new InvalidOperationException();
            }

            _connectedNode = _parent._gridView.GetNodeControl(Connected);
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
    }
}
