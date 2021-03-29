using SCRCommon.Viewmodels;
using SCRDialogEditor.Viewmodel;
using System;
using System.Collections.Generic;
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
                nameof(GridTile),
                typeof(Brush),
                typeof(UcGridEditor),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((d, e) =>
                    {
                        ((Path)((UcGridEditor)d).GridBackground.Visual).Fill = (Brush)e.NewValue;
                    })
                )
            );


        public Brush GridTile
        {
            get => (Brush)GetValue(GridTileProperty);
            set => SetValue(GridTileProperty, value);
        }

        public RelayCommand Cmd_RecenterView
            => new(RecenterView);

        public RelayCommand Cmd_AddNode
            => new(AddNodeCenter);

        private VmGrid Grid => (VmGrid)DataContext;

        /// <summary>
        /// Grid matrix transform
        /// </summary>
        public MatrixTransform GridTransform { get; private set; }

        /// <summary>
        /// Background grid brush
        /// </summary>
        public VisualBrush GridBackground { get; private set; }

        /// <summary>
        /// Last mouse position
        /// </summary>
        private Point? _mousePos;

        /// <summary>
        /// Last grid mouse position
        /// </summary>
        private Point? _gridMousePos;

        /// <summary>
        /// Dragging difference (drag initialized)
        /// </summary>
        private Point? _dragDif;

        /// <summary>
        /// where a select box was started
        /// </summary>
        private Point _SelectBoxStart;

        /// <summary>
        /// Whether the dragging process is running
        /// </summary>
        private bool _dragging = false;

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

        private void GridMouseUp(object sender, MouseButtonEventArgs e)
        {
            switch(e.ChangedButton)
            {
                case MouseButton.Middle:
                    Mouse.OverrideCursor = null;
                    break;
                case MouseButton.Left:
                    _dragDif = null;
                    _dragging = false;
                    if(SelectBlock.Visibility == Visibility.Visible)
                        SelectBoxFinish();
                    else
                        Grid.LetGo();
                    break;
            }
        }

        private void GridMouseLeave(object sender, MouseEventArgs e)
        {
            _mousePos = null;
            _gridMousePos = null;
            _dragDif = null;
            _dragging = false;
            SelectBlock.Visibility = Visibility.Collapsed;
            Mouse.OverrideCursor = null;
            Grid.LetGo();
        }

        private void GridMouseDown(object sender, MouseButtonEventArgs e)
        {
            Focus();
            if(e.ChangedButton == MouseButton.Middle)
                Mouse.OverrideCursor = Cursors.SizeAll;
        }

        private void GridMouseMove(object sender, MouseEventArgs e)
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
                if(Grid.Connecting != null)
                    Grid.MoveConnection(gt);
                else
                {
                    Point gdif = new((gt.X - _gridMousePos?.X) ?? 0, (gt.Y - _gridMousePos?.Y) ?? 0);

                    if(!_dragging)
                    {
                        _dragDif = _dragDif == null ? gdif : new(_dragDif.Value.X + gdif.X, _dragDif.Value.Y + gdif.Y);
                        if(Length(_dragDif.Value) > 5)
                        {
                            if(Grid.Grabbed != null)
                                Grid.MoveGrabbed(_dragDif.Value);
                            else if(!NodesDisplay.IsMouseOver)
                            {
                                SelectBlock.Visibility = Visibility.Visible;
                                _SelectBoxStart = new Point(
                                    gt.X - _dragDif.Value.X,
                                    gt.Y - _dragDif.Value.Y
                                    );
                                SetSelectBox(gt);
                            }

                            _dragging = true;
                        }
                    }
                    else
                    {
                        if(Grid.Grabbed != null)
                            Grid.MoveGrabbed(gdif);
                        else if(SelectBlock.Visibility == Visibility.Visible)
                            SetSelectBox(gt);
                    }
                }
            }

            _mousePos = t;
            _gridMousePos = gt;
        }

        private void GridMouseWheel(object sender, MouseWheelEventArgs e)
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

        private void AddNodeCenter()
        {
            double posX = ActualWidth / 2;
            double posY = ActualHeight / 2;
            Point p = GridTransform.Inverse.Transform(new Point(posX, posY));
            Grid.CreateNode(p);

        }

        private void UpdateBackground()
        {
            double width = brushDim * GridTransform.Matrix.M11;
            GridBackground.Viewport = new Rect(GridTransform.Matrix.OffsetX, GridTransform.Matrix.OffsetY, width, width);
        }

        public static Point FromGridSpace(int x, int y)
        {
            return new(
                x * brushDim + halfBrushDim,
                y * brushDim + halfBrushDim
                );
        }

        public static Point ToGridSpace(Point TransformSpace)
        {
            return new(
               ((int)TransformSpace.X - halfBrushDim * (TransformSpace.X < 0 ? 2 : 0)) / brushDim,
               ((int)TransformSpace.Y - halfBrushDim * (TransformSpace.Y < 0 ? 2 : 0)) / brushDim
           );
        }

        private static float Length(Point point)
        {
            return (float)Math.Sqrt(point.X * point.X + point.Y * point.Y);
        }

        private void SetSelectBox(Point end)
        {
            double x = end.X < _SelectBoxStart.X ? end.X : _SelectBoxStart.X;
            double y = end.Y < _SelectBoxStart.Y ? end.Y : _SelectBoxStart.Y;
            double width = Math.Abs(end.X - _SelectBoxStart.X);
            double height = Math.Abs(end.Y - _SelectBoxStart.Y);

            Canvas.SetLeft(SelectBlock, x);
            Canvas.SetTop(SelectBlock, y);

            SelectBlock.Width = width;
            SelectBlock.Height = height;
        }

        private void SelectBoxFinish()
        {
            Rect selectRect = new(
                Canvas.GetLeft(SelectBlock), 
                Canvas.GetTop(SelectBlock), 
                SelectBlock.Width, 
                SelectBlock.Height);

            List<VmNode> selected = new();

            for(int i = 0; i < NodesDisplay.Items.Count; i++)
            {
                UcGridNode ucgn = (UcGridNode)VisualTreeHelper.GetChild(NodesDisplay.ItemContainerGenerator.ContainerFromIndex(i), 0);
                if(selectRect.IntersectsWith(ucgn.SelectRect))
                    selected.Add(ucgn.Node);
            }

            Grid.SelectMultiple(selected.ToArray(), Keyboard.Modifiers == ModifierKeys.Shift);

            SelectBlock.Visibility = Visibility.Collapsed;
        }
    }
}
