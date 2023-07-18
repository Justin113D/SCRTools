using Microsoft.Win32;
using SCR.Tools.WPF.Controls;
using System.Windows;

namespace SCR.Tools.TranslationEditor.ProjectEditor.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for UcSettings.xaml
    /// </summary>
    public partial class UcSettings : SettingsControl
    {
        public override double WindowWidth => 500;

        public override double WindowHeight => 280;

        private UcSettings()
        {
            InitializeComponent();
            DefaultPathTextBox.Text = Properties.Settings.Default.DefaultFormatPath;
        }

        public static void OpenSettings()
        {
            SettingsWindow.OpenSettingsDialog(new UcSettings());
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

        public override void OnSettingsClose()
        {
            Properties.Settings.Default.DefaultFormatPath = DefaultPathTextBox.Text;
        }
    }
}
