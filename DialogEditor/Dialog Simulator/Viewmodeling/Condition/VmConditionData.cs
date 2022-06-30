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

        public VmValueDictionary<bool> Flags { get; }

        public VmValueDictionary<bool> DynamicFlags { get; }

        public VmValueDictionary<int> Items { get; }

        public VmSlotDictionary<ChaoSlot, VmChaoSlot> Chao { get; }

        public VmIntSet Cards { get; }

        public VmSlotDictionary<TeamSlot, VmTeamSlot> TeamMembers{ get; }

        public VmIntList PartyMembers { get; }

        public VmConditionData(ConditionData data)
        {
            Data = data;

            Flags = new(Data.Flags, default);
            DynamicFlags = new(Data.DynamicFlags, default);
            Items = new(Data.Items, default);
            Chao = new(data.Chao, new(), (p, i) => new(p, i));
            Cards = new(data.Cards);
            TeamMembers = new(data.TeamMembers, new(), (p, i) => new(p, i));
            PartyMembers = new(data.PartyMembers);
        }
    }
}
