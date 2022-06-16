using SCR.Tools.DialogEditor.Viewmodeling;
using Window = SCR.Tools.WPF.Styling.Window;

namespace SCR.Tools.DialogEditor.WPF.Windows
{
    /// <summary>
    /// Interaction logic for WndMain.xaml
    /// </summary>
    public partial class WndMain : Window
    {
        public WndMain()
        {
            InitDataContext();
            InitializeComponent();
        }

        private void InitDataContext()
        {
            VmMain vm = new();
            DataContext = vm;
        }

        private void IB_New(object sender, object e)
        {
            MenuBar.NewDialog(sender, new());
        }

        private void IB_Open(object sender, object e)
        {
            MenuBar.LoadDialog(sender, new());
        }

        private void IB_Save(object sender, object e)
        {
            MenuBar.SaveDialog(sender, new());
        }

        private void IB_SaveAs(object sender, object e)
        {
            MenuBar.SaveDialogAs(sender, new());
        }
    }
}
