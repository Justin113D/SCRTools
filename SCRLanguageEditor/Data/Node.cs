namespace SCRLanguageEditor.Data
{
    /// <summary>
    /// Base node class for other nodes in the file
    /// </summary>
    public abstract class Node
    {
        /// <summary>
        /// The name of the node
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description of the node
        /// </summary>
        private string description;

        /// <summary>
        /// Sets and gets the description of the node accordingly
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
                    description = value;
                }
            }
        }

        /// <summary>
        /// Whether the description is valid and should be displayed
        /// </summary>
        public bool HasDescription
        {
            get
            {
                return description == null;
            }
        }

        /// <summary>
        /// Creates a node with only a name
        /// </summary>
        /// <param name="name">The name of the node</param>
        public Node(string name)
        {
            Name = name;
            Description = null;
        }

        /// <summary>
        /// Create a node with a name and a descripton
        /// </summary>
        /// <param name="name">The name of the node</param>
        /// <param name="description">The description of the node</param>
        public Node(string name, string description)
        {
            Name = name;
            Description = description;
        }

    }
}
