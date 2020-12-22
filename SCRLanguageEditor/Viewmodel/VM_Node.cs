using PropertyChanged;
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
        /// Header node access
        /// </summary>
        protected VM_HeaderNode VMHeaderNode { get; }

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
        public virtual string Name
        {
            get => node.Name;
            set
            {
                if(string.IsNullOrWhiteSpace(value))
                    return;

                node.Name = value;
            }
        }

        /// <summary>
        /// The description of the node
        /// </summary>
        public string Description
        {
            get => node.Description;
            set
            {
                if(string.IsNullOrWhiteSpace(value))
                    return;

                node.Description = value;
            }
        }

        [SuppressPropertyChangedWarnings]
        public abstract bool IsExpanded { get; set; }

        /// <summary>
        /// The type of the node
        /// </summary>
        public Node.NodeType Type { get { return node.Type; } }

        /// <summary>
        /// Base constructor
        /// </summary>
        /// <param name="node">The assigned node</param>
        protected VM_Node(Node node, VM_HeaderNode vm)
        {
            this.node = node;
            VMHeaderNode = vm;
        }

        /// <summary>
        /// Abstract method to update necessary properties manually
        /// </summary>
        public abstract void UpdateProperties();
    }
}
