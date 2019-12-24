using SCRCommon.Viewmodels;
using SCRLanguageEditor.Data;

namespace SCRLanguageEditor.Viewmodel
{
    public abstract class VM_Node : BaseViewModel
    {
        protected Node node;

        public string Name
        {
            get
            {
                return node.Name;
            }
            set
            {
                node.Name = value;
            }
        }


        public string Description
        {
            get
            {
                return node.Description;
            }
            set
            {
                node.Description = value;
            }
        }

        public Node.NodeType Type { get { return node.Type; } }

        public VM_Node(Node node)
        {
            this.node = node;
        }
    }
}
