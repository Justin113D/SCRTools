using SCRTranslationEditor.Viewmodel;
using System.Text.RegularExpressions;
using System.Windows;

namespace SCRTranslationEditor
{

    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private readonly Regex _regex = new Regex("[^0-9]+");

        public SettingsWindow(VM_Settings settings)
        {
            InitializeComponent();
            DataContext = settings;
        }

        private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = _regex.IsMatch(e.Text);
        }
    }
}
