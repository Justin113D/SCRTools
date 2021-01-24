using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using SCRCommon.Viewmodels;

namespace SCRDialogEditor.Viewmodel
{
    public class VMMain : BaseViewModel
    {
        public VMGrid Grid { get; private set; }

        public VMMain()
        {
            Grid = new VMGrid();

            Grid.Nodes.Add(new VMNode(1));
        }
    }
}
