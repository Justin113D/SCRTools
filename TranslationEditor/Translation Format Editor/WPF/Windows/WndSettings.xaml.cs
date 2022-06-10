using SCR.Tools.WPF.Styling;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Window = SCR.Tools.WPF.Styling.Window;

namespace SCR.Tools.TranslationEditor.FormatEditor.WPF.Windows
{
    /// <summary>
    /// Interaction logic for WndSettings.xaml
    /// </summary>
    public partial class WndSettings : Window
    {
        public WndSettings()
        {
            InitializeComponent();
            FontSizeField.Text = Properties.Settings.Default.Fontsize.ToString();
            ThemeCombobox.ItemsSource = Enum.GetValues(typeof(Theme));
            ThemeCombobox.SelectedItem = BaseStyle.Theme;
            JsonIndentingCheckbox.IsChecked = Properties.Settings.Default.JsonIndenting;
        }

        private void FontSizeField_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !double.TryParse(e.Text, out _);
        }

        private void ThemeCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count <= 0 || e.AddedItems[0] is not Theme theme)
                return;

            if (BaseStyle.Theme != theme)
                BaseStyle.Theme = theme;
        }

        private void CloseSettings(object sender, RoutedEventArgs e)
        {
            double fs = double.Parse(FontSizeField.Text);
            if (Properties.Settings.Default.Fontsize != fs)
            {
                Properties.Settings.Default.Fontsize = fs;
                ((App)Application.Current).AppFontSize = fs;
            }

            Properties.Settings.Default.JsonIndenting = JsonIndentingCheckbox.IsChecked ?? false;

            Properties.Settings.Default.Save();
            Close();
        }
    }
}
