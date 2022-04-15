using SCRTranslationEditor.Viewmodel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SCRTranslationEditor
{
    public partial class MainWindow : Window
    {
        private TextBox _lastInteracted;

        public MainWindow()
        {
            VM_Main viewmodel = new VM_Main();
            DataContext = viewmodel;
            InitializeComponent();

            DragDrop.AddQueryContinueDragHandler(this, new QueryContinueDragEventHandler((o, a) =>
            {
                viewmodel.Dragging = a.Action == DragAction.Continue && a.KeyStates.HasFlag(DragDropKeyStates.LeftMouseButton) && !a.EscapePressed;
            }));
            viewmodel.ShowWaitCursor = DisplayWaitCursor;
            viewmodel.UpdateTextBox = UpdateTextBox;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            _lastInteracted = (TextBox)sender;
            if(e.Key == Key.Enter && !((TextBox)sender).AcceptsReturn)
            {
                ((TextBox)sender).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        private void UpdateTextBox()
        {
            if(_lastInteracted?.IsFocused == true)
                _lastInteracted.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }

        public void DisplayWaitCursor()
        {
            Mouse.SetCursor(Cursors.Wait);
            LayoutUpdated += ResetMouse;
        }

        private void ResetMouse(object o, System.EventArgs a)
        {
            Mouse.SetCursor(Cursors.Arrow);
            LayoutUpdated -= ResetMouse;
        }
    }
}
