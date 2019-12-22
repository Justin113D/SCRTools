using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SCRLanguageEditor.Viewmodel
{
    public class NodeTemplateSelector : DataTemplateSelector
    {
        public HierarchicalDataTemplate ParentTemplate { get; set; }

        public HierarchicalDataTemplate StringTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if(!(item is VM_Node))
            {
                return null;
            }
            VM_Node node = (VM_Node)item;
            return node.Type == Data.Node.NodeType.StringNode ? StringTemplate : ParentTemplate;
        }
    }
}
