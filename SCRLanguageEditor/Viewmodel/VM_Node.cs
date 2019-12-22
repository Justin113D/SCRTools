using SCRLanguageEditor.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCRLanguageEditor.Viewmodel
{
    public abstract class VM_Node
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
