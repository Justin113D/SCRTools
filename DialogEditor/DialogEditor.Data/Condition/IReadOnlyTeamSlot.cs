using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCR.Tools.Dialog.Data.Condition
{
    public interface IReadOnlyTeamSlot
    {
        public int Level { get; }

        public int Health { get; }

        public int MaxHealth { get; }

        public int PowerPoints { get; }

        public int MaxPowerPoints { get; }

        public IReadOnlySet<int> Equipment { get; }
    }
}
