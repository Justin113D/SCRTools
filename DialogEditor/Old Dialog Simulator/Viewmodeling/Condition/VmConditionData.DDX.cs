using SCR.Tools.Dialog.Data.Condition;
using SCR.Tools.DynamicDataExpression;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SCR.Tools.Dialog.Simulator.Viewmodeling.Condition
{
    internal partial class VmConditionData : IDataAccess, IDataSetter
    {
        public static ReadOnlyDictionary<string, DataKey> AccessKeys { get; }

        public static ReadOnlyDictionary<string, DataKey> SetterKeys { get; }

        static VmConditionData()
        {
            Dictionary<string, DataKey> accessKeys = new()
            {
                { "LF",  new("Local Flag",      KeyType.Boolean,    KeyType.None        ) }, // From local flags
                { "LN",  new("Local Number",    KeyType.Number,     KeyType.None        ) }, // From local numbers

                { "F",  new("Flag",             KeyType.Boolean,    KeyType.None        ) }, // From flag storage
                { "D",  new("Dynamic Flag",     KeyType.Boolean,    KeyType.None        ) }, // Flag functions

                { "R",  new("Rings",            KeyType.None,       KeyType.Number      ) }, // Ring count
                { "A",  new("Chao",             KeyType.Number,     KeyType.Number      ) }, // Chao Count
                { "AL", new("Chao Level",       KeyType.Number,     KeyType.None        ) }, // Level of a chao
                { "C",  new("Card",             KeyType.Boolean,    KeyType.Number      ) }, // Whether a card is collected / Cards count
                { "I",  new("Item",             KeyType.Number,     KeyType.Number      ) }, // Item Count

                { "T",  new("Team Member",      KeyType.Boolean,    KeyType.Number      ) }, // Whether team member is acquired / acquired team member count
                { "P",  new("Party Member",     KeyType.Number,     KeyType.NumberList  ) }, // Party member at index / Party member array
                { "E",  new("Equipped",         KeyType.NumberList, KeyType.NumberList  ) }, // ID-less returns party equipment
                { "EC", new("Equipped Count",   KeyType.Number,     KeyType.Number      ) }, // ID-less returns party equipment count
                { "L",  new("Level",            KeyType.Number,     KeyType.None        ) },
                { "HP", new("Health Percent",   KeyType.Number,     KeyType.Number      ) }
            };

            Dictionary<string, DataKey> setterKeys = new()
            {
                { "LF",  new("Local Flag",      KeyType.Boolean,    KeyType.None) },
                { "LN",  new("Local Number",    KeyType.Number,     KeyType.None) },

                { "F",  new("Flag",             KeyType.Boolean,    KeyType.None) },
                { "R",  new("Rings",            KeyType.None,       KeyType.Number) },
                { "C",  new("Card",             KeyType.Boolean,    KeyType.None) },
                { "I",  new("Item",             KeyType.Number,     KeyType.None) },
                { "T",  new("Team Member",      KeyType.Boolean,    KeyType.None) },
            };

            AccessKeys = new(accessKeys);
            SetterKeys = new(setterKeys);
        }

        public object GetValue(string key, long? id)
        {
            if (id == null)
            {
                return GetNoIDValue(key);
            }
            else
            {
                return GetIDValue(key, (int)id.Value);
            }
        }

        private object GetNoIDValue(string key)
        {
            switch (key)
            {
                case "R":
                    return Data.Rings;
                case "A":
                    return Data.Chao.Count;
                case "C":
                    return Data.Cards.Count;
                case "I":
                    return Data.Items.Count;
                case "T":
                    return Data.TeamMembers.Count(x => x.Value.Unlocked);
                case "P":
                    return Data.PartyMembers.Count;
                case "E":
                    return Data.PartyMembers.SelectMany(
                        x => Data.TeamMembers.TryGetValue(x, out TeamSlot? ts)
                        ? ts.Equipment.ToArray()
                        : Array.Empty<int>()).ToArray();
                case "HP":
                    long maxHealth = 0;
                    long health = 0;
                    foreach (var t in Data.PartyMembers)
                    {
                        if (Data.TeamMembers.TryGetValue(t, out TeamSlot? ts))
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

        private object GetIDValue(string key, int id)
        {
            switch (key)
            {
                case "LF":
                    return LocalBooleans.TryGetValue(id, out bool lf) && lf;
                case "LN":
                    return LocalNumbers.TryGetValue(id, out double ln) ? ln : 0d;
                case "F":
                    return Data.Flags.TryGetValue(id, out bool f) && f;
                case "D":
                    return false;
                case "A":
                    return Data.Chao.TryGetValue(id, out var chao) ? chao.Count : 0;
                case "AL":
                    return Data.Chao.TryGetValue(id, out var chaoLevel) ? chaoLevel.Level : 0;
                case "C":
                    return Data.Cards.Contains(id);
                case "I":
                    return Data.Items.TryGetValue(id, out int item) ? item : 0;
                case "T":
                    return Data.TeamMembers.TryGetValue(id, out TeamSlot? tsu) && tsu.Unlocked;
                case "P":
                    return Data.PartyMembers.Contains(id);
                case "E":
                    return Data.TeamMembers.TryGetValue(id, out TeamSlot? tse) ? tse.Equipment : Array.Empty<int>();
                case "L":
                    return Data.TeamMembers.TryGetValue(id, out TeamSlot? tsl) ? tsl.Level : 0;
                case "HP":
                    return Data.TeamMembers.TryGetValue(id, out TeamSlot? hpts) ? (long)((hpts.Health / (double)hpts.MaxHealth) * 100) : 0;
                default:
                    throw new InvalidOperationException();
            }
        }

        public void SetValue(string key, long? id, object value)
        {
            try
            {
                if (id == null)
                {
                    SetNoIDValue(key, value);
                }
                else
                {
                    SetIDValue(key, (int)id.Value, value);
                }
            }
            catch(InvalidOperationException)
            {
                throw new InvalidOperationException($"Invalid operation setting value {key} {id} to {value}");
            }
        }

        private void SetNoIDValue(string key, object value)
        {
            switch (key)
            {
                case "r":
                    Data.Rings = (int)value;
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        private void SetIDValue(string key, int id, object value)
        {
            KeyType type = SetterKeys[key].IDType;
            if(type == KeyType.Boolean)
            {
                SetIDBoolean(key, id, (bool)value);
            }
            else if(type == KeyType.Number)
            {
                SetIDNumber(key, id, (double)value);
            }
        }

        private void SetIDNumber(string key, int id, double value)
        {
            switch (key)
            {
                case "LN":
                    if (!LocalNumbers.TryAdd(id, value))
                    {
                        LocalNumbers[id] = value;
                    }
                    break;
                case "I":
                    if(!Data.Items.TryAdd(id, (int)value))
                    {
                        Data.Items[id] = (int)value;
                    }
                    break;
                default:
                    break;
            }
        }

        private void SetIDBoolean(string key, int id, bool value)
        {
            switch (key)
            {
                case "LF":
                    if(!LocalBooleans.TryAdd(id, value))
                    {
                        LocalBooleans[id] = value;
                    }
                    break;
                case "F":
                    if (!Data.Flags.TryAdd(id, value))
                    {
                        Data.Flags[id] = value;
                    }
                    break;
                case "C":
                    bool containsCard = Data.Cards.Contains(id);
                    if(value && !containsCard)
                    {
                        Data.Cards.Add(id);
                    }
                    else if(!value && containsCard)
                    {
                        Data.Cards.Remove(id);
                    }
                    break;
                case "T":
                    if(Data.TeamMembers.TryGetValue(id, out TeamSlot? ts))
                    {
                        ts.Unlocked = value;
                    }
                    else if(value)
                    {
                        Data.TeamMembers.Add(id, new()
                        {
                            Unlocked = true
                        });
                    }
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
