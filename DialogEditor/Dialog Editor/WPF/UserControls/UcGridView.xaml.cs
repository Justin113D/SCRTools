using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SCR.Tools.DialogEditor.WPF.UserControls
{

    /// <summary>
    /// Interaction logic for UcGridView.xaml
    /// </summary>
    public partial class UcGridView : UserControl
    {
        public const int brushDim = 50;
        public const int halfBrushDim = brushDim / 2;

        public static readonly DependencyProperty GridTileProperty =
            DependencyProperty.Register(
                nameof(GridTile),
                typeof(Brush),
                typeof(UcGridView),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((d, e) =>
                    {
                        ((Path)((UcGridView)d).GridBackground.Visual).Fill = (Brush)e.NewValue;
                    })
                )
            );

        public Brush GridTile
        {
            get => (Brush)GetValue(GridTileProperty);
            set => SetValue(GridTileProperty, value);
        }

        /// <summary>
        /// Grid matrix transform
        /// </summary>
        public MatrixTransform GridTransform { get; private set; }

        /// <summary>
        /// Background grid brush
        /// </summary>
        public VisualBrush GridBackground { get; private set; }


        /// <summary>
        /// Last relative mouse position
        /// </summary>
        private Point? _mousePos;

        /// <summary>
        /// Last grid mouse position
        /// </summary>
        private Point? _gridMousePos;

        public UcGridView()
        {
            GridTransform = new();

            GridBackground = new()
            {
                TileMode = TileMode.Tile,
                ViewportUnits = BrushMappingMode.Absolute,
                ViewboxUnits = BrushMappingMode.Absolute,
                Viewbox = new(0, 0, brushDim, brushDim),
                Viewport = new Rect(0, 0, brushDim, brushDim),
                Visual = new Path()
                {
                    Fill = GridTile,
                    Data = new RectangleGeometry(new(1, 1, brushDim - 2, brushDim - 2))
                }
            };

            InitializeComponent();
        }

        private void UpdateBackground()
        {
            double width = brushDim * GridTransform.Matrix.M11;
            GridBackground.Viewport = new Rect(GridTransform.Matrix.OffsetX, GridTransform.Matrix.OffsetY, width, width);
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
                Mouse.OverrideCursor = Cursors.SizeAll;
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            _mousePos = null;
            _gridMousePos = null;
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(this);
            Point gridMousePos = GridTransform.Inverse.Transform(mousePos);

            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                Point dif = new((mousePos.X - _mousePos?.X) ?? 0, (mousePos.Y - _mousePos?.Y) ?? 0);

                Matrix m = GridTransform.Matrix;
                m.Translate(dif.X, dif.Y);
                GridTransform.Matrix = m;

                UpdateBackground();
                Mouse.OverrideCursor = Cursors.SizeAll;
            }
            else
            {
                Mouse.OverrideCursor = null;
            }

            _mousePos = mousePos;
            _gridMousePos = gridMousePos;
        }

        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if(_gridMousePos == null)
            {
                return;
            }

            double newScale = GridTransform.Matrix.M11 + e.Delta * 0.0005d;

            newScale = Math.Clamp(newScale, 0.1, 1);

            if (newScale == GridTransform.Matrix.M11)
            {
                return;
            }

            double scaleFactor = newScale / GridTransform.Matrix.M11;

            Matrix m = GridTransform.Matrix;
            m.ScaleAtPrepend(scaleFactor, scaleFactor, _gridMousePos.Value.X, _gridMousePos.Value.Y);
            GridTransform.Matrix = m;

            UpdateBackground();
        }
    }
}
