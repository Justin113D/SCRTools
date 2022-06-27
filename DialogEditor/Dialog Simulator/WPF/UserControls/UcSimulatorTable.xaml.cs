using SCR.Tools.Dialog.Simulator.Viewmodeling;
using System.Windows.Controls;

namespace SCR.Tools.Dialog.Simulator.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for UcSimulatorTable.xaml
    /// </summary>
    internal partial class UcSimulatorTable : UserControl
    {
        public VmSimulator ViewModel
            => (VmSimulator)DataContext;

        public UcSimulatorTable()
        {
            InitializeComponent();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ListBox)sender).SelectedValue is not VmNode node)
            {
                return;
            }

            ViewModel.Jump(node);
        }
    }
}
