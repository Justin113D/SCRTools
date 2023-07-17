using SCR.Tools.Dialog.Data.Condition;
using SCR.Tools.UndoRedo.Collections;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;

namespace SCR.Tools.Dialog.Simulator.Viewmodeling.Condition
{
    internal partial class VmConditionData
    {
        public ConditionData Data { get; }

        public VmValueDictionary<bool> Flags { get; }

        public VmValueDictionary<bool> DynamicFlags { get; }

        public VmValueDictionary<int> Items { get; }

        public VmSlotDictionary<ChaoSlot, VmChaoSlot> Chao { get; }

        public VmIntSet Cards { get; }

        public VmSlotDictionary<TeamSlot, VmTeamSlot> TeamMembers{ get; }

        public VmIntList PartyMembers { get; }

        public TrackDictionary<int, double> LocalNumbers { get; }

        public TrackDictionary<int, bool> LocalBooleans { get; }

        public VmConditionData(ConditionData data)
        {
            Data = data;

            LocalNumbers = new();
            LocalBooleans = new();

            Flags = new(Data.Flags, default);
            DynamicFlags = new(Data.DynamicFlags, default);
            Items = new(Data.Items, default);
            Chao = new(data.Chao, new(), (p, i) => new(p, i));
            Cards = new(data.Cards);
            TeamMembers = new(data.TeamMembers, new(), (p, i) => new(p, i));
            PartyMembers = new(data.PartyMembers);
        }

        public void ResetLocal()
        {
            BeginChangeGroup();

            LocalNumbers.Clear();
            LocalBooleans.Clear();

            EndChangeGroup();
        }
    }
}
