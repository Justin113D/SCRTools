using SCR.Tools.UndoRedo;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SCR.Tools.DialogEditor.Data
{
    public static class JsonFormatHandler
    {
        #region To Json

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

        #region From Json

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

            if (json[nameof(Dialog.Nodes)] is JsonArray jsonNodes)
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
    }
}
