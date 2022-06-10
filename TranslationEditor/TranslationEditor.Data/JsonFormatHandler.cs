using SCR.Tools.UndoRedo;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SCR.Tools.TranslationEditor.Data
{
    public static class JsonFormatHandler
    {
        private const string _defaultLanguage = "DefaultLanguage";

        #region To Json

        public static string WriteFormat(this HeaderNode header, bool indenting)
        {
            using MemoryStream stream = new();
            using Utf8JsonWriter jsonWriter = new(stream, new()
            {
                Indented = indenting
            });

            jsonWriter.WriteStartObject();
            jsonWriter.WriteString(nameof(HeaderNode.Name), header.Name);
            jsonWriter.WriteString(_defaultLanguage, header.Language);
            jsonWriter.WriteString(nameof(HeaderNode.Author), header.Author);

            jsonWriter.WriteStartArray(nameof(HeaderNode.Versions));
            foreach (Version version in header.Versions)
            {
                jsonWriter.WriteStringValue(version.ToString());
            }
            jsonWriter.WriteEndArray();

            WriteChildNodes(jsonWriter, header.ChildNodes);

            jsonWriter.WriteEndObject();

            jsonWriter.Flush();

            return Encoding.UTF8.GetString(stream.ToArray());
        }

        private static void WriteChildNodes(Utf8JsonWriter jsonWriter, IList<Node> nodes)
        {
            jsonWriter.WriteStartArray(nameof(ParentNode.ChildNodes));
            foreach (Node n in nodes)
            {
                WriteNode(jsonWriter, n);
            }
            jsonWriter.WriteEndArray();
        }

        private static void WriteNode(Utf8JsonWriter jsonWriter, Node node)
        {
            jsonWriter.WriteStartObject();
            jsonWriter.WriteString(nameof(Node.Name), node.Name);
            if (node.Description != null)
            {
                jsonWriter.WriteString(nameof(Node.Description), node.Description);
            }

            if (node is StringNode stringNode)
            {
                jsonWriter.WriteString(nameof(StringNode.DefaultValue), stringNode.DefaultValue);
                if (stringNode.VersionIndex != 0)
                {
                    jsonWriter.WriteNumber(nameof(StringNode.VersionIndex), stringNode.VersionIndex);
                }
            }
            else if (node is ParentNode parent)
            {
                WriteChildNodes(jsonWriter, parent.ChildNodes);
            }
            else
            {
                throw new InvalidDataException($"Node is no a parent or stringnode! Node is \"{node.GetType()}\"");
            }
            jsonWriter.WriteEndObject();
        }

        #endregion

        #region From Json

        public static HeaderNode ReadFormat(string text)
        {
            ChangeTracker prev = ChangeTracker.Global;

            new ChangeTracker().Use();
            ChangeTracker.Global.BeginGroup();

            HeaderNode result = new();

            try
            {
                JsonNode? json = JsonNode.Parse(text);
                if (json == null)
                {
                    throw new ArgumentException("Format not a valid json object");
                }

                // reading format, language and author
                JsonNode? formatName = json[nameof(HeaderNode.Name)];
                if (formatName == null)
                    throw new InvalidDataException("Format cant be null");

                result.Name = formatName.GetValue<string>();
                result.Language = json[_defaultLanguage]?.GetValue<string>() ?? "";
                result.Author = json[nameof(HeaderNode.Author)]?.GetValue<string>() ?? "";

                // reading versions
                if (json[nameof(HeaderNode.Versions)] is not JsonArray jsonVersions)
                    throw new InvalidDataException("Header has no versions!");

                Version[] versions = new Version[jsonVersions.Count];
                for (int i = 0; i < versions.Length; i++)
                {
                    JsonNode? jsonVersion = jsonVersions[i];
                    if (jsonVersion == null)
                        throw new InvalidDataException("Version cant be null");

                    versions[i] = new(jsonVersion.GetValue<string>());
                }
                result.SetVersions(versions);

                // reading the child node treess
                Node[] childNodes = ReadChildNodes(json);
                foreach (Node node in childNodes)
                {
                    result.AddChildNode(node, false);
                }
            }
            finally
            {
                ChangeTracker.Global.EndGroup();
                prev.Use();
            }

            return result;
        }

        private static Node ReadNode(JsonNode json)
        {
            if (json[nameof(ParentNode.ChildNodes)] == null)
            {
                return ReadStringNode(json);
            }
            else
            {
                return ReadParentNode(json);
            }
        }

        private static void ReadNodeProperties(JsonNode node, out string name, out string? description)
        {
            string? readName = node[nameof(Node.Name)]?.GetValue<string>();
            if (readName == null)
                throw new InvalidDataException("Name cant be null!");

            name = readName;
            description = node[nameof(Node.Description)]?.GetValue<string>();
        }

        private static StringNode ReadStringNode(JsonNode json)
        {
            ReadNodeProperties(json, out string name, out string? description);

            string? defaultValue = json[nameof(StringNode.DefaultValue)]?.GetValue<string>();
            if (defaultValue == null)
                throw new InvalidDataException("Default value cant be null!");

            int versionIndex = json[nameof(StringNode.VersionIndex)]?.GetValue<int>() ?? 0;

            return new(name, defaultValue, versionIndex, description);
        }

        private static Node[] ReadChildNodes(JsonNode json)
        {
            if (json[nameof(ParentNode.ChildNodes)] is not JsonArray array)
                throw new InvalidDataException("Childnodes cant be null");


            Node[] result = new Node[array.Count];
            for (int i = 0; i < result.Length; i++)
            {
                JsonNode? childJson = array[i];
                if (childJson == null)
                    throw new InvalidDataException("Childnode cant be null!");

                result[i] = ReadNode(childJson);
            }

            return result;
        }

        private static ParentNode ReadParentNode(JsonNode json)
        {
            ReadNodeProperties(json, out string name, out string? description);

            Node[] childNodes = ReadChildNodes(json);

            ParentNode result = new(name, description);
            foreach (Node node in childNodes)
            {
                result.AddChildNode(node, false);
            }

            return result;
        }

        #endregion
    }
}
