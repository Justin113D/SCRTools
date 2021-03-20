using SCRCommon.Viewmodels;
using SCRCommon.WpfStyles;
using SCRDialogEditor.Viewmodel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SCRDialogEditor.XAML
{
    /// <summary>
    /// Interaction logic for UcGridEditor.xaml
    /// </summary>
    public partial class UcGridEditor : UserControl
    {
        /// <summary>
        /// Grid background cell width
        /// </summary>
        public const int brushDim = 50;

        /// <summary>
        /// Half the width of a background cell
        /// </summary>
        public const int halfBrushDim = brushDim / 2;


        public static readonly DependencyProperty GridTileProperty =
            DependencyProperty.Register(
                "GridTile",
                typeof(Brush),
                typeof(UcGridEditor),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((d, e) => {
                        ((Path)((UcGridEditor)d).GridBackground.Visual).Fill = (Brush)e.NewValue;
                    })
                )
            );


        public Brush GridTile
        {
            get => (Brush)GetValue(GridTileProperty);
            set => SetValue(GridTileProperty, value);
        }

        public static readonly DependencyProperty GridLineProperty =
            DependencyProperty.Register(
                "GridLine",
                typeof(Brush),
                typeof(UcGridEditor),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((d, e) => { })
                )
            );

        public Brush GridLine
        {
            get => (Brush)GetValue(GridLineProperty);
            set => SetValue(GridLineProperty, value);
        }

        public RelayCommand Cmd_RecenterView
            => new(RecenterView);


        private VmGrid Grid => (VmGrid)DataContext;

        public MatrixTransform GridTransform { get; private set; }

        /// <summary>
        /// Background grid brush
        /// </summary>
        public VisualBrush GridBackground { get; private set; }

        private Point? _mousePos;

        private Point? _gridMousePos;

        public UcGridEditor()
        {

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

            GridTransform = new();
            InitializeComponent();
        }


        /// <summary>
        /// Recenters the view
        /// </summary>
        private void RecenterView()
        {
            Matrix m = GridTransform.Matrix;
            m.OffsetX = 0;
            m.OffsetY = 0;
            GridTransform.Matrix = m;
            UpdateBackground();
        }

        private void GridBorder_MouseUp(object sender, MouseButtonEventArgs e)
        {
            switch(e.ChangedButton)
            {
                case MouseButton.Middle:
                    Mouse.OverrideCursor = null;
                    break;
                case MouseButton.Left:
                    Grid.LetGo();
                    break;
            }
        }

        private void GridBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            _mousePos = null;
            _gridMousePos = null;
            Mouse.OverrideCursor = null;
            Grid.LetGo();
        }

        private void GridBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Focus();
            if(e.ChangedButton == MouseButton.Middle)
                Mouse.OverrideCursor = Cursors.SizeAll;
        }

        private void GridBorder_MouseMove(object sender, MouseEventArgs e)
        {
            Point t = e.GetPosition(this);
            Point gt = GridTransform.Inverse.Transform(t);

            if(e.MiddleButton == MouseButtonState.Pressed)
            {
                Point dif = new((t.X - _mousePos?.X) ?? 0, (t.Y - _mousePos?.Y) ?? 0);

                Matrix m = GridTransform.Matrix;
                m.Translate(dif.X, dif.Y);
                GridTransform.Matrix = m;

                UpdateBackground();
            }


            if(e.LeftButton == MouseButtonState.Pressed)
            {
                Point gdif = new((gt.X - _gridMousePos?.X) ?? 0, (gt.Y - _gridMousePos?.Y) ?? 0);
                Grid.MoveGrabbed(gdif, gt);
            }

            _mousePos = t;
            _gridMousePos = gt;
        }

        private void GridBorder_MouseWheel(object sender, MouseWheelEventArgs e)
        {
           double newScale = GridTransform.Matrix.M11 + e.Delta * 0.0005d;

            if(newScale < 0.1d)
                newScale = 0.1d;
            else if(newScale > 1)
                newScale = 1;

            if(newScale == GridTransform.Matrix.M11)
                return;

            double scaleFactor = newScale / GridTransform.Matrix.M11;

            Matrix m = GridTransform.Matrix;
            m.ScaleAtPrepend(scaleFactor, scaleFactor, _gridMousePos.Value.X, _gridMousePos.Value.Y);
            GridTransform.Matrix = m;

            UpdateBackground();
        }

        private void EventCommand_AddNode(object sender, object e)
        {
            if(_gridMousePos == null)
                return;
            Grid.CreateNode(_gridMousePos.Value);
        }

        private void UpdateBackground()
        {
            double width = brushDim * GridTransform.Matrix.M11;
            GridBackground.Viewport = new Rect(GridTransform.Matrix.OffsetX, GridTransform.Matrix.OffsetY, width, width);
        }

        public static Point FromGridSpace(Point GridSpace)
        {
            return new (
                GridSpace.X * brushDim + halfBrushDim,
                GridSpace.Y * brushDim + halfBrushDim
                );
        }

        public static Point ToGridSpace(Point TransformSpace)
        {
             return new(
                ((int)TransformSpace.X - halfBrushDim * (TransformSpace.X < 0 ? 2 : 0)) / brushDim,
                ((int)TransformSpace.Y - halfBrushDim * (TransformSpace.Y < 0 ? 2 : 0)) / brushDim
            );
        }


    }
}
