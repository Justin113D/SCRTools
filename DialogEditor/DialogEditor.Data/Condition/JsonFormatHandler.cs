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

        internal static void WriteConditionDataContents(Utf8JsonWriter jsonWriter, ConditionData data)
        {
            jsonWriter.WriteStartObject();

            if (data.Rings > 0)
            {
                jsonWriter.WriteNumber(nameof(ConditionData.Rings), data.Rings);
            }

            WriteFlags(jsonWriter, nameof(ConditionData.Flags), data.Flags);
            WriteFlags(jsonWriter, nameof(ConditionData.DynamicFlags), data.DynamicFlags);
            WriteItems(jsonWriter, data.Items);
            WriteChao(jsonWriter, data.Chao);
            WriteIntCollection(jsonWriter, nameof(ConditionData.Cards), data.Cards);
            WriteTeamMembers(jsonWriter, data.TeamMembers);
            WriteIntCollection(jsonWriter, nameof(ConditionData.PartyMembers), data.PartyMembers);

            jsonWriter.WriteEndObject();
        }

        private static void WriteFlags(Utf8JsonWriter jsonWriter, string name, IDictionary<int, bool> flags)
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

        private static void WriteItems(Utf8JsonWriter jsonWriter, IDictionary<int, int> items)
        {
            if (items.Count == 0)
            {
                return;
            }

            jsonWriter.WriteStartObject(nameof(ConditionData.Items));

            foreach (KeyValuePair<int, int> flag in items)
            {
                jsonWriter.WriteNumber(flag.Key.ToString(), flag.Value);
            }

            jsonWriter.WriteEndObject();
        }

        private static void WriteChao(Utf8JsonWriter jsonWriter, IDictionary<int, ChaoSlot> chao)
        {
            if (chao.Count == 0)
            {
                return;
            }

            jsonWriter.WriteStartObject(nameof(ConditionData.Chao));

            foreach (KeyValuePair<int, ChaoSlot> slot in chao)
            {
                jsonWriter.WriteStartObject(slot.Key.ToString());

                jsonWriter.WriteNumber(nameof(ChaoSlot.Count), slot.Value.Count);
                jsonWriter.WriteNumber(nameof(ChaoSlot.Level), slot.Value.Level);

                jsonWriter.WriteEndObject();
            }

            jsonWriter.WriteEndObject();
        }

        private static void WriteTeamMembers(Utf8JsonWriter jsonWriter, IDictionary<int, TeamSlot> teamMembers)
        {
            if (teamMembers.Count == 0)
            {
                return;
            }

            jsonWriter.WriteStartObject(nameof(ConditionData.TeamMembers));

            foreach (KeyValuePair<int, TeamSlot> slot in teamMembers)
            {
                jsonWriter.WriteStartObject(slot.Key.ToString());

                jsonWriter.WriteNumber(nameof(TeamSlot.Level), slot.Value.Level);
                jsonWriter.WriteNumber(nameof(TeamSlot.Health), slot.Value.Health);
                jsonWriter.WriteNumber(nameof(TeamSlot.MaxHealth), slot.Value.MaxHealth);
                jsonWriter.WriteNumber(nameof(TeamSlot.PowerPoints), slot.Value.PowerPoints);
                jsonWriter.WriteNumber(nameof(TeamSlot.MaxPowerPoints), slot.Value.MaxPowerPoints);

                WriteIntCollection(jsonWriter, nameof(TeamSlot.Equipment), slot.Value.Equipment);

                jsonWriter.WriteEndObject();
            }

            jsonWriter.WriteEndObject();
        }

        private static void WriteIntCollection(Utf8JsonWriter jsonWriter, string name, ICollection<int> collection)
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

        internal static ConditionData ReadConditionData(JsonNode node)
        {
            ConditionData result = new()
            {
                Rings = node[nameof(ConditionData.Rings)]?.GetValue<int>() ?? 0
            };

            ReadFlags(node, nameof(ConditionData.Flags), result.Flags);
            ReadFlags(node, nameof(ConditionData.DynamicFlags), result.DynamicFlags);
            ReadItems(node, result);
            ReadChao(node, result);
            ReadIntCollection(node, nameof(ConditionData.Cards), result.Cards);
            ReadTeamMembers(node, result);
            ReadIntCollection(node, nameof(ConditionData.PartyMembers), result.PartyMembers);

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

        private static void ReadFlags(JsonNode parent, string name, IDictionary<int, bool> output)
        {
            LoopJsonDictionary(parent, name, (id, node) =>
                output.Add(id, node.GetValue<bool>()));
        }

        private static void ReadItems(JsonNode parent, ConditionData output)
        {
            LoopJsonDictionary(parent, nameof(ConditionData.Items), (id, node) =>
                output.Items.Add(id, node.GetValue<int>()));
        }

        private static void ReadChao(JsonNode parent, ConditionData output)
        {
            LoopJsonDictionary(parent, nameof(ConditionData.Chao), (id, node) =>
            {
                ChaoSlot slot = new()
                {
                    Count = node[nameof(ChaoSlot.Count)]?.GetValue<int>() ?? 0,
                    Level = node[nameof(ChaoSlot.Level)]?.GetValue<int>() ?? 0
                };

                output.Chao.Add(id, slot);
            });
        }

        private static void ReadTeamMembers(JsonNode parent, ConditionData output)
        {
            LoopJsonDictionary(parent, nameof(ConditionData.TeamMembers), (id, node) =>
            {
                TeamSlot slot = new()
                {
                    Level = node[nameof(TeamSlot.Level)]?.GetValue<int>() ?? 0,
                    Health = node[nameof(TeamSlot.Health)]?.GetValue<int>() ?? 0,
                    MaxHealth = node[nameof(TeamSlot.MaxHealth)]?.GetValue<int>() ?? 0,
                    PowerPoints = node[nameof(TeamSlot.PowerPoints)]?.GetValue<int>() ?? 0,
                    MaxPowerPoints = node[nameof(TeamSlot.MaxPowerPoints)]?.GetValue<int>() ?? 0,
                };

                ReadIntCollection(node, nameof(TeamSlot.Equipment), slot.Equipment);

                output.TeamMembers.Add(id, slot);
            });
        }

        #endregion
    }
}
