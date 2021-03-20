using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace SCRDialogEditor.Data
{
    /// <summary>
    /// Complete Dialog that encapsulates all nodes
    /// </summary>
    public class Dialog
    {
        /// <summary>
        /// Nodes list
        /// </summary>
        private List<Node> _nodes;

        /// <summary>
        /// Starter node (unwrapped)
        /// </summary>
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
        public ReadOnlyCollection<Node> Nodes { get; private set; }

        /// <summary>
        /// Name of the dialog
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Dialog description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Author of the dialog
        /// </summary>
        public string Author { get; set; }

        public Dialog()
        {
            _nodes = new();
            Nodes = _nodes.AsReadOnly();
        }

        /// <summary>
        /// Creates a new node
        /// </summary>
        public Node CreateNode()
        {
            Node n = new();
            _nodes.Add(n);
            return n;
        }

        /// <summary>
        /// Removes a node from the network
        /// </summary>
        public void RemoveNode(Node node)
        {
            node.Disconnect();
            _nodes.Remove(node);
        }

        public void Sort()
        {
            List<Node> before = new(_nodes.ToArray());

            _nodes.Clear();

            Queue<Node> nodeQueue = new();
            while(_nodes.Count < before.Count)
            {
                Node next = before.Find(x => x.Inputs.Count == 0 && !_nodes.Contains(x));
                if(next == null)
                    next = before.Find(x => !_nodes.Contains(x));

                while(nodeQueue.Count > 0 || next != null)
                {
                    Node q = next ?? nodeQueue.Dequeue();
                    next = null;

                    if(_nodes.Contains(q))
                        continue;

                    _nodes.Add(q);
                    if(q.Outputs.Count == 1)
                        next = q.Outputs[0].Output;
                    else
                    {
                        foreach(var o in q.Outputs)
                        {
                            nodeQueue.Enqueue(o.Output);
                        }
                    }
                }
            }
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

        /// <summary>
        /// Writes the dialog to a Json stream
        /// </summary>
        public void WriteJson(JsonWriter writer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("Author");
            writer.WriteValue(Author);

            writer.WritePropertyName("Name");
            writer.WriteValue(Name);

            writer.WritePropertyName("Description");
            writer.WriteValue(Description);

            writer.WritePropertyName("Nodes");
            writer.WriteStartArray();

            foreach(Node n in Nodes)
                n.WriteJSON(writer, this);

            writer.WriteEndArray();

            writer.WriteEndObject();
        }

        /// <summary>
        /// Loads the Dialog from a file
        /// </summary>
        public static Dialog LoadFromFile(string path)
        {
            try
            {
                using JsonTextReader reader = new(new StringReader(File.ReadAllText(path)));
                return ReadJson(reader);
            }
            catch(JsonException)
            {
                throw new InvalidDataException("The loaded json file is not a valid format");
            }
        }

        /// <summary>
        /// Loads a dialog from a Json strean
        /// </summary>
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
                                result._nodes.Add(Node.ReadJson(reader, outputIndices));
                            }
                            break;
                    }
                }
            }

            foreach(var k in outputIndices)
                k.Key.SetOutput(result.Nodes[k.Value]);

            return result;
        }


        public override string ToString()
            => $"{Name} - {Description}";
    }
}
