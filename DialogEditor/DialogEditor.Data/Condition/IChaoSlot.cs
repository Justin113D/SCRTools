using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCR.Tools.Dialog.Data.Condition
{
    public interface IReadOnlyChaoSlot
    {
        public int Count { get; }

        public int Level { get; }
    }
}
