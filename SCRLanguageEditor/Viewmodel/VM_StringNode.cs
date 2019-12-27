using SCRLanguageEditor.Data;

namespace SCRLanguageEditor.Viewmodel
{
    /// <summary>
    /// Viewmodel for a string node
    /// </summary>
    public class VM_StringNode : VM_Node
    {
        /// <summary>
        /// The string node which the viewmodel accesses and modifies
        /// </summary>
        private StringNode StrNode
        {
            get
            {
                return (StringNode)node;
            }
        }

        /// <summary>
        /// Redirects to the string node value
        /// </summary>
        public string Value
        {
            get
            {
                return StrNode.NodeValue;
            }
            set
            {
                StrNode.NodeValue = value;
            }
        }

        /// <summary>
        /// Base constructor
        /// </summary>
        /// <param name="node"></param>
        public VM_StringNode(StringNode node) : base(node)
        {
            
        }

        /// <summary>
        /// Updates value and whether the node requires and update
        /// </summary>
        public override void UpdateProperties()
        {
            OnPropertyChanged(nameof(RequiresUpdate));
            OnPropertyChanged(nameof(Value));
        }
    }
}
