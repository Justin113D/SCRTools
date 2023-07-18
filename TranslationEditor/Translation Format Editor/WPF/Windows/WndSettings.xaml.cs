using SCR.Tools.WPF.Theme;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SCR.Tools.TranslationEditor.FormatEditor.WPF.Windows
{
    /// <summary>
    /// Interaction logic for WndSettings.xaml
    /// </summary>
    public partial class WndSettings : ThemeWindow
    {
        private static App App
            => (App)Application.Current;

        public WndSettings()
        {
            InitializeComponent();
            FontSizeField.Text = Properties.Settings.Default.Fontsize.ToString();
            SkinCombobox.ItemsSource = Enum.GetValues<Skin>();
            SkinCombobox.SelectedItem = App.Skin;
            JsonIndentingCheckbox.IsChecked = Properties.Settings.Default.JsonIndenting;
        }

        private void FontSizeField_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !double.TryParse(e.Text, out _);
        }

        private void SkinCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count <= 0 || e.AddedItems[0] is not Skin skin)
                return;

            App.Skin = skin;
        }

        private void CloseSettings(object sender, RoutedEventArgs e)
        {
            App.AppFontSize = double.Parse(FontSizeField.Text);
            Properties.Settings.Default.JsonIndenting = JsonIndentingCheckbox.IsChecked ?? false;
            Properties.Settings.Default.Save();
            Close();
        }
    }
}
