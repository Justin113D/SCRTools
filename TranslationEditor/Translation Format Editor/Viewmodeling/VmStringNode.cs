using SCR.Tools.TranslationEditor.Data;
using SCR.Tools.UndoRedo;

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

                ChangeTracker.Global.BeginGroup();

                StringNode.DefaultValue = value;
                TrackNotifyProperty(nameof(DefaultValue));

                ChangeTracker.Global.EndGroup();
            }
        }

        public VmStringNode(VmFormat project, StringNode node) : base(project, node) { }
    }
}
