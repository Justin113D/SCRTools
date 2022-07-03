using SCR.Tools.Dialog.Data.Condition;
using SCR.Tools.DynamicDataExpression;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SCR.Tools.Dialog.Simulator.Viewmodeling.Condition
{
    internal sealed class VmConditionDataAccessor : IDataAccess<VmConditionData>
    {
        public static readonly VmConditionDataAccessor DA = new();

        public ReadOnlyDictionary<string, DataKey<VmConditionData>> DataKeys { get; }

        private VmConditionDataAccessor()
        {
            #pragma warning disable IDE0055
            Dictionary<string, DataKey<VmConditionData>> dataKeys = new()
            {
                { "LF",  new("Local Flag",      KeyType.Boolean,    KeyType.None,       GetLocalFlag) }, // From local flags
                { "LN",  new("Local Number",    KeyType.Number,     KeyType.None,       GetLocalNumber) }, // From local numbers

                { "F",  new("Flag",             KeyType.Boolean,    KeyType.None,       GetFlag) }, // From flag storage
                { "D",  new("Dynamic Flag",     KeyType.Boolean,    KeyType.None,       GetDynamicFlag) }, // Flag functions

                { "R",  new("Rings",            KeyType.None,       KeyType.Number,     GetRings) }, // Ring count
                { "A",  new("Chao",             KeyType.Number,     KeyType.Number,     GetChao) }, // Chao Count
                { "AL", new("Chao Level",       KeyType.Number,     KeyType.None,       GetChaoLevel) }, // Level of a chao
                { "C",  new("Card",             KeyType.Boolean,    KeyType.Number,     GetCard) }, // Whether a card is collected / Cards count
                { "I",  new("Item",             KeyType.Number,     KeyType.Number,     GetItem) }, // Item Count

                { "T",  new("Team Member",      KeyType.Boolean,    KeyType.Number,     GetTeamMember) }, // Whether team member is acquired / acquired team member count
                { "P",  new("Party Member",     KeyType.Number,     KeyType.NumberList, GetPartyMember) }, // Party member at index / Party member array
                { "E",  new("Equipped",         KeyType.NumberList, KeyType.NumberList, GetEquipped) }, // ID-less returns party equipment
                { "EC", new("Equipped Count",   KeyType.Number,     KeyType.Number,     GetEquippedCount) }, // ID-less returns party equipment count
                { "L",  new("Level",            KeyType.Number,     KeyType.None,       GetLevel)},
                { "HP", new("Health Percent",   KeyType.Number,     KeyType.Number,     GetHealthPoints)}
            };
            #pragma warning restore IDE0055


            DataKeys = new(dataKeys);
        }

        private object GetLocalFlag(double? id, VmConditionData data)
        {
            if (id == null)
            {
                throw new InvalidOperationException();
            }

            return false;
        }

        private object GetLocalNumber(double? id, VmConditionData data)
        {
            if (id == null)
            {
                throw new InvalidOperationException();
            }

            return 0;
        }

        private object GetFlag(double? id, VmConditionData data)
        {
            if (id == null)
            {
                throw new InvalidOperationException();
            }

            return data.Data.Flags.TryGetValue((int)id.Value, out bool result) && result;
        }

        private object GetDynamicFlag(double? id, VmConditionData data)
        {
            if (id == null)
            {
                throw new InvalidOperationException();
            }

            return false;
        }

        private object GetRings(double? id, VmConditionData data)
        {
            if (id != null)
            {
                throw new InvalidOperationException();
            }

            return data.Data.Rings;
        }

        private object GetChao(double? id, VmConditionData data)
        {
            if (id != null)
                return data.Data.Chao.TryGetValue((int)id.Value, out var t) ? t.Count : 0;
            else
                return data.Data.Chao.Count;
        }

        private object GetChaoLevel(double? id, VmConditionData data)
        {
            if (id == null)
            {
                throw new InvalidOperationException();
            }

            return data.Data.Chao.TryGetValue((int)id.Value, out var t) ? t.Level : 0;
        }

        private object GetCard(double? id, VmConditionData data)
        {
            if (id != null)
                return data.Data.Cards.Contains((int)id.Value);
            else
                return data.Data.Cards.Count;
        }

        private object GetItem(double? id, VmConditionData data)
        {
            if (id != null)
                return data.Data.Items.TryGetValue((int)id.Value, out int result) ? result : 0;
            else
                return data.Data.Items.Count;
        }

        private object GetTeamMember(double? id, VmConditionData data)
        {
            if (id != null)
                return data.Data.TeamMembers.ContainsKey((int)id.Value);
            else
                return data.Data.TeamMembers.Count;
        }

        private object GetPartyMember(double? id, VmConditionData data)
        {
            if (id != null)
            {
                int index = (int)id;
                if(index >= data.Data.PartyMembers.Count)
                {
                    return -1;
                }
                return data.Data.PartyMembers[index];
            }
            else
                return data.Data.PartyMembers.ToArray();
        }
        private object GetEquipped(double? id, VmConditionData data)
        {
            if (id != null)
                return data.Data.TeamMembers.TryGetValue((int)id.Value, out TeamSlot? ts) ? ts.Equipment : Array.Empty<int>();
            else
                return data.Data.PartyMembers.SelectMany(x => data.Data.TeamMembers.TryGetValue(x, out TeamSlot? ts) ? ts.Equipment.ToArray() : Array.Empty<int>()).ToArray();
        }

        private object GetEquippedCount(double? id, VmConditionData data)
        {
            return ((int[])GetEquipped(id, data)).Length;
        }

        private object GetLevel(double? id, VmConditionData data)
        {
            if (id == null)
            {
                throw new InvalidOperationException();
            }

            return data.Data.TeamMembers.TryGetValue((int)id.Value, out TeamSlot? t) ? t.Level : 0;
        }

        private object GetHealthPoints(double? id, VmConditionData data)
        {
            if (id != null)
                return data.Data.TeamMembers.TryGetValue((int)id.Value, out TeamSlot? ts) ? (long)(ts.Health / (double)ts.MaxHealth * 100) : 0;
            else
            {
                long maxHealth = 0;
                long health = 0;
                foreach (var t in data.Data.PartyMembers)
                {
                    if (data.Data.TeamMembers.TryGetValue(t, out TeamSlot? ts))
                    {
                        maxHealth += ts.MaxHealth;
                        health += ts.Health;
                    }
                }
                return (long)(health / (double)maxHealth * 100);
            }
        }

    }
}
