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
                VMHeader.Tracker.BeginGroup();
                if(string.IsNullOrWhiteSpace(value) || !VMHeader.ChangeKey(value, StringNode))
                {
                    VMHeader.Tracker.EndGroup();
                    return;
                }


                if(StringNode.VersionID != VMHeader.NewestFormatID)
                {
                    VMHeader.Tracker.TrackChange(new ChangedValue<int>((v) => StringNode.VersionID = v, StringNode.VersionID, VMHeader.NewestFormatID));
                    StringNode.VersionID = VMHeader.NewestFormatID;
                }

                base.Name = value;

                VMHeader.Tracker.EndGroup();
            }
        }

        public override string Description
        {
            get => base.Description;
            set
            {
                VMHeader.Tracker.BeginGroup();

                if(StringNode.VersionID != VMHeader.NewestFormatID)
                {
                    VMHeader.Tracker.TrackChange(new ChangedValue<int>((v) => StringNode.VersionID = v, StringNode.VersionID, VMHeader.NewestFormatID));
                    StringNode.VersionID = VMHeader.NewestFormatID;
                }

                base.Description = value;

                VMHeader.Tracker.EndGroup();
            }
        }

        /// <summary>
        /// Redirects to the string node value
        /// </summary>
        public string Value
        {
            get => Properties.Settings.Default.DevMode ? StringNode.DefaultValue : StringNode.NodeValue;
            set
            {
                if(Properties.Settings.Default.DevMode)
                {
                    if(StringNode.DefaultValue == value)
                        return;

                    VMHeader.Tracker.TrackChange(new ChangedValue<string>((v) =>
                    {
                        StringNode.DefaultValue = v;
                        OnPropertyChanged(nameof(Value));
                    }, StringNode.DefaultValue, value));
                    StringNode.DefaultValue = value;
                }
                else
                {
                    if(StringNode.NodeValue == value)
                        return;

                    int oldstate = Node.NodeState;
                    string oldNodeValue = StringNode.NodeValue;
                    StringNode.NodeValue = value;
                    int newState = Node.NodeState;
                    bool wasRetranslated = StringNode.retranslated;

                    VMHeader.Tracker.TrackChange(new Change((v) =>
                    {
                        StringNode.NodeValue = v ? value : oldNodeValue;
                        UpdateProperties();
                        Parent?.UpdateNodeState();

                        if(v)
                        {
                            StringNode.retranslated = wasRetranslated;
                            VMHeader.NodeUpdated(oldstate, newState);
                        }
                        else
                        {
                            VMHeader.NodeUpdated(newState, oldstate);
                        }

                    }));

                    UpdateProperties();
                    Parent?.UpdateNodeState();
                    VMHeader.NodeUpdated(oldstate, Node.NodeState);
                }
            }
        }

        /// <summary>
        /// Relaycommand for the ctrl + r binding
        /// </summary>
        public RelayCommand Cmd_Reset { get; }

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
            VMHeader.Tracker.BeginGroup();

            VMHeader.Tracker.TrackChange(new ChangedValue<bool>((v) => StringNode.requiresUpdate = v, StringNode.requiresUpdate, false));
            VMHeader.Tracker.TrackChange(new ChangedValue<bool>((v) => StringNode.retranslated = v, StringNode.retranslated, false));
            VMHeader.Tracker.TrackChange(new ChangedValue<string>((v) =>
            {
                StringNode.NodeValue = v;
                UpdateProperties();
                Parent?.UpdateNodeState();
            }, StringNode.NodeValue, StringNode.DefaultValue));

            VMHeader.Tracker.EndGroup();

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
            VMHeader.Tracker.BeginGroup();
            VMHeader.RemoveKey(Name);
            base.Remove();
            VMHeader.Tracker.EndGroup();
        }
    }
}
