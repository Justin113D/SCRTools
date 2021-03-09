using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace SCRDialogEditor.Data
{
    /// <summary>
    /// Complete Dialog that encapsulates all nodes
    /// </summary>
    public class Dialog
    {
        private Node _startNode;

        /// <summary>
        /// Starter Node
        /// </summary>
        public Node StartNode
        {
            get
            {
                if(_startNode == null)
                {
                    foreach(Node n in Nodes)
                        if(n.Inputs.Count == 0)
                            return n;
                }
                return _startNode;
            }
            set => _startNode = value;
        }

        /// <summary>
        /// Node Contents
        /// </summary>
        public List<Node> Nodes;

        public string Name;

        public string Description;

        public string Author;

        public Dialog()
        {
            Nodes = new List<Node>();
        }

        /// <summary>
        /// Saves the Dialog to a file
        /// </summary>
        /// <param name="path"></param>
        public void SaveToFile(string path)
        {
            using StringWriter strWriter = new();
            using JsonTextWriter writer = new(strWriter)
            {
                Formatting = Formatting.Indented
            };

            WriteJson(writer);

            File.WriteAllText(path, strWriter.ToString());
        }

        public void WriteJson(JsonWriter writer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("Nodes");
            writer.WriteStartArray();

            foreach(Node n in Nodes)
                n.WriteJSON(writer);

            writer.WriteEndArray();

            writer.WriteEndObject();
        }

        /// <summary>
        /// Loads the Dialog from a file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Dialog LoadFromFile(string path)
        {
            using JsonTextReader reader = new(new StringReader(File.ReadAllText(path)));
            return ReadJson(reader);
        }

        public static Dialog ReadJson(JsonReader reader)
        {
            Dialog result = new();

            Dictionary<NodeOutput, int> outputIndices = new Dictionary<NodeOutput, int>();

            while(reader.Read() && reader.TokenType != JsonToken.EndObject)
            {
                if(reader.TokenType == JsonToken.PropertyName)
                {
                    string tokenName = (string)reader.Value;
                    reader.Read();
                    switch(tokenName)
                    {
                        case "Name":
                            result.Name = (string)reader.Value;
                            break;
                        case "Description":
                            result.Description = (string)reader.Value;
                            break;
                        case "Author":
                            result.Author = (string)reader.Value;
                            break;
                        case "Nodes":
                            while(reader.Read() && reader.TokenType != JsonToken.EndArray)
                            {
                                result.Nodes.Add(Node.ReadJson(reader, result, outputIndices));
                            }
                            break;
                    }
                }
            }

            foreach(var k in outputIndices)
                k.Key.SetOutput(result.Nodes[k.Value]);

            return result;
        }
    }
}
