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
        private StringNode StrNode => (StringNode)node;

        public override string Name { 
            get => base.Name; 
            set
            {
                if(string.IsNullOrWhiteSpace(value))
                    return;

                if(VMHeaderNode.ChangeKey(value, StrNode))
                {
                    base.Name = value;
                }
            }
        }

        /// <summary>
        /// Redirects to the string node value
        /// </summary>
        public string Value
        {
            get => StrNode.NodeValue;
            set => StrNode.NodeValue = value;
        }

        /// <summary>
        /// Relaycommand for the ctrl + r binding
        /// </summary>
        public RelayCommand Cmd_Reset { get; private set; }

        public override bool IsExpanded
        {
            get => false;
            set => _ = value;
        }

        /// <summary>
        /// Base constructor
        /// </summary>
        /// <param name="node"></param>
        public VM_StringNode(StringNode node, VM_HeaderNode vm) : base(node, vm)
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
