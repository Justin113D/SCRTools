using SCRCommon.WpfStyles;
using SCRDialogEditor.Viewmodel;

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
