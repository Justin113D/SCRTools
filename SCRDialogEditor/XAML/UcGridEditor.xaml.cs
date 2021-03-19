using SCRDialogEditor.Viewmodel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SCRDialogEditor.XAML
{
    /// <summary>
    /// Interaction logic for UcGridEditor.xaml
    /// </summary>
    public partial class UcGridEditor : UserControl
    {

        private VmGrid Grid => (VmGrid)DataContext;

        private Point? _mousePos;

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
            Point dif = new((t.X - _mousePos?.X) ?? 0, (t.Y - _mousePos?.Y) ?? 0);

            if(e.MiddleButton == MouseButtonState.Pressed)
                Grid.MoveGrid(dif);

            if(e.LeftButton == MouseButtonState.Pressed)
                Grid.MoveGrabbed(dif, t);

            _mousePos = t;
        }

        private void EventCommand_AddNode(object sender, object e)
        {
            if(_mousePos == null)
                return;

            int posX = (int)(_mousePos?.X - Grid.Position.X);
            posX += VmGrid.halfBrushDim * (posX < 0 ? -1 : 1);

            int posY = (int)(_mousePos?.Y - Grid.Position.Y);
            posY += VmGrid.halfBrushDim * (posY < 0 ? -1 : 1);

            Grid.CreateNode(new Point(posX, posY));
        }

    }
}
