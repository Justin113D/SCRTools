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
        /// Sets the value of the node accordingly
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

        /// <summary>
        /// Create a string node
        /// </summary>
        /// <param name="name">The name of the node</param>
        /// <param name="nodeValue">The string value held in the node</param>
        public StringNode(string name, string nodeValue) : base(name) => NodeValue = nodeValue;

        /// <summary>
        /// Create a string node with a description
        /// </summary>
        /// <param name="name">The name of the node</param>
        /// <param name="description">The description of the node</param>
        /// <param name="nodeValue">The string value held in the node</param>
        public StringNode(string name, string description, string nodeValue) : base(name, description) => NodeValue = nodeValue;
    }
}
