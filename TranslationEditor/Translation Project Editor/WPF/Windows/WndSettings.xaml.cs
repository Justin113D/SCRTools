using Microsoft.Win32;
using SCR.Tools.WPF.Theme;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SCR.Tools.TranslationEditor.ProjectEditor.WPF.Windows
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
            DefaultPathTextBox.Text = Properties.Settings.Default.DefaultFormatPath;
            SkinCombobox.ItemsSource = Enum.GetValues<Skin>();
            SkinCombobox.SelectedItem = App.Skin;
        }

        private void FontSizeField_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !double.TryParse(e.Text, out _);
        }

        private void CloseSettings(object sender, RoutedEventArgs e)
        {
            double fs = double.Parse(FontSizeField.Text);
            if (Properties.Settings.Default.Fontsize != fs)
            {
                Properties.Settings.Default.Fontsize = fs;
                App.AppFontSize = fs;
            }

            Properties.Settings.Default.DefaultFormatPath = DefaultPathTextBox.Text;

            Properties.Settings.Default.Save();
            Close();
        }

        private void SkinCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count <= 0 || e.AddedItems[0] is not Skin skin)
                return;

            if (App.Skin != skin)
                App.Skin = skin;
        }

        private void DefaultPathDialog(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new()
            {
                Title = "Select format file",
                Filter = "JSON file (*.json)|*.json"
            };

            if (ofd.ShowDialog() == true)
            {
                DefaultPathTextBox.Text = ofd.FileName;
            }
        }
    }
}
