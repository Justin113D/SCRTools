using SCR.Tools.WPF.Styling;
using SCR.Tools.DialogEditor.Viewmodeling;
using SCR.Tools.WPF.IO;

namespace SCR.Tools.DialogEditor.WPF.Windows
{
    /// <summary>
    /// Interaction logic for DialogOptionsWindow.xaml
    /// </summary>
    public partial class WndDialogOptions : Window
    {

        private readonly TextFileHandler _dialogOptionsFileHandler;

        public WndDialogOptions(VmDialogOptions vmDialogOptions, TextFileHandler dialogOptionsFileHandler)
        {
            DataContext = vmDialogOptions;
            _dialogOptionsFileHandler = dialogOptionsFileHandler;
            InitializeComponent();
        }

        private void MenuItem_OpenFile(object sender, System.Windows.RoutedEventArgs e)
        {
            _dialogOptionsFileHandler.Open();
        }

        private void MenuItem_NewFile(object sender, System.Windows.RoutedEventArgs e)
        {
            _dialogOptionsFileHandler.Reset();
        }

        private void MenuItem_Save(object sender, System.Windows.RoutedEventArgs e)
        {
            _dialogOptionsFileHandler.Save(false);
        }

        private void MenuItem_SaveAs(object sender, System.Windows.RoutedEventArgs e)
        {
            _dialogOptionsFileHandler.Save(true);
        }

        private void IB_New(object sender, object e)
        {
            _dialogOptionsFileHandler.Reset();
        }

        private void IB_Open(object sender, object e)
        {
            _dialogOptionsFileHandler.Open();
        }

        private void IB_Save(object sender, object e)
        {
            _dialogOptionsFileHandler.Save(false);
        }

        private void IB_SaveAs(object sender, object e)
        {
            _dialogOptionsFileHandler.Save(true);
        }
    }
}
