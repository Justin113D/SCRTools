using SCRLanguageEditor.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCRLanguageEditor.Viewmodel
{
    public class VM_StringNode : VM_Node
    {
        private StringNode StrNode
        {
            get
            {
                return (StringNode)node;
            }
        }

        public string Value
        {
            get
            {
                return StrNode.NodeValue;
            }
            set
            {
                StrNode.NodeValue = value;
            }
        }

        public VM_StringNode(StringNode node) : base(node)
        {
            
        }
    }
}
