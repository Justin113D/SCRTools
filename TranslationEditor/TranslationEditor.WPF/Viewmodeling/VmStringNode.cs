using SCR.Tools.TranslationEditor.Data;
using SCR.Tools.TranslationEditor.Data.Events;
using SCR.Tools.Viewmodeling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCR.Tools.TranslationEditor.WPF.Viewmodeling
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

                _project.ProjectTracker.BeginGroup();
                StringNode.NodeValue = value;
                TrackNotifyProperty(nameof(NodeValue));
                TrackNotifyProperty(nameof(KeepDefault));
                _project.ProjectTracker.EndGroup();
            }
        }

        public bool KeepDefault
        {
            get => StringNode.KeepDefault;
            set
            {
                if (StringNode.KeepDefault == value)
                    return;

                _project.ProjectTracker.BeginGroup();
                StringNode.KeepDefault = value;
                TrackNotifyProperty(nameof(KeepDefault));
                TrackNotifyProperty(nameof(NodeValue));
                _project.ProjectTracker.EndGroup();
            }
        }

        public RelayCommand Cmd_Reset
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
            _project.ProjectTracker.BeginGroup();
            _project.DecreaseNodeCounter(args.OldState);
            _project.IncreaseNodeCounter(args.NewState);
            _project.ProjectTracker.EndGroup();
        }

        public override void RefreshNodeValues()
        {
            TrackNotifyProperty(nameof(KeepDefault));
            TrackNotifyProperty(nameof(NodeValue));
            TrackNotifyProperty(nameof(State));
        }
    }
}
