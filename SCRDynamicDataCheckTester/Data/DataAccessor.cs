using SCR.Expression;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
                { "E",  new("Equipped",     KeyType.NumberList, KeyType.NumberList, GetEquipped) }, // idless returns party equipment
                { "EC", new("Equipped Count", KeyType.Number,   KeyType.Number,     GetEquippedCount) }, // idless returns party equipment count
                { "L",  new("Level",        KeyType.Number,     KeyType.None,       GetLevel)},
                { "HP", new("Health Percent", KeyType.Number,    KeyType.Number,     GetHealthPoints)}
            };
            #pragma warning restore IDE0055


            DataKeys = new(dataKeys);
        }

        private object GetFlag(double? id, MockSCRData data)
        {
            return data.Flags.TryGetValue((int)id.Value, out bool result) && result;
        }

        private object GetDynamicFlag(double? id, MockSCRData data)
        {
            return false;
        }

        private object GetRings(double? id, MockSCRData data)
        {
            return data.Rings;
        }

        private object GetChao(double? id, MockSCRData data)
        {
            if(id.HasValue)
                return data.Chao.TryGetValue((int)id.Value, out var t) ? t.Count : 0;
            else
                return data.Chao.Count;
        }

        private object GetChaoLevel(double? id, MockSCRData data)
        {
            return data.Chao.TryGetValue((int)id.Value, out var t) ? t.Level : 0;
        }

        private object GetCard(double? id, MockSCRData data)
        {
            if(id.HasValue)
                return data.Cards.Contains((int)id.Value);
            else
                return data.Cards.Count;
        }

        private object GetItem(double? id, MockSCRData data)
        {
            if(id.HasValue)
                return data.Items.TryGetValue((int)id.Value, out int result) ? result : 0;
            else
                return data.Items.Count;
        }

        private object GetTeamMember(double? id, MockSCRData data)
        {
            if(id.HasValue)
                return data.TeamMembers.ContainsKey((int)id.Value);
            else
                return data.TeamMembers.Count;
        }

        private object GetPartyMember(double? id, MockSCRData data)
        {
            if(id.HasValue)
                return data.PartyMembers.Contains((int)id.Value);
            else
                return data.PartyMembers.Count;
        }
        private object GetEquipped(double? id, MockSCRData data)
        {
            if(id.HasValue)
                return data.TeamMembers.TryGetValue((int)id.Value, out TeamSlot ts) ? ts.Equipment : Array.Empty<int>();
            else
                return data.PartyMembers.SelectMany(x => data.TeamMembers.TryGetValue(x, out TeamSlot ts) ? ts.Equipment : Array.Empty<int>()).ToArray();
        }

        private object GetEquippedCount(double? id, MockSCRData data)
        {
            return ((int[])GetEquipped(id, data)).Length;
        }

        private object GetLevel(double? id, MockSCRData data)
        {
            return data.TeamMembers.TryGetValue((int)id.Value, out TeamSlot t) ? t.Level : 0;
        }

        private object GetHealthPoints(double? id, MockSCRData data)
        {
            if(id.HasValue)
                return data.TeamMembers.TryGetValue((int)id.Value, out TeamSlot ts) ? (long)((ts.Health / (double)ts.MaxHealth) * 100) : 0;
            else
            {
                long maxHealth = 0;
                long health = 0;
                foreach(var t in data.PartyMembers)
                {
                    if(data.TeamMembers.TryGetValue((int)id.Value, out TeamSlot ts))
                    {
                        maxHealth += ts.MaxHealth;
                        health += ts.Health;
                    }
                }
                return (long)((health / (double)maxHealth) * 100);
            }
        }

    }
}
