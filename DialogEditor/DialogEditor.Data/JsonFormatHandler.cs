using SCR.Tools.UndoRedo;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;

namespace SCR.Tools.Dialog.Data
{
    public static class JsonFormatHandler
    {
        #region Dialog To Json

        public static string WriteDialog(this DialogContainer dialog, bool indenting)
        {
            using MemoryStream stream = new();
            using Utf8JsonWriter jsonWriter = new(stream, new()
            {
                Indented = indenting
            });

            WriteDialogContents(jsonWriter, dialog);

            jsonWriter.Flush();

            return Encoding.UTF8.GetString(stream.ToArray());
        }

        private static void WriteDialogContents(Utf8JsonWriter jsonWriter, DialogContainer dialog)
        {
            jsonWriter.WriteStartObject();

            jsonWriter.WriteString(nameof(DialogContainer.Author), dialog.Author);
            jsonWriter.WriteString(nameof(DialogContainer.Name), dialog.Name);
            jsonWriter.WriteString(nameof(DialogContainer.Description), dialog.Description);

            jsonWriter.WriteStartArray(nameof(DialogContainer.Nodes));

            foreach (Node node in dialog.Nodes)
            {
                WriteNode(jsonWriter, node, dialog);
            }

            jsonWriter.WriteEndArray();

            if (dialog.ConditionData.Count > 0)
            {
                jsonWriter.WriteStartArray(nameof(DialogContainer.ConditionData));

                foreach (Condition.ConditionData? t in dialog.ConditionData)
                {
                    Condition.JsonFormatHandler.WriteConditionDataContents(jsonWriter, t);
                }

                jsonWriter.WriteEndArray();
            }

            jsonWriter.WriteEndObject();
        }

        private static void WriteNode(Utf8JsonWriter jsonWriter, Node node, DialogContainer dialog)
        {
            jsonWriter.WriteStartObject();

            jsonWriter.WriteNumber(nameof(Node.LocationX), node.LocationX);
            jsonWriter.WriteNumber(nameof(Node.LocationY), node.LocationY);

            if (node.RightPortrait)
            {
                jsonWriter.WriteBoolean(nameof(Node.RightPortrait), node.RightPortrait);
            }

            jsonWriter.WriteStartArray(nameof(Node.Outputs));

            foreach (NodeOutput output in node.Outputs)
            {
                WriteNodeOutputs(jsonWriter, output, dialog);
            }

            jsonWriter.WriteEndArray();
            jsonWriter.WriteEndObject();
        }

        private static void WriteNodeOutputs(Utf8JsonWriter jsonWriter, NodeOutput output, DialogContainer dialog)
        {
            jsonWriter.WriteStartObject();

            if (!string.IsNullOrWhiteSpace(output.Expression))
            {
                jsonWriter.WriteString(nameof(NodeOutput.Expression), output.Expression);
            }

            if (!string.IsNullOrWhiteSpace(output.Character))
            {
                jsonWriter.WriteString(nameof(NodeOutput.Character), output.Character);
            }

            if (!string.IsNullOrWhiteSpace(output.Icon))
            {
                jsonWriter.WriteString(nameof(NodeOutput.Icon), output.Icon);
            }

            jsonWriter.WriteString(nameof(NodeOutput.Text), output.Text);

            if (output.Fallback)
            {
                jsonWriter.WriteBoolean(nameof(NodeOutput.Fallback), output.Fallback);
            }

            if (output.DisableReuse)
            {
                jsonWriter.WriteBoolean(nameof(NodeOutput.DisableReuse), output.DisableReuse);
            }

            if (output.SharedDisabledIndex >= 0)
            {
                jsonWriter.WriteNumber(nameof(NodeOutput.SharedDisabledIndex), output.SharedDisabledIndex);
            }

            if (!string.IsNullOrWhiteSpace(output.Condition))
            {
                jsonWriter.WriteString(nameof(NodeOutput.Condition), output.Condition);
            }

            if (output.Connected != null)
            {
                jsonWriter.WriteNumber(nameof(NodeOutput.Connected), dialog.Nodes.IndexOf(output.Connected));
            }

            jsonWriter.WriteEndObject();
        }

        #endregion

        #region Json To Dialog

