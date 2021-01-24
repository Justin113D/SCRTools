using SCRTranslationEditor.Viewmodel;
using System.Windows;

namespace SCRTranslationEditor
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow(VM_Settings settings)
        {
            InitializeComponent();
            DataContext = settings;
        }
    }
}
