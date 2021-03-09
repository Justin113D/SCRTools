using SCRDialogEditor.Viewmodel;
using System.Windows;

namespace SCRDialogEditor
{

    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class WndSettings : Window
    {
        public WndSettings(VmSettings settings)
        {
            DataContext = settings;
            InitializeComponent();
        }
    }
}
