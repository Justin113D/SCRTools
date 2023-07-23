using System.Windows;
using System.Windows.Controls;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;

namespace SCR.Tools.Dialog.Simulator.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for UcSimulatorView.xaml
    /// </summary>
    internal partial class UcSimulatorView : UserControl
    {
        public UcSimulatorView()
        {
            InitializeComponent();
        }

        private void Button_Undo(object sender, RoutedEventArgs e)
        {
            UndoChange();
        }

        private void Button_Redo(object sender, RoutedEventArgs e)
        {
            RedoChange();
        }
    }
}
