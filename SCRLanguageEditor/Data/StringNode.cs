using Newtonsoft.Json;

namespace SCRLanguageEditor.Data
{
    /// <summary>
    /// A node holding a text value, the main focus of the language/text file
    /// </summary>
    public class StringNode : Node
    {
        /// <summary>
        /// The value of the string held in the node
        /// </summary>
        private string nodeValue;

        /// <summary>
        /// Version id of this node in the format
        /// </summary>
        public int VersionID { get; set; }

        public bool requiresUpdate;

        public bool retranslated;

        /// <summary>
        /// The default value for this node
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// Gets and sets nodes value accordingly
        /// </summary>
        public string NodeValue
        {
            get => nodeValue;
            set
            {
                nodeValue = value == null ? "" : value.Trim(' ');
                retranslated = true;
            }
        }

        /// <summary>
        /// Nodestate </br>
        /// 0 = Untranslated </br> 
        /// 1 = Requires update </br>
        /// 2 = Retranslated </br>
        /// 3 = Translated
        /// </summary>
        public override int NodeState
        {
            get
            {
                if(NodeValue == DefaultValue)
                    return 0;

                if(requiresUpdate)
                    return retranslated ? 2 : 1;

                return 3;
            }
        }

        /// <summary>
        /// Create a string node
        /// </summary>
        /// <param name="name">The name of the node</param>
        public StringNode(string name, string value, int versionID) : base(name, NodeType.StringNode)
        {
            nodeValue = DefaultValue = value;
            this.VersionID = versionID;
        }

        /// <summary>
        /// Create a string node with a description
        /// </summary>
        /// <param name="name">The name of the node</param>
        /// <param name="description">The description of the node</param>
        public StringNode(string name, string value, int versionID, string description) : base(name, description, NodeType.StringNode)
        {
            nodeValue = DefaultValue = value;
            VersionID = versionID;
        }

        /// <summary>
        /// Sets the node value to the default value
        /// </summary>
        public void ResetValue()
        {
            NodeValue = DefaultValue;
            requiresUpdate = false;
            retranslated = false;
        }

        protected override void WriteJsonInner(JsonTextWriter writer)
        {
            writer.WritePropertyName("DefaultValue");
            writer.WriteValue(DefaultValue);

            if(VersionID != 0)
            {
                writer.WritePropertyName("VersionID");
                writer.WriteValue(VersionID);
            }
        }
    }
}
