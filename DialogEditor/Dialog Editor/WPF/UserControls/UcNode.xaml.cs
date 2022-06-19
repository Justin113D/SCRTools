using SCR.Tools.DialogEditor.Viewmodeling;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SCR.Tools.DialogEditor.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for UcNode.xaml
    /// </summary>
    public partial class UcNode : UserControl
    {
        private bool _dragging;

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
                    node.CanvasX = ToGridSpace((int)args.NewValue);
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
                    node.CanvasY = ToGridSpace((int)args.NewValue);
                })
            );

        public int LocationY
        {
            get => (int)GetValue(LocationYProperty);
            set => SetValue(LocationYProperty, value);
        }

        private ContentPresenter CanvasController
            => (ContentPresenter)VisualParent;

        private double CanvasX
        {
            get => Canvas.GetLeft(CanvasController);
            set => Canvas.SetLeft(CanvasController, value);
        }

        private double CanvasY
        {
            get => Canvas.GetTop(CanvasController);
            set => Canvas.SetTop(CanvasController, value);
        }


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
        }
   
        public static double ToGridSpace(int value)
        {
            return value * UcGridView.brushDim + UcGridView.halfBrushDim;
        }

        public static int FromGridSpace(double value)
        {
            return ((int)value - UcGridView.halfBrushDim * (value < 0 ? 2 : 0)) / UcGridView.brushDim;
        }

        private void Select()
        {
            bool multi = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
            
            if(!multi && Viewmodel.Selected)
            {
                return;
            }

            Viewmodel.Select(multi, true);
        }

        public void SetDragOffset(Point offset)
        {
            CanvasX = ToGridSpace(LocationX) + offset.X;
            CanvasY = ToGridSpace(LocationY) + offset.Y;
        }

        public void ApplyCanvasPosition()
        {
            LocationX = FromGridSpace(CanvasX);
            LocationY = FromGridSpace(CanvasY);
        }

        public void ResetCanvasPosition()
        {
            CanvasX = ToGridSpace(LocationX);
            CanvasY = ToGridSpace(LocationY);
        }


        private void GrabGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton != MouseButton.Left)
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
                if(!GridView.IsDraggingNodes)
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
