using SCRLanguageEditor.Data;
using System.Collections.ObjectModel;
using System.Linq;

namespace SCRLanguageEditor.Viewmodel
{
    /// <summary>
    /// Viewmodel for a parent node
    /// </summary>
    public class VM_ParentNode : VM_Node
    {
        /// <summary>
        /// The parent node which the viewmodel accesses and modifies
        /// </summary>
        private ParentNode ParentNode
        {
            get
            {
                return (ParentNode)node;
            }
        }

        /// <summary>
        /// The node children of the parent node
        /// </summary>
        public ObservableCollection<VM_Node> Children { get; set; }

        /// <summary>
        /// Expands and collapses the children properly
        /// </summary>
        public override bool IsExpanded
        {
            get
            {
                return Children?.Any(f => f != null) == true;
            }
            set
            {
                if(value)
                {
                    Expand();
                }
                else
                {
                    ClearChildren();
                }
            }
        }

        /// <summary>
        /// Base constructor
        /// </summary>
        /// <param name="node">The parent node</param>
        public VM_ParentNode(ParentNode node, VM_HeaderNode vm) : base(node, vm)
        {
            ClearChildren();
        }

        /// <summary>
        /// Removes the children and places a dummy object
        /// </summary>
        private void ClearChildren()
        {
            Children = new ObservableCollection<VM_Node>
            {
                null
            };
        }

        /// <summary>
        /// Creates new node viewmodels from the node children to display
        /// </summary>
        private void Expand()
        {
            Children.Clear();
            foreach(Node n in ParentNode.ChildNodes)
            {
                switch (n.Type)
                { 
                    case Node.NodeType.ParentNode:
                        Children.Add(new VM_ParentNode((ParentNode)n, VMHeaderNode));
                        break;
                    case Node.NodeType.StringNode:
                        Children.Add(new VM_StringNode((StringNode)n, VMHeaderNode));
                        break;
                    default:
                        Children.Add(null);
                        break;
                }
            }
        }

        /// <summary>
        /// Updates the viewmodels properties and those of the loaded children
        /// </summary>
        public override void UpdateProperties()
        {
            OnPropertyChanged(nameof(RequiresUpdate));
            foreach(VM_Node n in Children)
            {
                n?.UpdateProperties();
            }
        }
    }
}
