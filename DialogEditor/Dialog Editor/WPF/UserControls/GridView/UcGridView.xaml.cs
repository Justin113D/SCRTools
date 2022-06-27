using SCR.Tools.Dialog.Editor.Viewmodeling;
using SCR.Tools.Viewmodeling;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SCR.Tools.Dialog.Editor.WPF.UserControls.GridView
{
    public class NodeControlNotGeneratedException : Exception
    {
        public ContentPresenter Content { get; }

        public NodeControlNotGeneratedException(ContentPresenter content) : base()
        {
            Content = content;
        }
    }


    /// <summary>
    /// Interaction logic for UcGridView.xaml
    /// </summary>
    public partial class UcGridView : UserControl
    {
        public delegate void NodesGeneratedEventHandler();
        public event NodesGeneratedEventHandler? NodesGenerated;

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


        private Point? _previousMousePos;

        private Point _selectBoxStartGridPos;

        private Point? SelectBoxStartGridPos
        {
            get
            {
                if (SelectBlock.Visibility != Visibility.Visible)
                {
                    return null;
                }

                return _selectBoxStartGridPos;
            }
            set
            {
                if (value == null)
                {
                    SelectBlock.Visibility = Visibility.Collapsed;
                    return;
                }

                SelectBlock.Visibility = Visibility.Visible;
                _selectBoxStartGridPos = value ?? default;
            }
        }

        public Point? DragNodePosition { get; set; }

        private UcNode[]? _selectedNodes;

        public bool IsDraggingNodes
            => _selectedNodes != null;

        public UcNodeOutputSocket? ConnectingSocket { get; private set; }

        #region Commands

        public RelayCommand CmdRecenterView
            => new(RecenterView);

        public RelayCommand CmdAddNode
            => new(AddNodeCenter);

        public RelayCommand<VmNode> CmdFocusNode
            => new(FocusNode);

        #endregion

        public VmDialog Viewmodel
            => (VmDialog)DataContext;

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

            DataContextChanged += UcGridView_DataContextChanged;

            InitializeComponent();

            NodesDisplay.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
        }

        private void UcGridView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            NodeConnections.Items.Clear();
        }

        private void ItemContainerGenerator_StatusChanged(object? sender, EventArgs e)
        {
            if (NodesDisplay.ItemContainerGenerator.Status ==
                System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
            {
                NodesGenerated?.Invoke();
            }
        }

        private void UpdateBackground()
        {
            double width = brushDim * GridTransform.Matrix.M11;
            GridBackground.Viewport = new Rect(GridTransform.Matrix.OffsetX, GridTransform.Matrix.OffsetY, width, width);
        }

        public UcNode GetNodeControl(VmNode viewmodel)
        {
            ContentPresenter content = (ContentPresenter)NodesDisplay.ItemContainerGenerator.ContainerFromItem(viewmodel);

            if (VisualTreeHelper.GetChildrenCount(content) == 0)
            {
                throw new NodeControlNotGeneratedException(content);
            }
            else
            {
                return (UcNode)VisualTreeHelper.GetChild(content, 0);
            }
        }

        private UcNode? GetMouseOverUcNode()
        {
            for (int i = 0; i < NodesDisplay.ItemContainerGenerator.Items.Count; i++)
            {
                ContentPresenter presenter = (ContentPresenter)NodesDisplay.ItemContainerGenerator.ContainerFromIndex(i);
                if (presenter.IsMouseOver)
                {
                    return (UcNode)VisualTreeHelper.GetChild(presenter, 0);
                }
            }

            return null;
        }

        #region Space conversion

        public static double ToGridCoordinates(int value)
            => (value * brushDim) + halfBrushDim;

        public static int FromGridCoordinates(double value)
            => ((int)value - (halfBrushDim * (value < 0 ? 2 : 0))) / brushDim;

        public Point ConvertToGridSpace(Point point)
        {
            return GridTransform.Inverse.Transform(point);
        }

        public Point GetMouseGridPos(MouseEventArgs e)
        {
            return ConvertToGridSpace(e.GetPosition(this));
        }

        #endregion


        #region Grid moving

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetCursorPos(int X, int Y);

        private void WrapCursor(MouseEventArgs e)
        {
            Window window = Window.GetWindow(this);
            Point origin = window.PointToScreen(new(0, 0));

            Point relativeMousePos = e.GetPosition(this);

            Point newRelativeMousePos = e.GetPosition(window);

            if (relativeMousePos.X <= 0)
            {
                relativeMousePos.X += ActualWidth;
                newRelativeMousePos.X += ActualWidth;
            }
            else if (relativeMousePos.X >= ActualWidth)
            {
                relativeMousePos.X -= ActualWidth;
                newRelativeMousePos.X -= ActualWidth;
            }

            if (relativeMousePos.Y <= 0)
            {
                relativeMousePos.Y += ActualHeight;
                newRelativeMousePos.Y += ActualHeight;
            }
            else if (relativeMousePos.Y >= ActualHeight)
            {
                relativeMousePos.Y -= ActualHeight;
                newRelativeMousePos.Y -= ActualHeight;
            }

            _previousMousePos = relativeMousePos;

            SetCursorPos(
               (int)(origin.X + newRelativeMousePos.X),
               (int)(origin.Y + newRelativeMousePos.Y));

        }

        private void PanGrid(Point mousePos, bool active)
        {
            if (_previousMousePos == null || !active)
            {
                return;
            }

            Point dif = new(
                (mousePos.X - _previousMousePos?.X) ?? 0,
                (mousePos.Y - _previousMousePos?.Y) ?? 0);

            Matrix m = GridTransform.Matrix;
            m.Translate(dif.X, dif.Y);
            GridTransform.Matrix = m;

            UpdateBackground();

            _previousMousePos = mousePos;
        }

        private void EndPanGrid()
        {
            _previousMousePos = null;
            Mouse.OverrideCursor = null;
        }

        private void RecenterView()
        {
            if (ConnectingSocket != null)
            {
                return;
            }

            Matrix m = GridTransform.Matrix;
            m.OffsetX = 0;
            m.OffsetY = 0;
            GridTransform.Matrix = m;
            UpdateBackground();
        }

        private void FocusNode(VmNode node)
        {
            if (ConnectingSocket != null)
            {
                return;
            }

            FrameworkElement obj = (FrameworkElement)NodesDisplay.ItemContainerGenerator.ContainerFromItem(node);

            Point GridLoc = new(
                Canvas.GetLeft(obj) + obj.ActualWidth / 2,
                Canvas.GetTop(obj) + obj.ActualHeight / 2);

            Matrix m = GridTransform.Matrix;
            Point p = ConvertToGridSpace(
                new(ActualWidth / 2, ActualHeight / 2));

            m.TranslatePrepend(p.X - GridLoc.X, p.Y - GridLoc.Y);
            GridTransform.Matrix = m;

            UpdateBackground();
        }

        #endregion

        #region Drag select box

        private void DragSelectBox(Point mousePos, bool active)
        {
            if (SelectBoxStartGridPos == null || !active)
            {
                return;
            }

            Point dragToGridPos = ConvertToGridSpace(mousePos);

            double x = dragToGridPos.X < _selectBoxStartGridPos.X ? dragToGridPos.X : _selectBoxStartGridPos.X;
            double y = dragToGridPos.Y < _selectBoxStartGridPos.Y ? dragToGridPos.Y : _selectBoxStartGridPos.Y;
            double width = Math.Abs(dragToGridPos.X - _selectBoxStartGridPos.X);
            double height = Math.Abs(dragToGridPos.Y - _selectBoxStartGridPos.Y);

            Canvas.SetLeft(SelectBlock, x);
            Canvas.SetTop(SelectBlock, y);

            SelectBlock.Width = width;
            SelectBlock.Height = height;
        }

        private void EndSelectBox(bool selectContents)
        {
            if (selectContents)
            {
                Rect selectRect = new(
                    Canvas.GetLeft(SelectBlock),
                    Canvas.GetTop(SelectBlock),
                    SelectBlock.Width,
                    SelectBlock.Height);

                if (!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))
                {
                    Viewmodel.DeselectAll();
                }

                for (int i = 0; i < NodesDisplay.Items.Count; i++)
                {
                    UcNode ucNode = (UcNode)VisualTreeHelper.GetChild(
                        NodesDisplay.ItemContainerGenerator.ContainerFromIndex(i),
                        0);

                    if (selectRect.IntersectsWith(ucNode.SelectRect)
                        && !ucNode.Viewmodel.Selected)
                    {
                        ucNode.Viewmodel.Select(true, false);
                    }
                }
            }

            SelectBoxStartGridPos = null;
        }

        #endregion

        #region Node moving

        public void InitMoveSelected()
        {
            if (DragNodePosition == null)
            {
                throw new InvalidOperationException("Nodeclickcheck has to be set!");
            }

            List<UcNode> selectedNodes = new();

            for (int i = 0; i < NodesDisplay.Items.Count; i++)
            {
                UcNode ucNode = (UcNode)VisualTreeHelper.GetChild(
                    NodesDisplay.ItemContainerGenerator.ContainerFromIndex(i),
                    0);

                if (ucNode.Viewmodel.Selected)
                {
                    selectedNodes.Add(ucNode);
                }
            }

            _selectedNodes = selectedNodes.ToArray();
            DragNodePosition = ConvertToGridSpace(DragNodePosition ?? default);
        }

        private void MoveSelected(Point mousePos, bool active)
        {
            if (_selectedNodes == null || !active)
            {
                return;
            }

            if (DragNodePosition == null)
            {
                throw new InvalidOperationException("Click check is null!");
            }


            Point gridMousePos = ConvertToGridSpace(mousePos);

            Point dif = new(
               (gridMousePos.X - DragNodePosition?.X) ?? 0,
               (gridMousePos.Y - DragNodePosition?.Y) ?? 0);

            foreach (UcNode t in _selectedNodes)
            {
                t.SetDragOffset(dif);
            }
        }

        public void DropSelect(bool revert)
        {
            if (_selectedNodes == null)
            {
                DragNodePosition = null;
                return;
            }

            if (DragNodePosition == null)
            {
                throw new InvalidOperationException("Clickcheck or selectednodes are null!");
            }

            if (revert)
            {
                foreach (UcNode node in _selectedNodes)
                {
                    node.ResetCanvasPosition();
                }
            }
            else
            {

                UndoRedo.GlobalChangeTrackerC.BeginChangeGroup();

                foreach (UcNode node in _selectedNodes)
                {
                    node.ApplyCanvasPosition();
                }

                UndoRedo.GlobalChangeTrackerC.EndChangeGroup();

            }

            DragNodePosition = null;
            _selectedNodes = null;
        }

        #endregion

        #region Node manipulation

        private void CreateNode(Point gridPos)
        {
            if (ConnectingSocket != null)
            {
                return;
            }

            int locationX = FromGridCoordinates(gridPos.X);
            int locationY = FromGridCoordinates(gridPos.Y);

            Viewmodel.CreateNode(locationX, locationY);
        }

        private void AddNodeCenter()
        {
            Point p = ConvertToGridSpace(new(
                ActualWidth / 2, ActualHeight / 2));
            CreateNode(p);
        }

        #endregion

        #region Node connection dragging

        public void SetDraggedConnection(UcNodeOutputSocket? socket)
        {
            ConnectingSocket = socket;
        }

        private void MoveDraggingConnection(Point mousePos)
        {
            UcNode? ucNode = GetMouseOverUcNode();
            if (ucNode != null)
            {
                ConnectingSocket?.SetEndPosition(new(ucNode.CanvasX, ucNode.CanvasY));
            }
            else
            {
                Point gridMousePos = ConvertToGridSpace(mousePos);
                ConnectingSocket?.SetEndPosition(gridMousePos, false);
            }
        }

        private void DropDraggingConnection()
        {
            UcNode? ucNode = GetMouseOverUcNode();
            if (ucNode != null)
            {
                ConnectingSocket?.DropConnection(ucNode.Viewmodel);
            }
            else
            {
                ConnectingSocket?.DropConnection(null);
            }
        }

        #endregion

        #region events

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // needed for being able to
            // interact with input bindings
            Focus();

            if (DragNodePosition != null || ConnectingSocket != null)
            {
                return;
            }

            if (e.ChangedButton == MouseButton.Left)
            {
                SelectBoxStartGridPos = GetMouseGridPos(e);
                SelectBlock.Width = 0;
                SelectBlock.Height = 0;
            }
            else if (e.ChangedButton == MouseButton.Middle)
            {
                _previousMousePos = e.GetPosition(this);
                Mouse.OverrideCursor = Cursors.SizeAll;
            }
        }

        private void GridCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (ConnectingSocket != null && e.ChangedButton == MouseButton.Left)
            {
                DropDraggingConnection();
            }

            if (_previousMousePos != null && e.ChangedButton == MouseButton.Middle)
            {
                EndPanGrid();
            }

            if (SelectBoxStartGridPos != null && e.ChangedButton == MouseButton.Left)
            {
                EndSelectBox(true);
            }

            if (_selectedNodes != null && e.ChangedButton == MouseButton.Left)
            {
                DropSelect(false);
            }
        }

        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (ConnectingSocket != null || DragNodePosition != null)
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

            Point gridMousePos = GetMouseGridPos(e);

            Matrix m = GridTransform.Matrix;
            m.ScaleAtPrepend(scaleFactor, scaleFactor, gridMousePos.X, gridMousePos.Y);
            GridTransform.Matrix = m;

            UpdateBackground();
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(this);

            if (ConnectingSocket == null)
            {
                PanGrid(mousePos, e.MiddleButton == MouseButtonState.Pressed);
                DragSelectBox(mousePos, e.LeftButton == MouseButtonState.Pressed);
                MoveSelected(mousePos, e.LeftButton == MouseButtonState.Pressed);
            }
            else
            {
                MoveDraggingConnection(mousePos);
            }

        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (ConnectingSocket != null)
            {
                ConnectingSocket.DropConnection(null);
            }

            if (_previousMousePos != null)
            {
                if (e.MiddleButton != MouseButtonState.Pressed)
                {
                    EndPanGrid();
                }
                else
                {
                    WrapCursor(e);
                    return;
                }
            }

            if (SelectBoxStartGridPos != null)
            {
                EndSelectBox(false);
            }

            if (DragNodePosition != null)
            {
                DropSelect(true);
            }
        }

        private void IB_CreateNode(object sender, object e)
        {
            Point nodePos = ConvertToGridSpace(Mouse.GetPosition(this));
            CreateNode(nodePos);
        }

        #endregion
    }
}