        public static DialogContainer ReadDialog(string text)
        {
            ChangeTracker prev = GlobalChangeTracker;

            new ChangeTracker().Use();
            BeginChangeGroup();

            DialogContainer result = new();

            try
            {
                JsonNode? json = JsonNode.Parse(text);
                if (json == null)
                {
                    throw new ArgumentException("Format not a valid json object");
                }

                ReadDialogContents(json, result);
            }
            finally
            {
                EndChangeGroup();
                prev.Use();
            }

            return result;
        }

        private static void ReadDialogContents(JsonNode json, DialogContainer output)
        {
            output.Author = json[nameof(DialogContainer.Author)]?.GetValue<string>() ?? "";
            output.Name = json[nameof(DialogContainer.Name)]?.GetValue<string>() ?? "";
            output.Description = json[nameof(DialogContainer.Description)]?.GetValue<string>() ?? "";

            Dictionary<NodeOutput, int> outputMap = new();

            if (json[nameof(DialogContainer.Nodes)] is JsonArray jsonNodes)
            {
                foreach (JsonNode? jsonNode in jsonNodes)
                {
                    Node node = output.CreateNode();
                    ReadNode(jsonNode ?? throw new InvalidDataException("Jsonnode is null!"), node, outputMap);
                }
            }
            else
            {
                throw new InvalidDataException("No nodes in dialog!");
            }

            // assigning the outputs
            foreach (KeyValuePair<NodeOutput, int> pair in outputMap)
            {
                if (pair.Value >= 0)
                {
                    pair.Key.Connect(output.Nodes[pair.Value]);
                }
            }

            if (json[nameof(DialogContainer.ConditionData)] is JsonArray jsonConditions)
            {
                foreach (JsonNode? jsonNode in jsonConditions)
                {
                    output.ConditionData.Add(
                        Condition.JsonFormatHandler.ReadConditionData(
                            jsonNode ?? throw new InvalidDataException("Jsonnode is null!")));
                }
            }
        }

        private static void ReadNode(JsonNode json, Node output, Dictionary<NodeOutput, int> outputMap)
        {
            output.LocationX = json[nameof(Node.LocationX)]?.GetValue<int>() ?? 0;
            output.LocationY = json[nameof(Node.LocationY)]?.GetValue<int>() ?? 0;
            output.RightPortrait = json[nameof(Node.RightPortrait)]?.GetValue<bool>() ?? false;

            if (json[nameof(Node.Outputs)] is JsonArray jsonNodes)
            {
                foreach (JsonNode? jsonNode in jsonNodes)
                {
                    NodeOutput nOut = output.CreateOutput();
                    ReadNodeOutput(jsonNode ?? throw new InvalidDataException("Jsonnode is null!"), nOut, outputMap);
                }
            }
            else
            {
                throw new InvalidDataException("One or more nodes have no output!");
            }

            // removing the automatically created output
            output.RemoveOutput(output.Outputs[0]);
        }

        private static void ReadNodeOutput(JsonNode json, NodeOutput output, Dictionary<NodeOutput, int> outputMap)
        {
            output.Expression = json[nameof(NodeOutput.Expression)]?.GetValue<string>() ?? "";
            output.Character = json[nameof(NodeOutput.Character)]?.GetValue<string>() ?? "";
            output.Icon = json[nameof(NodeOutput.Icon)]?.GetValue<string>() ?? "";
            output.Text = json[nameof(NodeOutput.Text)]?.GetValue<string>() ?? "";
            output.Fallback = json[nameof(NodeOutput.Fallback)]?.GetValue<bool>() ?? false;
            output.DisableReuse = json[nameof(NodeOutput.DisableReuse)]?.GetValue<bool>() ?? false;
            output.SharedDisabledIndex = json[nameof(NodeOutput.SharedDisabledIndex)]?.GetValue<int>() ?? -1;
            output.Condition = json[nameof(NodeOutput.Condition)]?.GetValue<string>() ?? "";

            int outIndex = json[nameof(NodeOutput.Connected)]?.GetValue<int>() ?? -1;
            outputMap.Add(output, outIndex);
        }

        #endregion

        #region Dialog Options to Json

