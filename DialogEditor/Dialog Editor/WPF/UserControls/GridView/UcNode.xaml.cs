using SCR.Tools.DialogEditor.Viewmodeling;
using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Collections.Generic;

namespace SCR.Tools.DialogEditor.WPF.UserControls.GridView
{
    /// <summary>
    /// Interaction logic for UcNode.xaml
    /// </summary>
    public partial class UcNode : UserControl
    {
        #region Dependency Properties

        public static readonly DependencyProperty GridViewProperty =
            DependencyProperty.Register(
                nameof(GridView),
                typeof(UcGridView),
                typeof(UcNode)
            );

        public UcGridView GridView
        {
            get => (UcGridView)GetValue(GridViewProperty);
            set => SetValue(GridViewProperty, value);
        }

        public static readonly DependencyProperty LocationXProperty =
            DependencyProperty.Register(
                nameof(LocationX),
                typeof(int),
                typeof(UcNode),
                new(0, (o, args) =>
                {
                    UcNode node = (UcNode)o;
                    node.CanvasX = UcGridView.ToGridCoordinates((int)args.NewValue);
                })
            );

        public int LocationX
        {
            get => (int)GetValue(LocationXProperty);
            set => SetValue(LocationXProperty, value);
        }

        public static readonly DependencyProperty LocationYProperty =
            DependencyProperty.Register(
                nameof(LocationY),
                typeof(int),
                typeof(UcNode),
                new(0, (o, args) =>
                {
                    UcNode node = (UcNode)o;
                    node.CanvasY = UcGridView.ToGridCoordinates((int)args.NewValue);
                })
            );

        public int LocationY
        {
            get => (int)GetValue(LocationYProperty);
            set => SetValue(LocationYProperty, value);
        }

        #endregion

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
            InitializeComponent();

            OutputSockets.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
        }

        private void ItemContainerGenerator_StatusChanged(object? sender, EventArgs e)
        {
            if(sender is not ItemContainerGenerator itemGen)
            {
                throw new InvalidOperationException("Generating items error");
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
            CanvasX = UcGridView.ToGridCoordinates(LocationX) + offset.X;
            CanvasY = UcGridView.ToGridCoordinates(LocationY) + offset.Y;
        }

        public void ApplyCanvasPosition()
        {
            LocationX = UcGridView.FromGridCoordinates(CanvasX);
            LocationY = UcGridView.FromGridCoordinates(CanvasY);
        }

        public void ResetCanvasPosition()
        {
            CanvasX = UcGridView.ToGridCoordinates(LocationX);
            CanvasY = UcGridView.ToGridCoordinates(LocationY);
        }


        private void GrabGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
            {
                return;
            }

            GridView.DragNodePosition = e.GetPosition(GridView);
        }

        private void GrabGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
            {
                return;
            }

            if (GridView.DragNodePosition != null)
            {
                if (!GridView.IsDraggingNodes)
                {
                    Select();
                }

                GridView.DropSelect(false);
            }
        }

        private void GrabGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (GridView.DragNodePosition != null && !GridView.IsDraggingNodes)
            {
                Select();

                GridView.InitMoveSelected();
            }
        }
    }
}
