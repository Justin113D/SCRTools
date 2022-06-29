using SCR.Tools.Dialog.Data.Condition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCR.Tools.Dialog.Simulator.Viewmodeling.Condition
{
    internal class VmConditionData
    {
        public ConditionData Data { get; }

        public VmConditionData(ConditionData data)
        {
            Data = data;
        }
    }
}
