using SCR.Tools.TranslationEditor.Data;
using SCR.Tools.TranslationEditor.Data.Events;
using SCR.Tools.UndoRedo;
using SCR.Tools.Viewmodeling;

namespace SCR.Tools.TranslationEditor.ProjectEditor.Viewmodeling
{
    public class VmStringNode : VmNode
    {
        private StringNode StringNode
            => (StringNode)_node;


        public override string DefaultValue
            => StringNode.DefaultValue;

        public string NodeValue
        {
            get => StringNode.NodeValue;
            set
            {
                if (StringNode.NodeValue == value)
                    return;

                ChangeTracker.Global.BeginGroup();
                StringNode.NodeValue = value;
                TrackNotifyProperty(nameof(NodeValue));
                TrackNotifyProperty(nameof(KeepDefault));
                ChangeTracker.Global.EndGroup();
            }
        }

        public bool KeepDefault
        {
            get => StringNode.KeepDefault;
            set
            {
                if (StringNode.KeepDefault == value)
                    return;

                ChangeTracker.Global.BeginGroup();
                StringNode.KeepDefault = value;
                TrackNotifyProperty(nameof(KeepDefault));
                TrackNotifyProperty(nameof(NodeValue));
                ChangeTracker.Global.EndGroup();
            }
        }

        public RelayCommand CmdReset
            => new(ResetValue);

        public VmStringNode(VmProject project, StringNode node) : base(project, node)
        {
            node.NodeStateChanged += UpdateNodeCounter;
        }

        ~VmStringNode()
        {
            _node.NodeStateChanged -= UpdateNodeCounter;
        }

        private void ResetValue()
        {
            StringNode.ResetValue();
        }

        private void UpdateNodeCounter(Node node, NodeStateChangedEventArgs args)
        {
            ChangeTracker.Global.BeginGroup();
            _project.DecreaseNodeCounter(args.OldState);
            _project.IncreaseNodeCounter(args.NewState);
            ChangeTracker.Global.EndGroup();
        }

        public override void RefreshNodeValues()
        {
            TrackNotifyProperty(nameof(KeepDefault));
            TrackNotifyProperty(nameof(NodeValue));
            TrackNotifyProperty(nameof(State));
        }
    }
}
