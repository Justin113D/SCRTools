using SCR.Tools.Dialog.Editor.Viewmodeling;
using System;
using System.IO;
using System.Windows;
using Window = SCR.Tools.WPF.Styling.Window;

namespace SCR.Tools.Dialog.Editor.WPF.Windows
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

            string optionsPath = Properties.Settings.Default.DefaultDialogOptionsPath;
            if (string.IsNullOrWhiteSpace(optionsPath))
                return;

            if (!File.Exists(optionsPath))
            {
                MessageBox.Show("Default dialog options path does not resolve to a file", "Default dialog options not found", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                string data = File.ReadAllText(optionsPath);
                vm.DialogOptions.Read(data, optionsPath);
            }
            catch (Exception e)
            {
                MessageBox.Show("Default dialog options path is not valid!\n " + e.Message, "Error loading default dialog options", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected override void Close(object sender, RoutedEventArgs e)
        {
            if (MenuBar.CloseConfirmation())
            {
                base.Close(sender, e);
            }
        }


        private void IB_New(object sender, object e)
        {
            if (Gridview.ConnectingSocket != null)
            {
                return;
            }

            MenuBar.NewDialog(sender, new());
        }

        private void IB_Open(object sender, object e)
        {
            if (Gridview.ConnectingSocket != null)
            {
                return;
            }

            MenuBar.LoadDialog(sender, new());
        }

        private void IB_Save(object sender, object e)
        {
            if (Gridview.ConnectingSocket != null)
            {
                return;
            }

            MenuBar.SaveDialog(sender, new());
        }

        private void IB_SaveAs(object sender, object e)
        {
            if (Gridview.ConnectingSocket != null)
            {
                return;
            }

            MenuBar.SaveDialogAs(sender, new());
        }

        private void IB_Undo(object sender, object e)
        {
            if (Gridview.ConnectingSocket != null)
            {
                return;
            }

            ((VmMain)DataContext).CmdUndo.Execute(null);
        }

        private void IB_Redo(object sender, object e)
        {
            if (Gridview.ConnectingSocket != null)
            {
                return;
            }

            ((VmMain)DataContext).CmdRedo.Execute(null);
        }
    }
}
