using SCR.Tools.WPF;
using SCR.Tools.DialogEditor.Viewmodel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SCR.Tools.DialogEditor.XAML
{
    public partial class UcGridNode : UserControl
    {
        private class OutputSocket
        {
            public FrameworkElement element;
            public Point relativePosition;
        }

        #region DependencyProperties

        /// <summary>
        /// The amount of connections attached to the node; a change forces a position update
        /// </summary>
        public static readonly DependencyProperty UpdateCounterProperty =
            DependencyProperty.Register(
                "UpdateCounter",
                typeof(int),
                typeof(UcGridNode),
                new FrameworkPropertyMetadata(
                    0,
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    new PropertyChangedCallback((d, e) =>
                    {

                        UcGridNode g = (UcGridNode)d;

                        if(!g.IsLoaded)
                            return;

                        if((int)e.NewValue != 0)
                        {
                            g._updateSockets = true;
                            g.OnLayoutUpdated(null, null);
                        }

                        if(e.NewValue == e.OldValue)
                            return;

                        if((int)e.OldValue == 0)
                            g.LayoutUpdated += g.OnLayoutUpdated;
                        else if((int)e.NewValue == 0)
                            g.LayoutUpdated -= g.OnLayoutUpdated;

                    })
                )
            );

        /// <summary>
        /// Wrapper property for <see cref="UpdateCounterProperty"/>
        /// </summary>
        public int UpdateCounter
        {
            get => (int)GetValue(UpdateCounterProperty);
            set => SetValue(UpdateCounterProperty, value);
        }

        #endregion

        /// <summary>
        /// Node datacontext
        /// </summary>
        public VmNode Node => (VmNode)DataContext;

        /// <summary>
        /// List container parent of this control
        /// </summary>
        private FrameworkElement _container;

        /// <summary>
        /// Last grid position
        /// </summary>
        private Point _lastPos;

        /// <summary>
        /// Output viewmodel to socket elements
        /// </summary>
        private readonly Dictionary<VmNodeOutput, OutputSocket> outputSockets;

        /// <summary>
        /// Recalcuate the socket offsets to the node on the next layout update
        /// </summary>
        private bool _recalcSockets;

        /// <summary>
        /// Update the socket connection positions
        /// </summary>
        private bool _updateSockets;

        /// <summary>
        /// Indicates whether the socket was pressed (and thus a connection can start
        /// </summary>
        private bool _startConnection;

        public Rect SelectRect
        {
            get
            {
                return new(
                    Node.Position.X + 5,
                    Node.Position.Y + 5,
                    RenderSize.Width - 10,
                    RenderSize.Height - 10
                    );
            }
        }

        public UcGridNode()
        {
            outputSockets = new();
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _container = WPFExtensions.FindParent<ItemsControl>(this);
            ((INotifyCollectionChanged)Node.Outputs).CollectionChanged += OutputsUpdated;
            _recalcSockets = true;
            if(Node.UpdatePositionCounter > 0)
                LayoutUpdated += OnLayoutUpdated;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if(Node.UpdatePositionCounter > 0)
                LayoutUpdated -= OnLayoutUpdated;
        }

        private void OnLayoutUpdated(object o, EventArgs e)
        {
            if(!IsDescendantOf(_container))
                return;

            Point newPos = TransformToAncestor(_container).Transform(default);

            if(newPos == _lastPos && !_updateSockets && !_recalcSockets)
                return;

            // recalculating the relative positions
            if(_recalcSockets)
            {
                foreach(var no in Node.Outputs)
                {
                    if(!outputSockets.TryGetValue(no, out OutputSocket socket))
                        continue;
                    socket.relativePosition = socket.element.TransformToAncestor(this).Transform(default);
                    double centerOffset = socket.element.ActualWidth / 2;
                    socket.relativePosition.X += centerOffset;
                    socket.relativePosition.Y += centerOffset;
                }
                _recalcSockets = false;
            }

            _lastPos = newPos;

            // updating the socket positions
            foreach(var no in Node.Outputs)
            {
                if(!outputSockets.TryGetValue(no, out OutputSocket socket))
                    continue;
                no.UpdateStartPosition(new(
                    newPos.X + socket.relativePosition.X,
                    newPos.Y + socket.relativePosition.Y
                    ));
            }

            // Updating the input socket position
            newPos = new Point(
                _lastPos.X,
                _lastPos.Y + 43
                );

            foreach(var no in Node.Inputs)
                no.UpdateEndPosition(newPos);

            _updateSockets = false;
        }


        public void Grab_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left)
                Node.Grid.Grab(Node, Keyboard.Modifiers == ModifierKeys.Shift);
        }

        public void Grab_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left)
                Node.Grid.Grab(null, default);
        }

        private void Socket_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left)
                _startConnection = false;
        }

        private void Socket_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left)
                _startConnection = true;
        }

        private void Socket_MouseLeave(object sender, MouseEventArgs e)
        {
            if(e.LeftButton != MouseButtonState.Pressed || !_startConnection || Node.Grid.Connecting != null)
                return;

            _startConnection = false;
            VmNodeOutput vmno = (VmNodeOutput)((FrameworkElement)sender).DataContext;
            Node.Grid.Connecting = vmno;
        }

        private void Socket_Loaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement s = (FrameworkElement)sender;
            outputSockets.Add((VmNodeOutput)s.DataContext, new OutputSocket()
            {
                element = s
            });

            _recalcSockets = true;
        }

        private void Socket_Unloaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement s = (FrameworkElement)sender;
            var socket = outputSockets.First(x => x.Value.element == s);
            outputSockets.Remove(socket.Key);
        }

        private void OutputsUpdated(object sender, NotifyCollectionChangedEventArgs e)
        {
            _recalcSockets = true;
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            VmGrid grid = Node.Grid;
            if(grid.Connecting == null)
                return;

            grid.Connecting.VmOutput = Node;
            grid.Connecting = null;
        }


    }
}
