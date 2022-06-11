using SCR.Tools.WPF.Styling;
using SCR.Tools.DialogEditor.Viewmodel;
using System;
using System.Text.RegularExpressions;
using System.Windows;

namespace SCR.Tools.DialogEditor
{

    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class WndSettings : SCR.Tools.WPF.Styling.Window
    {
        public WndSettings()
        {
            InitializeComponent();
            FontSizeField.Text = Properties.Settings.Default.Fontsize.ToString();
            ThemeCombobox.ItemsSource = Enum.GetValues(typeof(Theme));
            ThemeCombobox.SelectedItem = BaseStyle.Theme;
        }

        private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !double.TryParse(e.Text, out _);
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            double fs = double.Parse(FontSizeField.Text);
            if(Properties.Settings.Default.Fontsize != fs)
            {
                Properties.Settings.Default.Fontsize = fs;
                ((App)Application.Current).AppFontSize = fs;
            }

            Properties.Settings.Default.Save();
            Close();
        }

        private void ThemeCombobox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if(BaseStyle.Theme != (Theme)e.AddedItems[0])
                BaseStyle.Theme = (Theme)e.AddedItems[0];
        }
    }
}
