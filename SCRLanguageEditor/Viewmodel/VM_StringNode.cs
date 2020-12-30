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
        private StringNode StringNode => (StringNode)Node;

        public override string Name
        {
            get => base.Name;
            set
            {
                if(string.IsNullOrWhiteSpace(value))
                    return;

                if(VMHeader.ChangeKey(value, StringNode))
                {
                    base.Name = value;
                    StringNode.VersionID = VMHeader.NewestFormatID;
                }
            }
        }

        public override string Description 
        { 
            get => base.Description;
            set
            {
                base.Description = value;
                StringNode.VersionID = VMHeader.NewestFormatID;
            }
        }

        /// <summary>
        /// Redirects to the string node value
        /// </summary>
        public string Value
        {
            get => StringNode.NodeValue;
            set
            {
                StringNode.NodeValue = value;
                if(Properties.Settings.Default.DevMode)
                {
                    StringNode.DefaultValue = value;
                    return;
                }

                OnPropertyChanged(nameof(NodeState));
                Parent.UpdateNodeState();
            }
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
        public VM_StringNode(VM_ParentNode parent, StringNode node, VM_HeaderNode vm) : base(node, vm, parent)
        {
            Cmd_Reset = new RelayCommand(Reset);
        }

        private void Reset()
        {
            StringNode.ResetValue();
            UpdateProperties();
            Parent?.UpdateNodeState();
        }

        /// <summary>
        /// Updates value and whether the node requires and update
        /// </summary>
        public override void UpdateProperties()
        {
            OnPropertyChanged(nameof(NodeState));
            OnPropertyChanged(nameof(Value));
        }

        public override void Remove()
        {
            VMHeader.RemoveStringNode(StringNode);
            base.Remove();
        }
    }
}
