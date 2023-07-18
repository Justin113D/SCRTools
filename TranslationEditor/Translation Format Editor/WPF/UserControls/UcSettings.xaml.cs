using SCR.Tools.WPF.Controls;

namespace SCR.Tools.TranslationEditor.FormatEditor.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for UcSettings.xaml
    /// </summary>
    public partial class UcSettings : SettingsControl
    {
        public override double WindowWidth => 400;

        public override double WindowHeight => 210;

        private UcSettings()
        {
            InitializeComponent();
            JsonIndentingCheckbox.IsChecked = Properties.Settings.Default.JsonIndenting;
        }

        public static void OpenSettings()
        {
            SettingsWindow.OpenSettingsDialog(new UcSettings());
        }

        public override void OnSettingsClose()
        {
            Properties.Settings.Default.JsonIndenting = JsonIndentingCheckbox.IsChecked ?? false;
        }
    }
}
