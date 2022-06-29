using SCR.Tools.Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SCR.Tools.Dialog.Data.Condition.ReadOnly
{
    public class ReadOnlyConditionData : IReadOnlyConditionData
    {
        #region Private collections

        internal readonly Dictionary<int, bool> _flags;
        internal readonly Dictionary<int, bool> _dynamicFlags;
        internal readonly Dictionary<int, int> _items;
        internal readonly Dictionary<int, ReadOnlyChaoSlot> _chao;
        internal readonly HashSet<int> _cards;
        internal readonly Dictionary<int, ReadOnlyTeamSlot> _teamMembers;
        internal readonly List<int> _partyMembers;

        #endregion

        public ReadOnlyDictionary<int, bool> Flags { get; }

        public ReadOnlyDictionary<int, bool> DynamicFlags { get; }

        public int Rings { get; internal set; }

        public ReadOnlyDictionary<int, int> Items { get; }

        public ReadOnlyDictionary<int, ReadOnlyChaoSlot> Chao { get; }

        public ReadOnlySet<int> Cards { get; }

        public ReadOnlyDictionary<int, ReadOnlyTeamSlot> TeamMembers { get; }

        public ReadOnlyList<int> PartyMembers { get; }

        IReadOnlyDictionary<int, bool> IReadOnlyConditionData.Flags => Flags;

        IReadOnlyDictionary<int, bool> IReadOnlyConditionData.DynamicFlags => DynamicFlags;

        IReadOnlyDictionary<int, int> IReadOnlyConditionData.Items => Items;

        IReadOnlyDictionary<int, IReadOnlyChaoSlot> IReadOnlyConditionData.Chao => (IReadOnlyDictionary<int, IReadOnlyChaoSlot>)Chao;

        IReadOnlySet<int> IReadOnlyConditionData.Cards => Cards;

        IReadOnlyDictionary<int, IReadOnlyTeamSlot> IReadOnlyConditionData.TeamMembers => (IReadOnlyDictionary<int, IReadOnlyTeamSlot>)TeamMembers;

        IReadOnlyList<int> IReadOnlyConditionData.PartyMembers => PartyMembers;

        internal ReadOnlyConditionData()
        {
            _flags = new();
            _dynamicFlags = new();
            _items = new();
            _chao = new();
            _cards = new();
            _teamMembers = new();
            _partyMembers = new();

            Flags = new(_flags);
            DynamicFlags = new(_dynamicFlags);
            Items = new(_items);
            Chao = new(_chao);
            Cards = new(_cards);
            TeamMembers = new(_teamMembers);
            PartyMembers = new(_partyMembers);
        }

        public ReadOnlyConditionData(IReadOnlyConditionData data)
        {
            _flags = new(data.Flags);
            _dynamicFlags = new(data.DynamicFlags);
            _items = new(data.Items);
            _chao = data.Chao.ToDictionary(x => x.Key, y => new ReadOnlyChaoSlot(y.Value));
            _cards = new(data.Cards);
            _teamMembers = data.TeamMembers.ToDictionary(x => x.Key, y => new ReadOnlyTeamSlot(y.Value));
            _partyMembers = new(data.PartyMembers);

            Flags = new(_flags);
            DynamicFlags = new(_dynamicFlags);
            Items = new(_items);
            Chao = new(_chao);
            Cards = new(_cards);
            TeamMembers = new(_teamMembers);
            PartyMembers = new(_partyMembers);
        }
 
    }
}
