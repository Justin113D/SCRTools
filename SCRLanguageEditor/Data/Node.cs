using Newtonsoft.Json;
using System.Collections.Generic;

namespace SCRLanguageEditor.Data
{
    /// <summary>
    /// Base node class for other nodes in the file
    /// </summary>
    public abstract class Node
    {
        /// <summary>
        /// The type of node
        /// </summary>
        public enum NodeType : int
        {
            None = -1,
            HeaderNode = 0,
            ParentNode = 1,
            StringNode = 2
        }

        [JsonIgnore]
        /// <summary>
        /// The type of the node
        /// </summary>
        public readonly NodeType Type;

        /// <summary>
        /// The name of the node
        /// </summary>
        private string name;

        /// <summary>
        /// Gets and sets the nodes name accordingly
        /// </summary>
        public virtual string Name
        {
            get => name; 
            set
            {
                if (value == null)
                    name = "";
                else
                {
                    // remove all spaces before and after
                    name = value.Trim(' ');
                }
            }
        }

        public abstract int NodeState { get; }

        /// <summary>
        /// The description of the node
        /// </summary>
        private string description;

        /// <summary>
        /// Gets and sets the nodes description accordingly
        /// </summary>
        public string Description
        {
            get
            {
                return description;
            }
            set {
                if(string.IsNullOrWhiteSpace(value))
                {
                    description = null;
                }
                else
                {
                    description = value.Trim(' ');
                }
            }
        }

        /// <summary>
        /// Creates a node with only a name
        /// </summary>
        /// <param name="name">The name of the node</param>
        protected Node(string name, NodeType type)
        {
            this.name = name;
            Description = null;
            Type = type;
        }

        /// <summary>
        /// Create a node with a name and a descripton
        /// </summary>
        /// <param name="name">The name of the node</param>
        /// <param name="description">The description of the node</param>
        protected Node(string name, string description, NodeType type)
        {
            this.name = name;
            Description = description;
            Type = type;
        }

        /// <summary>
        /// Writes the node as a json object
        /// </summary>
        /// <param name="writer"></param>
        public void WriteJson(JsonTextWriter writer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("Name");
            writer.WriteValue(Name);

            if(Description != null)
            {
                writer.WritePropertyName("Description");
                writer.WriteValue(Description);
            }


            WriteJsonInner(writer);
            writer.WriteEndObject();
        }

        /// <summary>
        /// Per Node data to write
        /// </summary>
        /// <param name="writer"></param>
        protected abstract void WriteJsonInner(JsonTextWriter writer);

        /// <summary>
        /// Reads a Json node object
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static Node ReadJson(JsonTextReader reader)
        {
            string Name = null;
            string Description = null;
            string DefaultValue = null;
            int version = 0;
            List<Node> nodes = null;

            while(reader.Read() && reader.TokenType != JsonToken.EndObject)
            {
                if(reader.TokenType == JsonToken.PropertyName)
                {
                    string tokenName = (string)reader.Value;
                    reader.Read();
                    switch(tokenName)
                    {
                        case "Name":
                            Name = (string)reader.Value;
                            break;
                        case "Description":
                            Description = (string)reader.Value;
                            break;
                        case "DefaultValue":
                            DefaultValue = (string)reader.Value;
                            break;
                        case "VersionID":
                            version = (int)((long)reader.Value);
                            break;
                        case "ChildNodes":
                            nodes = new List<Node>();
                            while(reader.Read() && reader.TokenType != JsonToken.EndArray)
                            {
                                nodes.Add(ReadJson(reader));
                            }
                            break;
                    }
                }
            }

            if(nodes != null)
            {
                ParentNode result = new ParentNode(Name, Description);
                result.ChildNodes.AddRange(nodes);
                return result;
            }
            else
            {
                return new StringNode(Name, DefaultValue, version, Description);
            }
        }
    }
}
