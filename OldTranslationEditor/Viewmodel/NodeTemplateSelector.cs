using System.Windows;
using System.Windows.Controls;

namespace SCRTranslationEditor.Viewmodel
{
    /// <summary>
    /// Returns according HierarchyDataTemplate based on the given node type
    /// </summary>
    public class NodeTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Parent node template
        /// </summary>
        public HierarchicalDataTemplate ParentTemplate { get; set; }

        /// <summary>
        /// String node template
        /// </summary>
        public HierarchicalDataTemplate StringTemplate { get; set; }

        /// <summary>
        /// The selector
        /// </summary>
        /// <param name="item">The node object</param>
        /// <param name="container"></param>
        /// <returns></returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            //If the item isnt a VM_Node object, it is not valid
            if(!(item is VM_Node))
                return null;

            //Returning the template based on the node type
            VM_Node node = (VM_Node)item;
            return node.Type == Data.Node.NodeType.StringNode ? StringTemplate : ParentTemplate;
        }
    }
}
