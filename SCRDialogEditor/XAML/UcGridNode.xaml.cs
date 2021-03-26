using SCRCommon.WpfStyles;
using SCRDialogEditor.Viewmodel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SCRDialogEditor.XAML
{
    public partial class UcGridNode : UserControl
    {
        private class OutputSocket
        {
            public FrameworkElement element;
            public Point relativePosition;
        }

        #region DependencyProperties

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
                            g.updateSockets = true;
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

        public int UpdateCounter
        {
            get => (int)GetValue(UpdateCounterProperty);
            set => SetValue(UpdateCounterProperty, value);
        }

        #endregion


        private VmNode Node => (VmNode)DataContext;

        private FrameworkElement _container;

        private Point _lastPos;

        private readonly Dictionary<VmNodeOutput, OutputSocket> outputSockets;

        private bool recalcSockets;

        private bool updateSockets;

        public UcGridNode()
        {
            outputSockets = new();
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _container = WPFExtensions.FindParent<ItemsControl>(this);
            Node.Outputs.CollectionChanged += OutputsUpdated;
            recalcSockets = true;
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

            if(newPos == _lastPos && !updateSockets && !recalcSockets)
                return;

            // recalculating the relative positions
            if(recalcSockets)
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
                recalcSockets = false;
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

            updateSockets = false;
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


        private void Socket_MouseLeave(object sender, MouseEventArgs e)
        {
            if(e.LeftButton != MouseButtonState.Pressed || Node.Grid.Connecting != null)
                return;

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

            recalcSockets = true;
        }

        private void Socket_Unloaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement s = (FrameworkElement)sender;
            var socket = outputSockets.First(x => x.Value.element == s);
            outputSockets.Remove(socket.Key);
        }

        private void OutputsUpdated(object sender, NotifyCollectionChangedEventArgs e)
        {
            recalcSockets = true;
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
