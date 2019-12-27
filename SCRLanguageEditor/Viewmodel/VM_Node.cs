using SCRCommon.Viewmodels;
using SCRLanguageEditor.Data;

namespace SCRLanguageEditor.Viewmodel
{
    /// <summary>
    /// Base viewmodel for a node object
    /// </summary>
    public abstract class VM_Node : BaseViewModel
    {
        /// <summary>
        /// The node which the viewmodel accesses and modifies
        /// </summary>
        protected Node node;

        /// <summary>
        /// Whether the node requires an update, based on the file and format version
        /// Returns a width for the rectangle of the template
        /// </summary>
        public int RequiresUpdate
        {
            get
            {
                return node.RequiresUpdate ? 10 : 0;
            }
        }

        /// <summary>
        /// The name of the node
        /// </summary>
        public string Name
        {
            get
            {
                return node.Name;
            }
            set
            {
                node.Name = value;
            }
        }

        /// <summary>
        /// The description of the node
        /// </summary>
        public string Description
        {
            get
            {
                return node.Description;
            }
            set
            {
                node.Description = value;
            }
        }

        /// <summary>
        /// The type of the node
        /// </summary>
        public Node.NodeType Type { get { return node.Type; } }

        /// <summary>
        /// Base constructor
        /// </summary>
        /// <param name="node">The assigned node</param>
        protected VM_Node(Node node)
        {
            this.node = node;
        }

        /// <summary>
        /// Abstract method to update necessary properties manually
        /// </summary>
        public abstract void UpdateProperties();
    }
}
