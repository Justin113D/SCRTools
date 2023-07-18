using SCR.Tools.TranslationEditor.FormatEditor.Viewmodeling;
using SCR.Tools.WPF.Theme;
using System.Windows;

namespace SCR.Tools.TranslationEditor.FormatEditor.WPF.Windows
{
    /// <summary>
    /// Interaction logic for WndMain.xaml
    /// </summary>
    public partial class WndMain : ThemeWindow
    {
        public WndMain()
        {
            InitDataContext();
            InitializeComponent();
        }

        protected override void Close(object sender, RoutedEventArgs e)
        {
            if (MenuBar.CloseConfirmation())
            {
                base.Close(sender, e);
            }
        }

        private void InitDataContext()
        {
            VmMain vm = new();
            DataContext = vm;
        }

        private void IB_New(object sender, object e)
        {
            MenuBar.NewFormat(sender, new());
        }

        private void IB_Open(object sender, object e)
        {
            MenuBar.LoadFormat(sender, new());
        }

        private void IB_Save(object sender, object e)
        {
            MenuBar.SaveFormat(sender, new());
        }

        private void IB_SaveAs(object sender, object e)
        {
            MenuBar.SaveFormatAs(sender, new());
        }

        private void IB_Export(object sender, object e)
        {
            MenuBar.Export(sender, new());
        }
    }
}
