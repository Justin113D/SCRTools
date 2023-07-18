using SCR.Tools.TranslationEditor.Data;
using SCR.Tools.TranslationEditor.Data.Events;
using SCR.Tools.WPF.Viewmodeling;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;

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

                BeginChangeGroup();
                StringNode.NodeValue = value;
                TrackNotifyProperty(nameof(NodeValue));
                TrackNotifyProperty(nameof(KeepDefault));
                EndChangeGroup();
            }
        }

        public bool KeepDefault
        {
            get => StringNode.KeepDefault;
            set
            {
                if (StringNode.KeepDefault == value)
                    return;

                BeginChangeGroup();
                StringNode.KeepDefault = value;
                TrackNotifyProperty(nameof(KeepDefault));
                TrackNotifyProperty(nameof(NodeValue));
                EndChangeGroup();
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
            BeginChangeGroup();
            _project.DecreaseNodeCounter(args.OldState);
            _project.IncreaseNodeCounter(args.NewState);
            EndChangeGroup();
        }

        public override void RefreshNodeValues()
        {
            TrackNotifyProperty(nameof(KeepDefault));
            TrackNotifyProperty(nameof(NodeValue));
            TrackNotifyProperty(nameof(State));
        }
    }
}
