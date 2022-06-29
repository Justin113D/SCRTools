using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCR.Tools.Dialog.Data.Condition
{
    public interface IReadOnlyConditionData
    {
        public int Rings { get; }

        public IReadOnlyDictionary<int, bool> Flags { get; }

        public IReadOnlyDictionary<int, bool> DynamicFlags { get; }

        public IReadOnlyDictionary<int, int> Items { get; }

        public IReadOnlyDictionary<int, IReadOnlyChaoSlot> Chao { get; }

        public IReadOnlySet<int> Cards { get; }

        public IReadOnlyDictionary<int, IReadOnlyTeamSlot> TeamMembers { get; }

        public IReadOnlyList<int> PartyMembers { get; }
    }
}
