using SCR.Tools.UndoRedo.Collections;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;
using System.Linq;
using System.Collections.Generic;
using System;

namespace SCR.Tools.Dialog.Data.Condition
{
    /// <summary>
    /// Mock Condition data setup for when simulating a dialog
    /// </summary>
    public class ConditionData : ICloneable
    {
        private int _rings;

        public TrackDictionary<int, bool> Flags { get; }

        public TrackDictionary<int, bool> DynamicFlags { get; }

        public int Rings
        {
            get => _rings;
            set => BlankValueChange((v) => _rings = v, _rings, value);
        }

        public TrackDictionary<int, int> Items { get; }

        public TrackDictionary<int, ChaoSlot> Chao { get; }

        public TrackSet<int> Cards { get; }

        public TrackDictionary<int, TeamSlot> TeamMembers { get; }

        public TrackList<int> PartyMembers { get; }

        public ConditionData()
        {
            Flags = new();
            DynamicFlags = new();
            Items = new();
            Chao = new();
            Cards = new();
            TeamMembers = new();
            PartyMembers = new();
        }

        private ConditionData(ConditionData data)
        {
            Flags = new(new Dictionary<int, bool>(data.Flags));
            DynamicFlags = new(new Dictionary<int, bool>(data.DynamicFlags));
            Items = new(new Dictionary<int, int>(data.Items));
            Chao = new(data.Chao.ToDictionary(x => x.Key, x => x.Value.Clone()));
            Cards = new(new HashSet<int>(data.Cards));
            TeamMembers = new(data.TeamMembers.ToDictionary(x => x.Key, x => x.Value.Clone()));
            PartyMembers = new(new List<int>(data.PartyMembers));

            _rings = data.Rings;
        }

        public ConditionData Clone()
            => new(this);

        object ICloneable.Clone()
            => Clone();

    }
}
