using SCRCommon.Viewmodels;
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
        /// Relaycommand for the ctrl + r binding
        /// </summary>
        public RelayCommand Cmd_Reset { get; private set; }

        /// <summary>
        /// Base constructor
        /// </summary>
        /// <param name="node"></param>
        public VM_StringNode(StringNode node) : base(node)
        {
            Cmd_Reset = new RelayCommand(Reset);
        }

        private void Reset()
        {
            StrNode.ResetValue();
            OnPropertyChanged(nameof(Value));
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
