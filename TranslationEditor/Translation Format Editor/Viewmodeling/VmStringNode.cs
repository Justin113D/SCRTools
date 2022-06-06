using SCR.Tools.TranslationEditor.Data;

namespace SCR.Tools.TranslationEditor.FormatEditor.Viewmodeling
{
    public class VmStringNode : VmNode
    {
        private StringNode StringNode
            => (StringNode)_node;


        public string DefaultValue
        {
            get => StringNode.DefaultValue;
            set
            {
                if (StringNode.DefaultValue == value)
                    return;

                _format.FormatTracker.BeginGroup();

                StringNode.DefaultValue = value;
                TrackNotifyProperty(nameof(DefaultValue));

                _format.FormatTracker.EndGroup();
            }
        }

        public VmStringNode(VmFormat project, StringNode node) : base(project, node) { }
    }
}
