using SCR.Tools.WPF.Styling;
using SCR.Tools.Dialog.Editor.Viewmodeling;
using SCR.Tools.WPF.IO;
using System.Windows;
using Window = SCR.Tools.WPF.Styling.Window;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;
using Ookii.Dialogs.Wpf;

namespace SCR.Tools.Dialog.Editor.WPF.Windows
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

        protected override void Close(object sender, RoutedEventArgs e)
        {
            if (_dialogOptionsFileHandler?.ResetConfirmation() ?? true)
            {
                base.Close(sender, e);
            }
        }

        private void MenuItem_OpenFile(object sender, RoutedEventArgs e)
        {
            _dialogOptionsFileHandler.Open();
        }

        private void MenuItem_NewFile(object sender, RoutedEventArgs e)
        {
            _dialogOptionsFileHandler.Reset();
        }

        private void MenuItem_Save(object sender, RoutedEventArgs e)
        {
            _dialogOptionsFileHandler.Save(false);
        }

        private void MenuItem_SaveAs(object sender, RoutedEventArgs e)
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

        private void IB_Undo(object sender, object e)
        {
            UndoChange();
        }

        private void IB_Redo(object sender, object e)
        {
            RedoChange();
        }

        private void MenuItem_Undo(object sender, RoutedEventArgs e)
        {
            UndoChange();
        }

        private void MenuItem_Redo(object sender, RoutedEventArgs e)
        {
            RedoChange();
        }

        private void PortraitsPathDialog(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog ofd = new();

            if (ofd.ShowDialog() != true)
            {
                return;
            }

            ((VmDialogOptions)DataContext).PortraitsPath = ofd.SelectedPath;
        }
    }
}
