using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SCR.Tools.DialogEditor.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for UcNode.xaml
    /// </summary>
    public partial class UcNode : UserControl
    {
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

        public UcNode()
        {
            InitializeComponent();
        }
   
        private static double ToGridSpace(int value)
        {
            return value * UcGridView.brushDim + UcGridView.halfBrushDim;
        }

        private static int FromGridSpace(double value)
        {
            return ((int)value - UcGridView.halfBrushDim * (value < 0 ? 2 : 0)) / UcGridView.brushDim;
        }
    }
}
