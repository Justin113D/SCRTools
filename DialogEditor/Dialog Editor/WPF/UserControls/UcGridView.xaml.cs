﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using SCR.Tools.DialogEditor.Viewmodeling;
using System.Collections.Generic;

namespace SCR.Tools.DialogEditor.WPF.UserControls
{

    /// <summary>
    /// Interaction logic for UcGridView.xaml
    /// </summary>
    public partial class UcGridView : UserControl
    {
        /// Return Type: BOOL->int  
        ///X: int  
        ///Y: int  
        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetCursorPos(int X, int Y);

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

        public Point? NodeClickCheck { get; set; }

        private UcNode[]? _selectedNodes;

        public bool IsDragging
            => _selectedNodes != null;


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

            InitializeComponent();
        }

        private Point GetMouseGridPos(Point point)
        {
            return GridTransform.Inverse.Transform(point);
        }

        private Point GetMouseGridPos(MouseEventArgs e)
        {
            return GetMouseGridPos(e.GetPosition(this));
        }


        private void UpdateBackground()
        {
            double width = brushDim * GridTransform.Matrix.M11;
            GridBackground.Viewport = new Rect(GridTransform.Matrix.OffsetX, GridTransform.Matrix.OffsetY, width, width);
        }


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
            if(_previousMousePos == null || !active)
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


        private void DragSelectBox(Point mousePos, bool active)
        {
            if (SelectBoxStartGridPos == null || !active)
            {
                return;
            }

            Point dragToGridPos = GetMouseGridPos(mousePos);

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

                if(!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))
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


        public void InitMoveSelected()
        {
            if(NodeClickCheck == null)
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
            // converting it to grid space
            NodeClickCheck = GetMouseGridPos(NodeClickCheck ?? default);
        }

        private void MoveSelected(Point mousePos, bool active)
        {
            if(_selectedNodes == null || !active)
            {
                return;
            }

            if(NodeClickCheck == null)
            {
                throw new InvalidOperationException("Click check is null!");
            }


            Point gridMousePos = GetMouseGridPos(mousePos);

            Point dif = new(
               (gridMousePos.X - NodeClickCheck?.X) ?? 0,
               (gridMousePos.Y - NodeClickCheck?.Y) ?? 0);

            foreach(UcNode t in _selectedNodes)
            {
                t.SetDragOffset(dif);
            }
        }

        public void DropSelect(bool revert)
        {
            if(_selectedNodes == null)
            {
                NodeClickCheck = null;
                return;
            }

            if (NodeClickCheck == null)
            {
                throw new InvalidOperationException("Clickcheck or selectednodes are null!");
            }

            if(revert)
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

            NodeClickCheck = null;
            _selectedNodes = null;
        }


        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (NodeClickCheck != null)
            {
                return;
            }

            if (e.ChangedButton == MouseButton.Left)
            {
                SelectBoxStartGridPos = GetMouseGridPos(e);
                SelectBlock.Width = 0;
                SelectBlock.Height = 0;
            }
            else if(e.ChangedButton == MouseButton.Middle)
            {
                _previousMousePos = e.GetPosition(this);
                Mouse.OverrideCursor = Cursors.SizeAll;
            }
        }

        private void GridCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(_previousMousePos != null && e.ChangedButton == MouseButton.Middle)
            {
                EndPanGrid();
            }

            if(SelectBoxStartGridPos != null && e.ChangedButton == MouseButton.Left)
            {
                EndSelectBox(true);
            }

            if(_selectedNodes != null && e.ChangedButton == MouseButton.Left)
            {
                DropSelect(false);
            }
        }

        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
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

            PanGrid(mousePos, e.MiddleButton == MouseButtonState.Pressed);
            DragSelectBox(mousePos, e.LeftButton == MouseButtonState.Pressed);
            MoveSelected(mousePos, e.LeftButton == MouseButtonState.Pressed);
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
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

            if(SelectBoxStartGridPos != null)
            {
                EndSelectBox(false);
            }

            if(NodeClickCheck != null)
            {
                DropSelect(true);
            }
        }

    }
}
