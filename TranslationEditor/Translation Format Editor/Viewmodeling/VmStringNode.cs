using SCR.Tools.TranslationEditor.Data;
using SCR.Tools.UndoRedo;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;

namespace SCR.Tools.TranslationEditor.FormatEditor.Viewmodeling
{
    public class VmStringNode : VmNode
    {
        public StringNode StringNode
            => (StringNode)Node;


        public string DefaultValue
        {
            get => StringNode.DefaultValue;
            set
            {
                if (StringNode.DefaultValue == value)
                    return;

                BeginChangeGroup();

                StringNode.DefaultValue = value;
                TrackNotifyProperty(nameof(DefaultValue));

                EndChangeGroup();
            }
        }

        public VmStringNode(VmFormat project, StringNode node) : base(project, node) { }
    }
}
