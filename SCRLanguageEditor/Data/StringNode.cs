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
        /// The default value for this node
        /// </summary>
        public string DefaultValue { get; private set; }

        /// <summary>
        /// Gets and sets nodes value accordingly
        /// </summary>
        public string NodeValue
        {
            get
            {
                return nodeValue;
            }
            set
            {
                if(value == null)
                {
                    nodeValue = "";
                }
                else
                {
                    // remove all spaces before and after
                    nodeValue = value.Trim(' ');
                }
            }
        }

        public readonly int versionID;

        public bool RequiresUpdate { get; private set; }

        /// <summary>
        /// Create a string node
        /// </summary>
        /// <param name="name">The name of the node</param>
        public StringNode(string name, string value, int versionID) : base(name, NodeType.StringNode)
        {
            nodeValue = DefaultValue = value;
            this.versionID = versionID;
        }

        /// <summary>
        /// Create a string node with a description
        /// </summary>
        /// <param name="name">The name of the node</param>
        /// <param name="description">The description of the node</param>
        public StringNode(string name, string value, int versionID, string description) : base(name, description, NodeType.StringNode)
        {
            nodeValue = DefaultValue = value;
            this.versionID = versionID;
            RequiresUpdate = false;
        }

        /// <summary>
        /// Sets the node value to the default value
        /// </summary>
        public void ResetValue()
        {
            NodeValue = DefaultValue;
        }
    }
}
