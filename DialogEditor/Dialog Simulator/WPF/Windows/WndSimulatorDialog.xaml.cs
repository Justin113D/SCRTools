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
        private readonly ChangeTracker _previous;

        private WndSimulatorDialog(VmSimulator vm)
        {
            _previous = GlobalChangeTracker;
            vm.SimulatorTracker.Use();
            DataContext = vm;
            InitializeComponent();
        }

        protected override void OnClosed(EventArgs e)
        {
            _previous.Use();
            base.OnClosed(e);
        }

        public static void RunSimulator(DialogContainer data, DialogOptions options)
        {
            VmSimulator? viewmodel;

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

            new WndSimulatorDialog(viewmodel).ShowDialog();
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
