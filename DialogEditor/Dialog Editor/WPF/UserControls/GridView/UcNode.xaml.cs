using SCR.Tools.DialogEditor.Viewmodeling;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Collections.Generic;
using System.ComponentModel;

namespace SCR.Tools.DialogEditor.WPF.UserControls.GridView
{
    /// <summary>
    /// Interaction logic for UcNode.xaml
    /// </summary>
    public partial class UcNode : UserControl
    {
        public UcGridView? _gridView;

        private ContentPresenter CanvasController
            => (ContentPresenter)VisualParent;

        public double CanvasX
        {
            get => Canvas.GetLeft(CanvasController);
            set
            {
                Canvas.SetLeft(CanvasController, value);
                UpdateSocketConnections();
            }
        }

        public double CanvasY
        {
            get => Canvas.GetTop(CanvasController);
            set
            {
                Canvas.SetTop(CanvasController, value);
                UpdateSocketConnections();
            }
        }


        private UcNodeOutputSocket[] _sockets = Array.Empty<UcNodeOutputSocket>();

        public List<UcNodeOutputSocket> ConnectedSockets = new();


        public Rect SelectRect
        {
            get
            {
                return new(
                    CanvasX + 5,
                    CanvasY + 5,
                    RenderSize.Width - 10,
                    RenderSize.Height - 10
                    );
            }
        }

        public VmNode Viewmodel
            => (VmNode)DataContext;


        public UcNode()
        {
            DataContextChanged += OnDataContextChanged;
            Unloaded += OnUnloaded;
            InitializeComponent();
            OutputSockets.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
        }


        #region Control Event handlers

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                // unexpected behavior
                throw new InvalidOperationException();
            }

            if (e.NewValue is INotifyPropertyChanged newINotify)
            {
                newINotify.PropertyChanged += OnPropertyChanged;

                CanvasX = UcGridView.ToGridCoordinates(Viewmodel.LocationX);
                CanvasY = UcGridView.ToGridCoordinates(Viewmodel.LocationY);
            }
        }

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(VmNode.LocationX))
            {
                CanvasX = UcGridView.ToGridCoordinates(Viewmodel.LocationX);
            }

            if (e.PropertyName == nameof(VmNode.LocationY))
            {
                CanvasY = UcGridView.ToGridCoordinates(Viewmodel.LocationY);
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Viewmodel.PropertyChanged -= OnPropertyChanged;
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);

            if (_gridView != null)
            {
                throw new InvalidOperationException();
            }

            DependencyObject element = VisualParent;
            while (element is not UcGridView)
            {
                element = VisualTreeHelper.GetParent(element);
            }
            _gridView = (UcGridView)element;
        }
        
        private void ItemContainerGenerator_StatusChanged(object? sender, EventArgs e)
        {
            if(sender is not ItemContainerGenerator itemGen)
            {
                throw new InvalidOperationException("Generating items error");
            }

            // preventing NAN
            if(CanvasX != CanvasX || CanvasY != CanvasY)
            {
                return;
            }

            if(itemGen.Status 
                is GeneratorStatus.GeneratingContainers
                or GeneratorStatus.NotStarted)
            {
                _sockets = Array.Empty<UcNodeOutputSocket>();
            }
            else if(OutputSockets.ItemContainerGenerator.Status
                == GeneratorStatus.ContainersGenerated)
            {
                _sockets = new UcNodeOutputSocket[OutputSockets.Items.Count];

                for (int i = 0; i < itemGen.Items.Count; i++)
                {
                    DependencyObject itemContainer = OutputSockets.ItemContainerGenerator.ContainerFromIndex(i);
                    itemContainer = VisualTreeHelper.GetChild(itemContainer, 0);

                    UcNodeOutputSocket socket = (UcNodeOutputSocket)VisualTreeHelper.GetChild(itemContainer, 0);

                    _sockets[i] = socket;
                    socket.RecalculateNodeOffset(this);
                    socket.SetCanvasPosition(new(CanvasX, CanvasY));
                }
            }
            else
            {
                throw new InvalidOperationException("Generating items error");
            }
        }

        #endregion

        private void UpdateSocketConnections()
        {
            Point canvasPos = new(CanvasX, CanvasY);
            foreach(UcNodeOutputSocket socket in _sockets)
            {
                socket.SetCanvasPosition(canvasPos);
            }

            foreach (UcNodeOutputSocket socket in ConnectedSockets)
            {
                socket.SetEndPosition(canvasPos);
            }
        }


        private void Select()
        {
            bool multi = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

            if (!multi && Viewmodel.Selected)
            {
                return;
            }

            Viewmodel.Select(multi, true);
        }

        public void SetDragOffset(Point offset)
        {
            CanvasX = UcGridView.ToGridCoordinates(Viewmodel.LocationX) + offset.X;
            CanvasY = UcGridView.ToGridCoordinates(Viewmodel.LocationY) + offset.Y;
        }

        public void ApplyCanvasPosition()
        {
            Viewmodel.LocationX = UcGridView.FromGridCoordinates(CanvasX);
            Viewmodel.LocationY = UcGridView.FromGridCoordinates(CanvasY);
        }

        public void ResetCanvasPosition()
        {
            CanvasX = UcGridView.ToGridCoordinates(Viewmodel.LocationX);
            CanvasY = UcGridView.ToGridCoordinates(Viewmodel.LocationY);
        }


        private void GrabGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(_gridView == null)
            {
                throw new InvalidOperationException();
            }

            if (e.ChangedButton != MouseButton.Left)
            {
                return;
            }

            _gridView.DragNodePosition = e.GetPosition(_gridView);
        }

        private void GrabGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_gridView == null)
            {
                throw new InvalidOperationException();
            }

            if (e.ChangedButton != MouseButton.Left)
            {
                return;
            }

            if (_gridView.DragNodePosition != null)
            {
                if (!_gridView.IsDraggingNodes)
                {
                    Select();
                }

                _gridView.DropSelect(false);
            }
        }

        private void GrabGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_gridView == null)
            {
                throw new InvalidOperationException();
            }

            if (_gridView.DragNodePosition != null && !_gridView.IsDraggingNodes)
            {
                Select();

                _gridView.InitMoveSelected();
            }
        }
    }
}
