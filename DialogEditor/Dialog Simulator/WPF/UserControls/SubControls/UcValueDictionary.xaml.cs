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

    public class ValueDictTypeTemplateSelector : DataTemplateSelector
    {

        public DataTemplate? IntegerTemplate { get; set; }

        public DataTemplate? BooleanTemplate { get; set; } 

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            Type[] types = item.GetType().GetGenericArguments();

            if (types[0] == typeof(int))
            {
                return IntegerTemplate ?? throw new InvalidOperationException();
            }
            else if(types[0] == typeof(bool))
            {
                return BooleanTemplate ?? throw new InvalidOperationException();
            }

            throw new InvalidOperationException("Item type is not valid");
        }
    }

    /// <summary>
    /// Interaction logic for UcValueDictionary.xaml
    /// </summary>
    public partial class UcValueDictionary : UserControl
    {

        public UcValueDictionary()
        {
            InitializeComponent();
        }

        private void IntBox_KeyDown(object sender, KeyEventArgs e)
        {
            IntBox textbox = (IntBox)sender;
            if (e.Key == Key.Enter && int.TryParse(textbox.Text, out int check) && check == textbox.Value)
            {
                if(DataContext is VmValueDictionary<int> intDict)
                {
                    intDict.AddValue(check);
                }
                else if(DataContext is VmValueDictionary<bool> boolDict)
                {
                    boolDict.AddValue(check);
                }
                else
                {
                    throw new InvalidOperationException("datacontext type is not valid");
                }

                textbox.Text = "";
            }
        }
    }
}
