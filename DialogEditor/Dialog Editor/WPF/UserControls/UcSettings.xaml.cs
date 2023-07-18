using Microsoft.Win32;
using SCR.Tools.WPF.Controls;
using System.Windows;

namespace SCR.Tools.Dialog.Editor.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for UcSettings.xaml
    /// </summary>
    public partial class UcSettings : SettingsControl
    {
        public override double WindowWidth => 400;

        public override double WindowHeight => 250;

        public UcSettings() : base()
        {
            InitializeComponent();
            DefaultPathTextBox.Text = Properties.Settings.Default.DefaultDialogOptionsPath;
            JsonIndentingCheckbox.IsChecked = Properties.Settings.Default.JsonIndenting;
        }

        public static void OpenSettings()
        {
            SettingsWindow.OpenSettingsDialog(new UcSettings());
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

        public override void OnSettingsClose()
        {
            Properties.Settings.Default.DefaultDialogOptionsPath = DefaultPathTextBox.Text;
            Properties.Settings.Default.JsonIndenting = JsonIndentingCheckbox.IsChecked ?? false;
        }
    }
}
