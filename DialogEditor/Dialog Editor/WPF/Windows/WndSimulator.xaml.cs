using SCR.Tools.DialogEditor.Data;
using SCR.Tools.DialogEditor.Viewmodeling.Simulator;
using SCR.Tools.UndoRedo;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;
using System;
using System.Windows;
using Window = SCR.Tools.WPF.Styling.Window;

namespace SCR.Tools.DialogEditor.WPF.Windows
{
    /// <summary>
    /// Interaction logic for WndSimulator.xaml
    /// </summary>
    public partial class WndSimulator : Window
    {
        private readonly ChangeTracker _previous;

        private WndSimulator(VmSimulator vm)
        {
            _previous = GlobalChangeTrackerC.GlobalChangeTracker;
            vm.SimulatorTracker.Use();
            DataContext = vm;
            InitializeComponent();

        }

        protected override void OnClosed(EventArgs e)
        {
            _previous.Use();
            base.OnClosed(e);
        }

        public static void RunSimulator(Dialog data, DialogOptions options)
        {
            VmSimulator? viewmodel;

            try
            {
                viewmodel = new(data, options);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Dialog invalid", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            new WndSimulator(viewmodel).ShowDialog();
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
