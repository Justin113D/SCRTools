using SCR.Tools.WPF.Theme;
using System;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SCR.Tools.WPF.Controls
{
    /// <summary>
    /// Interaction logic for WndBaseSettings.xaml
    /// </summary>
    public partial class SettingsWindow : ThemeWindow
    {
        private static ThemeApplication App
            => (ThemeApplication)Application.Current;

        private readonly SettingsControl? _settingsContent;

        private SettingsWindow(SettingsControl? settingsContent)
        {
            InitializeComponent();
            FontSizeField.Text = App.Settings.Fontsize.ToString();
            SkinCombobox.ItemsSource = Enum.GetValues<Skin>();
            SkinCombobox.SelectedItem = App.Skin;

            if(settingsContent == null)
            {
                SettingsContent.Visibility = Visibility.Collapsed;
            }
            else
            {
                _settingsContent = settingsContent;
                SettingsContent.Content = settingsContent;
                Width = settingsContent.WindowWidth;
                Height = settingsContent.WindowHeight;
            }
        }

        public static void OpenSettingsDialog(SettingsControl? settingsContent)
        {
            new SettingsWindow(settingsContent).ShowDialog();
        }

        private void SkinCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count <= 0 || e.AddedItems[0] is not Skin skin)
                return;

            App.Skin = skin;
        }

        private void FontSizeField_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !double.TryParse(e.Text, out _);
        }

        private void CloseSettings(object sender, RoutedEventArgs e)
        {
            App.AppFontSize = double.Parse(FontSizeField.Text);
            _settingsContent?.OnSettingsClose();
            App.Settings.Save();
            Close();
        }
    }
}
