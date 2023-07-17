using SCR.Tools.Dialog.Data;
using SCR.Tools.Dialog.Simulator.Viewmodeling;
using SCR.Tools.UndoRedo;
using System;
using System.Windows;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;
using Window = SCR.Tools.WPF.Styling.Window;

namespace SCR.Tools.Dialog.Simulator.WPF.Windows
{
    /// <summary>
    /// Interaction logic for WndSimulator.xaml
    /// </summary>
    public partial class WndSimulatorDialog : Window
    {
        private WndSimulatorDialog(VmMain vm)
        {
            DataContext = vm;
            InitializeComponent();
        }

        public static void RunSimulator(DialogContainer data, DialogOptions options)
        {
            VmMain? viewmodel;

            try
            {
                viewmodel = new(data, options);
            }
            catch (SimulatorException sx)
            {
                MessageBox.Show(sx.Message, "Condition invalid", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Dialog invalid", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ChangeTracker previous = GlobalChangeTracker;
            viewmodel.SimulatorTracker.Use();

            new WndSimulatorDialog(viewmodel).ShowDialog();

            previous.Use();
        }

        private void IB_Undo(object sender, object e)
        {
            UndoChange();
        }

        private void IB_Redo(object sender, object e)
        {
            RedoChange();
        }
    }
}
