using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCR.Tools.Dialog.Data.Condition.ReadOnly
{
    public class ReadOnlyChaoSlot : IReadOnlyChaoSlot
    {
        public int Count { get; internal set; }

        public int Level { get; internal set; }


        internal ReadOnlyChaoSlot() { }

        public ReadOnlyChaoSlot(IReadOnlyChaoSlot slot)
        {
            Count = slot.Count;
            Level = slot.Level;
        }
    }
}
