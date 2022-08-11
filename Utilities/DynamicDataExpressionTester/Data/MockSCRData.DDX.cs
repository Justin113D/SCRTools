using SCR.Tools.DynamicDataExpression;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SCR.Tools.DynamicDataExpressionTester.Data
{
    public partial class MockSCRData : IDataAccess
    {
        public static ReadOnlyDictionary<string, DataKey> AccessKeys { get; }

        static MockSCRData()
        {
            Dictionary<string, DataKey> accessKeys = new()
            {
                { "F",  new("Flag",         KeyType.Boolean,    KeyType.None) },
                { "D",  new("Dynamic Flag", KeyType.Boolean,    KeyType.None) },
                { "R",  new("Rings",        KeyType.None,       KeyType.Number) },
                { "A",  new("Chao",         KeyType.Number,     KeyType.Number) },
                { "AL", new("Chao Level",   KeyType.Number,     KeyType.None) },
                { "C",  new("Card",         KeyType.Number,     KeyType.Number) },
                { "I",  new("Item",         KeyType.Number,     KeyType.Number) },
                { "T",  new("Team Member",  KeyType.Boolean,     KeyType.Number) },
                { "P",  new("Party Member", KeyType.Boolean,     KeyType.Number) },
                { "E",  new("Equipped",     KeyType.NumberList, KeyType.NumberList) }, // idless returns party equipment
                { "L",  new("Level",        KeyType.Number,     KeyType.None)},
                { "HP", new("Health Percent", KeyType.Number,    KeyType.Number)}
            };

            AccessKeys = new(accessKeys);
        }

        public object GetValue(string key, long? id)
        {
            if(id == null)
            {
                return GetNoIDValue(key);
            }
            else
            {
                return GetIDValue(key, id.Value);
            }
        }

        private object GetNoIDValue(string key)
        {
            switch (key)
            {
                case "R":
                    return Rings;
                case "A":
                    return Chao.Count;
                case "C":
                    return Cards.Count;
                case "I":
                    return Items.Count;
                case "T":
                    return TeamMembers.Count;
                case "P":
                    return PartyMembers.Count;
                case "E":
                    return PartyMembers.SelectMany(
                        x => TeamMembers.TryGetValue(x, out TeamSlot ts) 
                        ? ts.Equipment 
                        : Array.Empty<int>()).ToArray();
                case "HP":
                    long maxHealth = 0;
                    long health = 0;
                    foreach (var t in PartyMembers)
                    {
                        if (TeamMembers.TryGetValue(t, out TeamSlot ts))
                        {
                            maxHealth += ts.MaxHealth;
                            health += ts.Health;
                        }
                    }
                    return (long)((health / (double)maxHealth) * 100);
                default:
                    throw new InvalidOperationException();
            }
        }

        private object GetIDValue(string key, long id)
        {
            switch (key)
            {
                case "F":
                    return Flags.TryGetValue((int)id, out bool result) && result;
                case "D":
                    return false;
                case "A":
                    return Chao.TryGetValue((int)id, out var chao) ? chao.Count : 0;
                case "AL":
                    return Chao.TryGetValue((int)id, out var chaoLevel) ? chaoLevel.Level : 0;
                case "C":
                    return Cards.Contains((int)id);
                case "I":
                    return Items.TryGetValue((int)id, out int item) ? item : 0;
                case "T":
                    return TeamMembers.ContainsKey((int)id);
                case "P":
                    return PartyMembers.Contains((int)id);
                case "E":
                    return TeamMembers.TryGetValue((int)id, out TeamSlot ts) ? ts.Equipment : Array.Empty<int>();
                case "L":
                    return TeamMembers.TryGetValue((int)id, out TeamSlot t) ? t.Level : 0;
                case "HP":
                    return TeamMembers.TryGetValue((int)id, out TeamSlot hpts) ? (long)((hpts.Health / (double)hpts.MaxHealth) * 100) : 0;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
