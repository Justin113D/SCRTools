using Gu.Wpf.NumericInput;
using SCR.Tools.Dialog.Simulator.Viewmodeling.Condition;
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

namespace SCR.Tools.Dialog.Simulator.WPF.UserControls.SubControls
{
    /// <summary>
    /// Interaction logic for UcIntSet.xaml
    /// </summary>
    public partial class UcIntSet : UserControl
    {
        public UcIntSet()
        {
            InitializeComponent();
        }

        private void IntBox_KeyDown(object sender, KeyEventArgs e)
        {
            IntBox textbox = (IntBox)sender;
            if (e.Key == Key.Enter && int.TryParse(textbox.Text, out int check) && check == textbox.Value)
            {
                ((VmIntSet)DataContext).AddValue(check);
                textbox.Text = "";
            }
        }
    }
}
