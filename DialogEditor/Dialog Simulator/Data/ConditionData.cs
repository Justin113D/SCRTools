using SCR.Tools.UndoRedo.Collections;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;
using System;

namespace SCR.Tools.Dialog.Simulator.Data
{
    [Serializable]
    public class ConditionData
    {
        private int _rings;

        public TrackDictionary<int, bool> Flags { get; set; }

        public int Rings
        {
            get => _rings;
            set => BlankValueChange((v) => _rings = v, _rings, value);
        }

        public TrackDictionary<int, int> Items { get; set; }

        public TrackDictionary<int, ChaoSlot> Chao { get; set; }

        public TrackSet<int> Cards { get; set; }

        public TrackDictionary<int, TeamSlot> TeamMembers { get; set; }

        public TrackList<int> PartyMembers { get; set; }

        public ConditionData()
        {
            Flags = new();
            Items = new();
            Chao = new();
            Cards = new();
            TeamMembers = new();
            PartyMembers = new();
        }
    }
}
