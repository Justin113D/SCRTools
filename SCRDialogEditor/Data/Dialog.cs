using Newtonsoft.Json;
using SCRCommon.Viewmodels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace SCRDialogEditor.Data
{
    /// <summary>
    /// Complete Dialog that encapsulates all nodes
    /// </summary>
    public class Dialog
    {
        #region Private field
        /// <summary>
        /// Nodes list
        /// </summary>
        private readonly List<Node> _nodes;

        private string _name;

        private string _description;

        private string _author;

        #endregion

        /// <summary>
        /// Starter Node
        /// </summary>
        public Node StartNode => _nodes.First(x => x.Inputs.Count == 0);

        /// <summary>
        /// Node Contents
        /// </summary>
        public ReadOnlyCollection<Node> Nodes { get; private set; }

        /// <summary>
        /// Name of the dialog
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                string oldValue = _name;
                ChangeTracker.Global.TrackChange(new ChangedValue<string>(
                    (v) => _name = v,
                    oldValue,
                    value
                ));
            }
        }

        /// <summary>
        /// Dialog description
        /// </summary>
        public string Description
        {
            get => _description;
            set
            {
                string oldValue = _description;
                ChangeTracker.Global.TrackChange(new ChangedValue<string>(
                    (v) => _description = v,
                    oldValue,
                    value
                ));
            }
        }

        /// <summary>
        /// Author of the dialog
        /// </summary>
        public string Author
        {
            get => _author;
            set
            {
                string oldValue = _author;
                ChangeTracker.Global.TrackChange(new ChangedValue<string>(
                    (v) => _author = v,
                    oldValue,
                    value
                ));
            }
        }

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

            ChangeTracker.Global.TrackChange(new ChangedListSingleEntry<Node>(
                _nodes,
                n,
                _nodes.Count,
                null
            ));

            return n;
        }

        /// <summary>
        /// Removes a node from the network
        /// </summary>
        public void RemoveNode(Node node)
        {
            ChangeTracker.Global.BeginGroup();
            node.Disconnect();

            ChangeTracker.Global.TrackChange(new ChangedListSingleEntry<Node>(
                _nodes,
                node,
                null,
                null
            ));

            ChangeTracker.Global.EndGroup();
        }

        /// <summary>
        ///  (Somewhat) Sorts the nodes
        /// </summary>
        public void Sort()
        {
            List<Node> sorted = new();

            Queue<Node> nodeQueue = new();
            while(sorted.Count < _nodes.Count)
            {
                Node next = _nodes.Find(x => x.Inputs.Count == 0 && !sorted.Contains(x));
                if(next == null)
                    next = _nodes.Find(x => !sorted.Contains(x));

                while(nodeQueue.Count > 0 || next != null)
                {
                    Node q = next ?? nodeQueue.Dequeue();
                    next = null;

                    if(sorted.Contains(q))
                        continue;

                    sorted.Add(q);
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

            ChangeTracker.Global.TrackChange(new ChangedList<Node>(
                _nodes,
                sorted.ToArray(),
                null
            ));
        }

        #region Json

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
            ChangeTracker old = ChangeTracker.Global;
            new ChangeTracker().Use();
            try
            {
                using JsonTextReader reader = new(new StringReader(File.ReadAllText(path)));
                Dialog result = ReadJson(reader);
                old.Use();
                return result;
            }
            catch(JsonException)
            {
                old.Use();
                throw new InvalidDataException("The loaded json file is not a valid format");
            }
        }

        /// <summary>
        /// Loads a dialog from a Json strean
        /// </summary>
        public static Dialog ReadJson(JsonReader reader)
        {
            Dialog result = new();
            Dictionary<NodeOutput, int> outputIndices = new();

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

        #endregion


        public override string ToString()
            => $"{Name} - {Description}";
    }
}
