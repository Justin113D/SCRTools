using SCRLanguageEditor.Viewmodel;
using System.Collections;
using System.Linq;
using System.Windows;

namespace SCRLanguageEditor
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
