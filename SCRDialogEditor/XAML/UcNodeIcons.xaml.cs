using System.Windows.Controls;
using System.Windows.Input;

namespace SCRDialogEditor.XAML
{
    /// <summary>
    /// Interaction logic for UcNodeIcons.xaml
    /// </summary>
    public partial class UcNodeIcons : UserControl
    {
        public UcNodeIcons()
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
