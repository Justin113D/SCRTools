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
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (value == null)
                {
                    name = "";
                }
                else
                {
                    // remove all spaces before and after
                    name = value.Trim(' ');
                }
            }
        }

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
        /// Whether this node (or any of its child nodes, if it has any) requires an update
        /// </summary>
        public bool RequiresUpdate;

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

    }
}
