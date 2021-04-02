using SCRDialogEditor.Viewmodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCRDialogEditor.XAML
{
    internal static class DesignDataFactory
    {
        static DesignDataFactory()
        {
            Main = new VmMain();
            Grid = new VmGrid(Main, new());

            Node = new VmNode(Grid, new());
            Node.AddOutput();

            VmNodeOutput Output = Node.Outputs[0];
            Output.IsExpanded = true;
            Output.Text = "This is a test dialog!";
        }

        public static VmMain Main { get; }

        public static VmGrid Grid { get; }

        public static VmNode Node { get; }
    }
}
