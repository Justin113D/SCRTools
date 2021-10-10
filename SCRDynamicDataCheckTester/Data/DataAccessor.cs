using SCR.Expression;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SCRDynamicDataCheckTester.Data
{
    public sealed class DataAccessor : IDataAccess<MockSCRData>
    {
        public static readonly DataAccessor DA = new();

        public ReadOnlyDictionary<string, DataKey<MockSCRData>> DataKeys { get; }

        private DataAccessor()
        {
            #pragma warning disable IDE0055
            Dictionary<string, DataKey<MockSCRData>> dataKeys = new()
            {
                { "F",  new("Flag",         KeyType.Boolean,    KeyType.None,       GetFlag) },
                { "D",  new("Dynamic Flag", KeyType.Boolean,    KeyType.None,       GetDynamicFlag) },
                { "R",  new("Rings",        KeyType.None,       KeyType.Number,     GetRings) },
                { "A",  new("Chao",         KeyType.Number,     KeyType.Number,     GetChao) },
                { "AL", new("Chao Level",   KeyType.Number,     KeyType.None,       GetChaoLevel) },
                { "C",  new("Card",         KeyType.Number,     KeyType.Number,     GetCard) },
                { "I",  new("Item",         KeyType.Number,     KeyType.Number,     GetItem) },
                { "T",  new("Team Member",  KeyType.Number,     KeyType.Boolean,    GetTeamMember) },
                { "P",  new("Party Member", KeyType.Number,     KeyType.Boolean,    GetPartyMember) },
                { "E",  new("Equipped",     KeyType.NumberList, KeyType.NumberList, GetEquipped) },
                { "EC", new("Equipped Count", KeyType.Number,   KeyType.Number,     GetEquippedCount) },
                { "L",  new("Level",        KeyType.Number,     KeyType.None,       GetLevel)},
                { "HP", new("Health Points", KeyType.Number,    KeyType.Number,     GetHealthPoints)}
            };
            #pragma warning restore IDE0055


            DataKeys = new(dataKeys);
        }

        private object GetFlag(long? id, MockSCRData data)
        {
            return data.Flags.TryGetValue((int)id.Value, out bool result) && result;
        }

        private object GetDynamicFlag(long? id, MockSCRData data)
        {
            return false;
        }

        private object GetRings(long? id, MockSCRData data)
        {
            return data.Rings;
        }

        private object GetChao(long? id, MockSCRData data)
        {
            if(id.HasValue)
                return data.Chao.TryGetValue((int)id.Value, out var t) ? t.count : 0;
            else
                return data.Chao.Count;
        }

        private object GetChaoLevel(long? id, MockSCRData data)
        {
            return data.Chao.TryGetValue((int)id.Value, out var t) ? t.level : 0;
        }

        private object GetCard(long? id, MockSCRData data)
        {
            if(id.HasValue)
                return data.Cards.Contains((int)id.Value);
            else
                return data.Cards.Count;
        }

        private object GetItem(long? id, MockSCRData data)
        {
            if(id.HasValue)
                return data.Items.TryGetValue((int)id.Value, out int result) ? result : 0;
            else
                return data.Items.Count;
        }

        private object GetTeamMember(long? id, MockSCRData data)
        {
            if(id.HasValue)
                return data.TeamMembers.ContainsKey((int)id.Value);
            else
                return data.TeamMembers.Count;
        }

        private object GetPartyMember(long? id, MockSCRData data)
        {
            if(id.HasValue)
                return data.PartyMembers.Contains((int)id.Value);
            else
                return data.PartyMembers.Count;
        }
        private object GetEquipped(long? id, MockSCRData data)
        {
            return Array.Empty<int>(); // TODO
        }

        private object GetEquippedCount(long? id, MockSCRData data)
        {
            return 0; // TODO
        }

        private object GetLevel(long? id, MockSCRData data)
        {
            return data.TeamMembers.TryGetValue((int)id.Value, out TeamSlot t) ? t.level : 0;
        }

        private object GetHealthPoints(long? id, MockSCRData data)
        {
            return 0; // TODO
        }

    }
}
