using SCR.Tools.Dialog.Data.Condition.ReadOnly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SCR.Tools.Dialog.Data.Condition
{
    public static class JsonFormatHandler
    {
        #region Condition Data to Json

        internal static void WriteConditionDataContents(Utf8JsonWriter jsonWriter, IReadOnlyConditionData data)
        {
            jsonWriter.WriteStartObject();

            if (data.Rings > 0)
            {
                jsonWriter.WriteNumber(nameof(IReadOnlyConditionData.Rings), data.Rings);
            }

            WriteFlags(jsonWriter, nameof(IReadOnlyConditionData.Flags), data.Flags);
            WriteFlags(jsonWriter, nameof(IReadOnlyConditionData.DynamicFlags), data.DynamicFlags);
            WriteItems(jsonWriter, data.Items);
            WriteChao(jsonWriter, data.Chao);
            WriteIntCollection(jsonWriter, nameof(IReadOnlyConditionData.Cards), data.Cards);
            WriteTeamMembers(jsonWriter, data.TeamMembers);
            WriteIntCollection(jsonWriter, nameof(IReadOnlyConditionData.PartyMembers), data.PartyMembers);

            jsonWriter.WriteEndObject();
        }

        private static void WriteFlags(Utf8JsonWriter jsonWriter, string name, IReadOnlyDictionary<int, bool> flags)
        {
            if (flags.Count == 0)
            {
                return;
            }

            jsonWriter.WriteStartObject(name);

            foreach (KeyValuePair<int, bool> flag in flags)
            {
                jsonWriter.WriteBoolean(flag.Key.ToString(), flag.Value);
            }

            jsonWriter.WriteEndObject();
        }

        private static void WriteItems(Utf8JsonWriter jsonWriter, IReadOnlyDictionary<int, int> items)
        {
            if (items.Count == 0)
            {
                return;
            }

            jsonWriter.WriteStartObject(nameof(IReadOnlyConditionData.Items));

            foreach (KeyValuePair<int, int> flag in items)
            {
                jsonWriter.WriteNumber(flag.Key.ToString(), flag.Value);
            }

            jsonWriter.WriteEndObject();
        }

        private static void WriteChao(Utf8JsonWriter jsonWriter, IReadOnlyDictionary<int, IReadOnlyChaoSlot> chao)
        {
            if (chao.Count == 0)
            {
                return;
            }

            jsonWriter.WriteStartObject(nameof(IReadOnlyConditionData.Chao));

            foreach (KeyValuePair<int, IReadOnlyChaoSlot> slot in chao)
            {
                jsonWriter.WriteStartObject(slot.Key.ToString());

                jsonWriter.WriteNumber(nameof(IReadOnlyChaoSlot.Count), slot.Value.Count);
                jsonWriter.WriteNumber(nameof(IReadOnlyChaoSlot.Level), slot.Value.Level);

                jsonWriter.WriteEndObject();
            }

            jsonWriter.WriteEndObject();
        }

        private static void WriteTeamMembers(Utf8JsonWriter jsonWriter, IReadOnlyDictionary<int, IReadOnlyTeamSlot> teamMembers)
        {
            if (teamMembers.Count == 0)
            {
                return;
            }

            jsonWriter.WriteStartObject(nameof(IReadOnlyConditionData.TeamMembers));

            foreach (KeyValuePair<int, IReadOnlyTeamSlot> slot in teamMembers)
            {
                jsonWriter.WriteStartObject(slot.Key.ToString());

                jsonWriter.WriteNumber(nameof(IReadOnlyTeamSlot.Level), slot.Value.Level);
                jsonWriter.WriteNumber(nameof(IReadOnlyTeamSlot.Health), slot.Value.Health);
                jsonWriter.WriteNumber(nameof(IReadOnlyTeamSlot.MaxHealth), slot.Value.MaxHealth);
                jsonWriter.WriteNumber(nameof(IReadOnlyTeamSlot.PowerPoints), slot.Value.PowerPoints);
                jsonWriter.WriteNumber(nameof(IReadOnlyTeamSlot.MaxPowerPoints), slot.Value.MaxPowerPoints);

                WriteIntCollection(jsonWriter, nameof(IReadOnlyTeamSlot.Equipment), slot.Value.Equipment);

                jsonWriter.WriteEndObject();
            }

            jsonWriter.WriteEndObject();
        }

        private static void WriteIntCollection(Utf8JsonWriter jsonWriter, string name, IReadOnlyCollection<int> collection)
        {
            bool notEmpty = false;
            foreach (int i in collection)
            {
                notEmpty = true;
            }
            if (!notEmpty)
            {
                return;
            }

            jsonWriter.WriteStartArray(name);

            foreach (int equipment in collection)
            {
                jsonWriter.WriteNumberValue(equipment);
            }

            jsonWriter.WriteEndArray();

        }

        #endregion

        #region Json to ConditionData

        internal static ReadOnlyConditionData ReadConditionData(JsonNode node)
        {
            ReadOnlyConditionData result = new()
            {
                Rings = node[nameof(IReadOnlyConditionData.Rings)]?.GetValue<int>() ?? 0
            };

            ReadFlags(node, nameof(IReadOnlyConditionData.Flags), result._flags);
            ReadFlags(node, nameof(IReadOnlyConditionData.DynamicFlags), result._dynamicFlags);
            ReadItems(node, result);
            ReadChao(node, result);
            ReadIntCollection(node, nameof(IReadOnlyConditionData.Cards), result._cards);
            ReadTeamMembers(node, result);
            ReadIntCollection(node, nameof(IReadOnlyConditionData.PartyMembers), result._partyMembers);

            return result;
        }

        private static void LoopJsonDictionary(JsonNode parent, string name, Action<int, JsonNode> action)
        {
            if (parent[name] is not JsonNode dictionary)
            {
                return;
            }

            foreach (KeyValuePair<string, JsonNode?> entry in dictionary.AsObject())
            {
                if (entry.Value == null)
                {
                    throw new InvalidDataException("Jsonnode is null!");
                }

                if (!int.TryParse(entry.Key, out int key))
                {
                    throw new InvalidDataException("Dictionary key is not a number!;");
                }

                action(key, entry.Value);
            }
        }

        private static void ReadIntCollection(JsonNode parent, string name, ICollection<int> output)
        {
            if (parent[name] is not JsonArray array)
            {
                return;
            }

            foreach (JsonNode? item in array)
            {
                if (item == null)
                {
                    throw new InvalidDataException("Jsonnode is null!");
                }

                output.Add(item.GetValue<int>());
            }
        }

        private static void ReadFlags(JsonNode parent, string name, Dictionary<int, bool> output)
        {
            LoopJsonDictionary(parent, name, (id, node) =>
                output.Add(id, node.GetValue<bool>()));
        }

        private static void ReadItems(JsonNode parent, ReadOnlyConditionData output)
        {
            LoopJsonDictionary(parent, nameof(IReadOnlyConditionData.Items), (id, node) =>
                output._items.Add(id, node.GetValue<int>()));
        }

        private static void ReadChao(JsonNode parent, ReadOnlyConditionData output)
        {
            LoopJsonDictionary(parent, nameof(IReadOnlyConditionData.Chao), (id, node) =>
            {
                ReadOnlyChaoSlot slot = new()
                {
                    Count = node[nameof(IReadOnlyChaoSlot.Count)]?.GetValue<int>() ?? 0,
                    Level = node[nameof(IReadOnlyChaoSlot.Level)]?.GetValue<int>() ?? 0
                };

                output._chao.Add(id, slot);
            });
        }

        private static void ReadTeamMembers(JsonNode parent, ReadOnlyConditionData output)
        {
            LoopJsonDictionary(parent, nameof(IReadOnlyConditionData.TeamMembers), (id, node) =>
            {
                ReadOnlyTeamSlot slot = new()
                {
                    Level = node[nameof(IReadOnlyTeamSlot.Level)]?.GetValue<int>() ?? 0,
                    Health = node[nameof(IReadOnlyTeamSlot.Health)]?.GetValue<int>() ?? 0,
                    MaxHealth = node[nameof(IReadOnlyTeamSlot.MaxHealth)]?.GetValue<int>() ?? 0,
                    PowerPoints = node[nameof(IReadOnlyTeamSlot.PowerPoints)]?.GetValue<int>() ?? 0,
                    MaxPowerPoints = node[nameof(IReadOnlyTeamSlot.MaxPowerPoints)]?.GetValue<int>() ?? 0,
                };

                ReadIntCollection(node, nameof(IReadOnlyTeamSlot.Equipment), slot._equipment);

                output._teamMembers.Add(id, slot);
            });
        }

        #endregion
    }
}
