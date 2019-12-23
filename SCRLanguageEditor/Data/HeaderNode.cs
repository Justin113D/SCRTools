using System.Collections.Generic;

namespace SCRLanguageEditor.Data
{
    /// <summary>
    /// The Header/Language node of the file, which holds all other nodes.
    /// The name is the language
    /// </summary>
    public class HeaderNode : Node
    {
        /// <summary>
        /// Author of the document
        /// </summary>
        private string author;

        /// <summary>
        /// Gets and sets the documents author accordingly
        /// </summary>
        public string Author
        {
            get
            {
                return author;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    author = null;
                }
                else
                {
                    author = value.Trim(' ');
                }
            }
        }

        /// <summary>
        /// Version of the game that the file is for
        /// </summary>
        private string version;

        /// <summary>
        /// Gets and sets the documents version accordingly
        /// </summary>
        public string Version
        {
            get
            {
                return version;
            }
            private set
            {
                if (value == null)
                {
                    version = "";
                }
                else
                {
                    // remove all spaces before and after
                    version = value.Trim(' ');
                }
            }
        }

        private List<StringNode> stringNodes;

        /// <summary>
        /// The children of this node
        /// </summary>
        public List<Node> ChildNodes { get; private set; }

        /// <summary>
        /// Creates a Header node from language and version
        /// </summary>
        /// <param name="name">The language of the file</param>
        /// <param name="version">The game version that the file is for</param>
        public HeaderNode(string version, List<Node> children, List<StringNode> stringNodes) : base(null, NodeType.HeaderNode)
        {
            this.version = version;
            author = "";
            ChildNodes = children;
            this.stringNodes = stringNodes;
        }

        
    }
}
