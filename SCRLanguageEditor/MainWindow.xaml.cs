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
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                ((TextBox)sender).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }
    }
}
