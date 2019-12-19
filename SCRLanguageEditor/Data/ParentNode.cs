using System.Collections.ObjectModel;

namespace SCRLanguageEditor.Data
{
    /// <summary>
    /// A node which holds more nodes as children (hierarchy node)
    /// </summary>
    public class ParentNode : Node
    {
        /// <summary>
        /// The children of this node
        /// </summary>
        public ObservableCollection<Node> ChildNodes { get; private set; }
         
        /// <summary>
        /// Creates a parent node
        /// </summary>
        /// <param name="name">The name of the parent node</param>
        public ParentNode(string name) : base(name)
        {
            ChildNodes = new ObservableCollection<Node>();
        }

        /// <summary>
        /// Creates a parent node with a description
        /// </summary>
        /// <param name="name">The name of the node</param>
        /// <param name="description">The description of the node</param>
        public ParentNode(string name, string description) : base(name, description)
        {
            ChildNodes = new ObservableCollection<Node>();
        }

    }
}
