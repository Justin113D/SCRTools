using Gu.Wpf.NumericInput;
using SCR.Tools.Dialog.Data.Condition;
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
    public class SlotDictTypeTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? TeamMemberTemplate { get; set; }

        public DataTemplate? ChaoTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is VmTeamSlot)
            {
                return TeamMemberTemplate ?? throw new InvalidOperationException();
            }
            else if (item is VmChaoSlot)
            {
                return ChaoTemplate ?? throw new InvalidOperationException();
            }

            throw new InvalidOperationException("Item type is not valid");
        }
    }

    /// <summary>
    /// Interaction logic for UcSlotDictionary.xaml
    /// </summary>
    public partial class UcSlotDictionary : UserControl
    {
        public UcSlotDictionary()
        {
            InitializeComponent();
        }

        private void IntBox_KeyDown(object sender, KeyEventArgs e)
        {
            IntBox textbox = (IntBox)sender;
            if (e.Key == Key.Enter && int.TryParse(textbox.Text, out int check) && check == textbox.Value)
            {
                if (DataContext is VmSlotDictionary<TeamSlot, VmTeamSlot> teamDict)
                {
                    teamDict.AddSlot(check);
                }
                else if (DataContext is VmSlotDictionary<ChaoSlot, VmChaoSlot> chaoDict)
                {
                    chaoDict.AddSlot(check);
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
