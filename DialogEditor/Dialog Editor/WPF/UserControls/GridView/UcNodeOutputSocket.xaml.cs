using SCR.Tools.DialogEditor.Viewmodeling;
using System;
using System.Collections.Generic;
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
        public static readonly DependencyProperty ConnectedProperty =
            DependencyProperty.Register(
                nameof(Connected),
                typeof(VmNode),
                typeof(UcNodeOutputSocket),
                new((o, e) => ((UcNodeOutputSocket)o).UpdateConnection())
            );

        public VmNode Connected
        {
            get => (VmNode)GetValue(ConnectedProperty);
            set => SetValue(ConnectedProperty, value);
        }


        private UcNode? _parent;

        private UcNode? _connectedNode;

        private bool _addedConnection;

        private readonly UcNodeConnection _connectionControl;

        private Point _NodeOffset = default;

        private Point _CanvasOffset = default;


        public UcNodeOutputSocket()
        {
            InitializeComponent();
            _connectionControl = new(this);
        }

        
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
            if(_parent == null)
            {
                throw new InvalidOperationException();
            }

            if(_addedConnection && Connected == null)
            {
                _parent.GridView.NodeConnections.Items.Remove(_connectionControl);
                _addedConnection = false;
            }
            else if(Connected != null && !_addedConnection)
            {
                _parent.GridView.NodeConnections.Items.Add(_connectionControl);
                _addedConnection = true;
            }


            _connectedNode?.ConnectedSockets.Remove(this);

            if (Connected == null)
            {
                _connectedNode = null;
            }
            else
            {
                _connectedNode = _parent.GridView.GetNodeControl(Connected);
                SetEndPosition(new(_connectedNode.CanvasX, _connectedNode.CanvasY));
            }

            _connectedNode?.ConnectedSockets.Add(this);
        }

        private void UpdateStartPosition()
        {
            Point absolutePosition = new(
                _CanvasOffset.X + _NodeOffset.X,
                _CanvasOffset.Y + _NodeOffset.Y);

            _connectionControl.SetStartPosition(absolutePosition);
        }
    }
}
