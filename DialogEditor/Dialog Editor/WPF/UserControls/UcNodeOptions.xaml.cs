using SCR.Tools.Dialog.Editor.Viewmodeling;
using SCR.Tools.WPF;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SCR.Tools.Dialog.Editor.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for UcNodeOptions.xaml
    /// </summary>
    public partial class UcNodeOptions : UserControl
    {
        public VmNodeOptions<System.Drawing.Color> Viewmodel
            => (VmNodeOptions<System.Drawing.Color>)DataContext;

        public UcNodeOptions()
        {
            InitializeComponent();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !((TextBox)sender).AcceptsReturn)
            {
                ((TextBox)sender).MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
            }
        }

        private void ColorButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Grid grid = (Grid)button.Parent;
            Border displayBorder = (Border)grid.Children[1];

            Color color = ((SolidColorBrush)displayBorder.Background).Color;
            Color previous = color;

            ColorPicker.ShowAsDialog(ref color);

            if (!color.Equals(previous))
            {
                displayBorder.Background = new SolidColorBrush(color);
            }
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox textbox = (TextBox)sender;
            if (e.Key == Key.Enter)
            {
                Viewmodel.AddOption(textbox.Text);
                textbox.Text = "";
            }
        }
    }
}
