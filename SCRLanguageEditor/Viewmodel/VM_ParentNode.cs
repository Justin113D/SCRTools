using SCRLanguageEditor.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCRLanguageEditor.Viewmodel
{
    public class VM_ParentNode : VM_Node
    {
        private ParentNode CatNode
        {
            get
            {
                return (ParentNode)node;
            }
        }

        public ObservableCollection<VM_Node> Children { get; set; }

        public bool IsExpanded
        {
            get
            {
                return Children?.Any(f => f != null) == true;
            }
            set
            {
                if(value)
                {
                    Expand();
                }
                else
                {
                    ClearChildren();
                }
            }
        }

        public VM_ParentNode(ParentNode node) : base(node)
        {
            ClearChildren();
        }

        private void ClearChildren()
        {
            Children = new ObservableCollection<VM_Node>
            {
                null
            };
        }

        private void Expand()
        {
            Children.Clear();
            foreach(Node n in CatNode.ChildNodes)
            {
                switch (n.Type)
                { 
                    case Node.NodeType.ParentNode:
                        Children.Add(new VM_ParentNode((ParentNode)n));
                        break;
                    case Node.NodeType.StringNode:
                        Children.Add(new VM_StringNode((StringNode)n));
                        break;
                    default:
                        Children.Add(null);
                        break;
                }
            }
        }
    }
}
