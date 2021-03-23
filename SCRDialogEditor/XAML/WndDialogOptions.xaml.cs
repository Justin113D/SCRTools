using SCRCommon.WpfStyles;
using SCRDialogEditor.Viewmodel;

namespace SCRDialogEditor.XAML
{
    /// <summary>
    /// Interaction logic for DialogOptionsWindow.xaml
    /// </summary>
    public partial class WndDialogOptions : Window
    {
        public WndDialogOptions(VmDialogOptions vmDO)
        {
            DataContext = vmDO;
            InitializeComponent();
        }
    }
}
