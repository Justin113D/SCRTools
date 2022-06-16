using SCR.Tools.UndoRedo;
using System.Drawing;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SCR.Tools.DialogEditor.Data
{
    public static class JsonFormatHandler
    {
        #region Dialog To Json

        public static string WriteDialog(this Dialog dialog, bool indenting)
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

        private static void WriteDialogContents(Utf8JsonWriter jsonWriter, Dialog dialog)
        {
            jsonWriter.WriteStartObject();

            jsonWriter.WriteString(nameof(Dialog.Author), dialog.Author);
            jsonWriter.WriteString(nameof(Dialog.Name), dialog.Name);
            jsonWriter.WriteString(nameof(Dialog.Description), dialog.Description);

            jsonWriter.WriteStartArray(nameof(Dialog.Nodes));

            foreach (Node node in dialog.Nodes)
            {
                WriteNode(jsonWriter, node, dialog);
            }

            jsonWriter.WriteEndArray();
            jsonWriter.WriteEndObject();
        }

        private static void WriteNode(Utf8JsonWriter jsonWriter, Node node, Dialog dialog)
        {
            jsonWriter.WriteStartObject();

            jsonWriter.WriteNumber(nameof(Node.LocationX), node.LocationX);
            jsonWriter.WriteNumber(nameof(Node.LocationY), node.LocationY);
            jsonWriter.WriteBoolean(nameof(Node.RightPortrait), node.RightPortrait);

            jsonWriter.WriteStartArray(nameof(Dialog.Nodes));

            foreach (NodeOutput output in node.Outputs)
            {
                WriteNodeOutputs(jsonWriter, output, dialog);
            }

            jsonWriter.WriteEndArray();
            jsonWriter.WriteEndObject();
        }

        private static void WriteNodeOutputs(Utf8JsonWriter jsonWriter, NodeOutput output, Dialog dialog)
        {
            jsonWriter.WriteStartObject();

            jsonWriter.WriteString(nameof(NodeOutput.Expression), output.Expression);
            jsonWriter.WriteString(nameof(NodeOutput.Character), output.Character);
            jsonWriter.WriteString(nameof(NodeOutput.Icon), output.Icon);
            jsonWriter.WriteString(nameof(NodeOutput.Text), output.Text);
            jsonWriter.WriteBoolean(nameof(NodeOutput.KeepEnabled), output.KeepEnabled);
            jsonWriter.WriteString(nameof(NodeOutput.Condition), output.Condition);
            jsonWriter.WriteNumber(nameof(NodeOutput.Event), output.Event);
            jsonWriter.WriteNumber(nameof(NodeOutput.Output),
                output.Output == null ? -1 : dialog.Nodes.IndexOf(output.Output));

            jsonWriter.WriteEndObject();
        }

        #endregion

        #region Json To Dialog

        public static Dialog ReadDialog(string text)
        {
            ChangeTracker prev = ChangeTracker.Global;

            new ChangeTracker().Use();
            ChangeTracker.Global.BeginGroup();

            Dialog result = new();

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
                ChangeTracker.Global.EndGroup();
                prev.Use();
            }

            return result;
        }

        private static void ReadDialogContents(JsonNode json, Dialog output)
        {
            output.Author = json[nameof(Dialog.Author)]?.GetValue<string>() ?? "";
            output.Name = json[nameof(Dialog.Name)]?.GetValue<string>() ?? "";
            output.Description = json[nameof(Dialog.Description)]?.GetValue<string>() ?? "";

            Dictionary<NodeOutput, int> outputMap = new();

            if (json[nameof(Dialog.Nodes)] is JsonArray jsonNodes)
            {
                foreach(JsonNode? jsonNode in jsonNodes)
                {
                    Node node = output.CreateNode();
                    ReadNode(jsonNode ?? throw new InvalidDataException("Jsonnode is null!"), node, outputMap);
                }
            }

            // assigning the outputs
            foreach(KeyValuePair<NodeOutput, int> pair in outputMap)
            {
                if(pair.Value >= 0)
                {
                    pair.Key.SetOutput(output.Nodes[pair.Value]);
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

            // removing the automatically created output
            output.RemoveOutput(output.Outputs[0]);
        }

        private static void ReadNodeOutput(JsonNode json, NodeOutput output, Dictionary<NodeOutput, int> outputMap)
        {
            output.Expression = json[nameof(NodeOutput.Expression)]?.GetValue<string>() ?? "";
            output.Character = json[nameof(NodeOutput.Character)]?.GetValue<string>() ?? "";
            output.Icon = json[nameof(NodeOutput.Icon)]?.GetValue<string>() ?? "";
            output.Text = json[nameof(NodeOutput.Text)]?.GetValue<string>() ?? "";
            output.KeepEnabled = json[nameof(NodeOutput.KeepEnabled)]?.GetValue<bool>() ?? false;
            output.Event = json[nameof(NodeOutput.Event)]?.GetValue<int>() ?? 0;

            string condition = json[nameof(NodeOutput.Condition)]?.GetValue<string>() ?? "";
            if(!output.SetCondition(condition))
            {
                throw new InvalidDataException($"Output condition \"{condition}\" invalid!");
            }

            int outIndex = json[nameof(NodeOutput.Output)]?.GetValue<int>() ?? -1;
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

            WriteDialogNodeOptions(jsonWriter, nameof(DialogOptions.CharacterOptions), options.CharacterOptions);
            WriteDialogNodeOptions(jsonWriter, nameof(DialogOptions.ExpressionOptions), options.ExpressionOptions);
            WriteDialogIcons(jsonWriter, options.NodeIcons, basePath);

            jsonWriter.WriteEndObject();

            jsonWriter.Flush();

            return Encoding.UTF8.GetString(stream.ToArray());
        }

        private static void WriteDialogNodeOptions(Utf8JsonWriter jsonWriter, string name, Dictionary<string, Color> options)
        {
            jsonWriter.WriteStartObject(name);

            foreach(KeyValuePair<string, Color> option in options)
            {
                string colorHex = option.Value.ToArgb().ToString("X8");
                jsonWriter.WriteString(option.Key, colorHex);
            }

            jsonWriter.WriteEndObject();
        }

        private static void WriteDialogIcons(Utf8JsonWriter jsonWriter, Dictionary<string, string> iconPaths, string? basePath)
        {
            jsonWriter.WriteStartObject(nameof(DialogOptions.NodeIcons));

            foreach (KeyValuePair<string, string> icon in iconPaths)
            {
                string path = icon.Value;

                if(basePath != null)
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
            ChangeTracker prev = ChangeTracker.Global;

            new ChangeTracker().Use();
            ChangeTracker.Global.BeginGroup();

            DialogOptions result = new();

            try
            {
                JsonNode? json = JsonNode.Parse(text);
                if (json == null)
                {
                    throw new ArgumentException("Format not a valid json object");
                }

                if(json[nameof(DialogOptions.CharacterOptions)] is JsonNode options)
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
                ChangeTracker.Global.EndGroup();
                prev.Use();
            }

            return result;
        }

        private static void ReadDialogNodeOptions(JsonNode json, Dictionary<string, Color> output)
        {
            foreach(KeyValuePair<string, JsonNode?> p in json.AsObject())
            {
                string? colorHex = p.Value?.GetValue<string>();
                if(colorHex == null)
                {
                    throw new InvalidDataException("Color not valid");
                }

                int colorHexNum = int.Parse(colorHex, System.Globalization.NumberStyles.HexNumber);
                Color col = Color.FromArgb(colorHexNum);

                output.Add(p.Key, col);
            }
        }

        private static void ReadDialogIcons(JsonNode json, Dictionary<string, string> output, string? basepath)
        {
            foreach (KeyValuePair<string, JsonNode?> p in json.AsObject())
            {
                string? path = p.Value?.GetValue<string>();

                if (path == null)
                {
                    throw new InvalidDataException("Path not valid");
                }

                if(basepath != null)
                {
                    path = Path.GetFullPath(path, basepath);
                }

                output.Add(p.Key, path);
            }
        }

        #endregion
    }
}
