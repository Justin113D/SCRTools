using SCRDialogEditor.Viewmodel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SCRDialogEditor.XAML
{
    /// <summary>
    /// Interaction logic for UcGridEditor.xaml
    /// </summary>
    public partial class UcGridEditor : UserControl
    {

        private VmGrid Grid => (VmGrid)DataContext;

        private Point? _mousePos;

        private Point? _gridMousePos;

        public UcGridEditor()
        {
            InitializeComponent();
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
            Point gt = Grid.Transform.Inverse.Transform(t);

            if(e.MiddleButton == MouseButtonState.Pressed)
            {
                Point dif = new((t.X - _mousePos?.X) ?? 0, (t.Y - _mousePos?.Y) ?? 0);
                Grid.MoveGrid(dif);
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
           double newScale = Grid.Transform.Matrix.M11 + e.Delta * 0.0005d;

            if(newScale < 0.1d)
                newScale = 0.1d;
            else if(newScale > 1)
                newScale = 1;

            if(newScale == Grid.Transform.Matrix.M11)
                return;

            double scaleFactor = newScale / Grid.Transform.Matrix.M11;

            Matrix m = Grid.Transform.Matrix;
            m.ScaleAtPrepend(scaleFactor, scaleFactor, _gridMousePos.Value.X, _gridMousePos.Value.Y);
            Grid.Transform.Matrix = m;

            Grid.UpdateBackground();
        }

        private void EventCommand_AddNode(object sender, object e)
        {
            if(_mousePos == null)
                return;

            int posX = (int)(_mousePos?.X - Grid.Transform.Matrix.OffsetX);
            posX += VmGrid.halfBrushDim * (posX < 0 ? -1 : 1);

            int posY = (int)(_mousePos?.Y - Grid.Transform.Matrix.OffsetY);
            posY += VmGrid.halfBrushDim * (posY < 0 ? -1 : 1);

            Grid.CreateNode(new Point(posX, posY));
        }


    }
}
