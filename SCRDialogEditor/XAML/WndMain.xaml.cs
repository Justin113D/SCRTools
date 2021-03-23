using SCRDialogEditor.Viewmodel;

namespace SCRDialogEditor.XAML
{
    public partial class WndMain : SCRCommon.WpfStyles.Window
    {
        public WndMain()
        {
            DataContext = new VmMain();
            InitializeComponent();

        }
    }
}
