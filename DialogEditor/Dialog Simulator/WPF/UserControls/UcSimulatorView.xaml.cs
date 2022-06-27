using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