        public static string WriteDialogOptions(this DialogOptions options, bool indenting, string? basePath = null)
        {
            using MemoryStream stream = new();
            using Utf8JsonWriter jsonWriter = new(stream, new()
            {
                Indented = indenting
            });

            jsonWriter.WriteStartObject();

            string? portraitPath = options.PortraitsPath;

            if (basePath != null && portraitPath != null)
            {
                portraitPath = Path.GetRelativePath(basePath, portraitPath);
            }

            jsonWriter.WriteString(nameof(DialogOptions.PortraitsPath), portraitPath);

            WriteDialogNodeOptions(jsonWriter, nameof(DialogOptions.CharacterOptions), options.CharacterOptions);
            WriteDialogNodeOptions(jsonWriter, nameof(DialogOptions.ExpressionOptions), options.ExpressionOptions);
            WriteDialogIcons(jsonWriter, options.NodeIcons, basePath);

            jsonWriter.WriteEndObject();

            jsonWriter.Flush();

            return Encoding.UTF8.GetString(stream.ToArray());
        }

        private static void WriteDialogNodeOptions(Utf8JsonWriter jsonWriter, string name, IDictionary<string, Color> options)
        {
            jsonWriter.WriteStartObject(name);

            foreach (KeyValuePair<string, Color> option in options)
            {
                string colorHex = option.Value.ToArgb().ToString("X8");
                jsonWriter.WriteString(option.Key, colorHex);
            }

            jsonWriter.WriteEndObject();
        }

        private static void WriteDialogIcons(Utf8JsonWriter jsonWriter, IDictionary<string, string> iconPaths, string? basePath)
        {
            jsonWriter.WriteStartObject(nameof(DialogOptions.NodeIcons));

            foreach (KeyValuePair<string, string> icon in iconPaths)
            {
                string path = icon.Value;

                if (basePath != null)
                {
                    path = Path.GetRelativePath(basePath, path);
                }

                jsonWriter.WriteString(icon.Key, path);
            }

            jsonWriter.WriteEndObject();
        }

        #endregion

        #region Json to Dialog Options

        public static DialogOptions ReadDialogOptions(string text, string? basePath = null)
        {
            ChangeTracker prev = GlobalChangeTracker;

            new ChangeTracker().Use();
            BeginChangeGroup();

            DialogOptions result = new();

            try
            {
                JsonNode? json = JsonNode.Parse(text);
                if (json == null)
                {
                    throw new ArgumentException("Format not a valid json object");
                }

                string? portraitsPath = json[nameof(DialogOptions.PortraitsPath)]?.GetValue<string?>();

                if (portraitsPath != null && basePath != null)
                {
                    portraitsPath = Path.GetFullPath(portraitsPath, basePath);
                }

                result.PortraitsPath = portraitsPath;

                if (json[nameof(DialogOptions.CharacterOptions)] is JsonNode options)
                {
                    ReadDialogNodeOptions(options, result.CharacterOptions);
                }

                if (json[nameof(DialogOptions.ExpressionOptions)] is JsonNode expressions)
                {
                    ReadDialogNodeOptions(expressions, result.ExpressionOptions);
                }

                if (json[nameof(DialogOptions.NodeIcons)] is JsonNode icons)
                {
                    ReadDialogIcons(icons, result.NodeIcons, basePath);
                }
            }
            finally
            {
                EndChangeGroup();
                prev.Use();
            }

            return result;
        }

        private static void ReadDialogNodeOptions(JsonNode json, IDictionary<string, Color> output)
        {
            foreach (KeyValuePair<string, JsonNode?> p in json.AsObject())
            {
                string? colorHex = p.Value?.GetValue<string>();
                if (colorHex == null)
                {
                    throw new InvalidDataException("Color not valid");
                }

                int colorHexNum = int.Parse(colorHex, System.Globalization.NumberStyles.HexNumber);
                Color col = Color.FromArgb(colorHexNum);

                output.Add(p.Key, col);
            }
        }

        private static void ReadDialogIcons(JsonNode json, IDictionary<string, string> output, string? basepath)
        {
            foreach (KeyValuePair<string, JsonNode?> p in json.AsObject())
            {
                string? path = p.Value?.GetValue<string>();

                if (path == null)
                {
                    throw new InvalidDataException("Path not valid");
                }

                if (basepath != null)
                {
                    path = Path.GetFullPath(path, basepath);
                }

                output.Add(p.Key, path);
            }
        }

        #endregion
    }
}
