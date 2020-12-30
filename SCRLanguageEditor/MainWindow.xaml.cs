using SCRLanguageEditor.Viewmodel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SCRLanguageEditor
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new VM_Main();
            InitializeComponent();

            DragDrop.AddQueryContinueDragHandler(this, new QueryContinueDragEventHandler((o, a) =>
            {
                ((VM_Main)DataContext).Dragging = a.Action == DragAction.Continue && a.KeyStates.HasFlag(DragDropKeyStates.LeftMouseButton) && !a.EscapePressed;
            }));

        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter && !((TextBox)sender).AcceptsReturn)
            {
                ((TextBox)sender).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

    }
}
