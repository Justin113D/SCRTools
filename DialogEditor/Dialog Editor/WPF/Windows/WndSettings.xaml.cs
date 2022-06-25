using Microsoft.Win32;
using SCR.Tools.WPF.Styling;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Window = SCR.Tools.WPF.Styling.Window;

namespace SCR.Tools.DialogEditor.WPF.Windows
{
    /// <summary>
    /// Interaction logic for WndSettings.xaml
    /// </summary>
    public partial class WndSettings : Window
    {
        public WndSettings()
        {
            InitializeComponent();
            ThemeCombobox.ItemsSource = Enum.GetValues(typeof(Theme));
            ThemeCombobox.SelectedItem = BaseStyle.Theme;
            DefaultPathTextBox.Text = Properties.Settings.Default.DefaultDialogOptionsPath;
            JsonIndentingCheckbox.IsChecked = Properties.Settings.Default.JsonIndenting;
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
            Properties.Settings.Default.DefaultDialogOptionsPath = DefaultPathTextBox.Text;
            Properties.Settings.Default.JsonIndenting = JsonIndentingCheckbox.IsChecked ?? false;

            Properties.Settings.Default.Save();
            Close();
        }

        private void DefaultPathDialog(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new()
            {
                Title = "Select dialog options file",
                Filter = "JSON file (*.json)|*.json"
            };

            if (ofd.ShowDialog() == true)
            {
                DefaultPathTextBox.Text = ofd.FileName;
            }
        }
    }
}
