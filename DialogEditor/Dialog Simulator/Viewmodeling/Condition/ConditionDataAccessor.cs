using SCR.Tools.Dialog.Data.Condition;
using SCR.Tools.DynamicDataExpression;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SCR.Tools.Dialog.Simulator.Viewmodeling.Condition
{
    public sealed class ConditionDataAccessor : IDataAccess<IReadOnlyConditionData>
    {
        public static readonly ConditionDataAccessor DA = new();

        public ReadOnlyDictionary<string, DataKey<IReadOnlyConditionData>> DataKeys { get; }

        private ConditionDataAccessor()
        {
            #pragma warning disable IDE0055
            Dictionary<string, DataKey<IReadOnlyConditionData>> dataKeys = new()
            {
                { "F",  new("Flag",         KeyType.Boolean,    KeyType.None,       GetFlag) }, // From flag storage
                { "D",  new("Dynamic Flag", KeyType.Boolean,    KeyType.None,       GetDynamicFlag) }, // Flag functions

                { "R",  new("Rings",        KeyType.None,       KeyType.Number,     GetRings) }, // Ring count
                { "A",  new("Chao",         KeyType.Number,     KeyType.Number,     GetChao) }, // Chao Count
                { "AL", new("Chao Level",   KeyType.Number,     KeyType.None,       GetChaoLevel) }, // Level of a chao
                { "C",  new("Card",         KeyType.Boolean,     KeyType.Number,     GetCard) }, // Whether a card is collected / Cards count
                { "I",  new("Item",         KeyType.Number,     KeyType.Number,     GetItem) }, // Item Count

                { "T",  new("Team Member",  KeyType.Boolean,     KeyType.Number,    GetTeamMember) }, // Whether team member is acquired / acquired team member count
                { "P",  new("Party Member", KeyType.Number,     KeyType.NumberList,    GetPartyMember) }, // Party member at index / Party member array
                { "E",  new("Equipped",     KeyType.NumberList, KeyType.NumberList, GetEquipped) }, // ID-less returns party equipment
                { "EC", new("Equipped Count", KeyType.Number,   KeyType.Number,     GetEquippedCount) }, // ID-less returns party equipment count
                { "L",  new("Level",        KeyType.Number,     KeyType.None,       GetLevel)},
                { "HP", new("Health Percent", KeyType.Number,    KeyType.Number,     GetHealthPoints)}
            };
            #pragma warning restore IDE0055


            DataKeys = new(dataKeys);
        }

        private object GetFlag(double? id, IReadOnlyConditionData data)
        {
            if (id == null)
            {
                throw new InvalidOperationException();
            }

            return data.Flags.TryGetValue((int)id.Value, out bool result) && result;
        }

        private object GetDynamicFlag(double? id, IReadOnlyConditionData data)
        {
            if (id == null)
            {
                throw new InvalidOperationException();
            }

            return false;
        }

        private object GetRings(double? id, IReadOnlyConditionData data)
        {
            if (id != null)
            {
                throw new InvalidOperationException();
            }

            return data.Rings;
        }

        private object GetChao(double? id, IReadOnlyConditionData data)
        {
            if (id != null)
                return data.Chao.TryGetValue((int)id.Value, out var t) ? t.Count : 0;
            else
                return data.Chao.Count;
        }

        private object GetChaoLevel(double? id, IReadOnlyConditionData data)
        {
            if (id == null)
            {
                throw new InvalidOperationException();
            }

            return data.Chao.TryGetValue((int)id.Value, out var t) ? t.Level : 0;
        }

        private object GetCard(double? id, IReadOnlyConditionData data)
        {
            if (id != null)
                return data.Cards.Contains((int)id.Value);
            else
                return data.Cards.Count;
        }

        private object GetItem(double? id, IReadOnlyConditionData data)
        {
            if (id != null)
                return data.Items.TryGetValue((int)id.Value, out int result) ? result : 0;
            else
                return data.Items.Count;
        }

        private object GetTeamMember(double? id, IReadOnlyConditionData data)
        {
            if (id != null)
                return data.TeamMembers.ContainsKey((int)id.Value);
            else
                return data.TeamMembers.Count;
        }

        private object GetPartyMember(double? id, IReadOnlyConditionData data)
        {
            if (id != null)
            {
                int index = (int)id;
                if(index >= data.PartyMembers.Count)
                {
                    return -1;
                }
                return data.PartyMembers[index];
            }
            else
                return data.PartyMembers.ToArray();
        }
        private object GetEquipped(double? id, IReadOnlyConditionData data)
        {
            if (id != null)
                return data.TeamMembers.TryGetValue((int)id.Value, out IReadOnlyTeamSlot? ts) ? ts.Equipment : Array.Empty<int>();
            else
                return data.PartyMembers.SelectMany(x => data.TeamMembers.TryGetValue(x, out IReadOnlyTeamSlot? ts) ? ts.Equipment.ToArray() : Array.Empty<int>()).ToArray();
        }

        private object GetEquippedCount(double? id, IReadOnlyConditionData data)
        {
            return ((int[])GetEquipped(id, data)).Length;
        }

        private object GetLevel(double? id, IReadOnlyConditionData data)
        {
            if (id == null)
            {
                throw new InvalidOperationException();
            }

            return data.TeamMembers.TryGetValue((int)id.Value, out IReadOnlyTeamSlot? t) ? t.Level : 0;
        }

        private object GetHealthPoints(double? id, IReadOnlyConditionData data)
        {
            if (id != null)
                return data.TeamMembers.TryGetValue((int)id.Value, out IReadOnlyTeamSlot? ts) ? (long)(ts.Health / (double)ts.MaxHealth * 100) : 0;
            else
            {
                long maxHealth = 0;
                long health = 0;
                foreach (var t in data.PartyMembers)
                {
                    if (data.TeamMembers.TryGetValue(t, out IReadOnlyTeamSlot? ts))
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
