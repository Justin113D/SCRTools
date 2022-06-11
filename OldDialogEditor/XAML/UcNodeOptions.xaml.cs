using System.Windows.Controls;
using System.Windows.Input;

namespace SCR.Tools.DialogEditor.XAML
{
    /// <summary>
    /// Interaction logic for NodeOptions.xaml
    /// </summary>
    public partial class UcNodeOptions : UserControl
    {
        public UcNodeOptions()
        {
            InitializeComponent();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter && !((TextBox)sender).AcceptsReturn)
            {
                ((TextBox)sender).MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
            }
        }
    }
}
