using SCRCommon.Viewmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCRDialogEditor.Viewmodel
{
    public class VMNode : BaseViewModel
    {
        public int NodeID { get; set; }

        public int PositionX { get; set; }

        public int PositionY { get; set; }

        public VMNode(int nodeID)
        {
            NodeID = nodeID;
        }
    }
}
