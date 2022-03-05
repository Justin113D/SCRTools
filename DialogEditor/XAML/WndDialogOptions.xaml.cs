using SCR.Tools.WPF.Styling;
using SCR.Tools.DialogEditor.Viewmodel;

namespace SCR.Tools.DialogEditor.XAML
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
