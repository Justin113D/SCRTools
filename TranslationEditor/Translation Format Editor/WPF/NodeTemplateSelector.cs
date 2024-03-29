﻿using SCR.Tools.TranslationEditor.FormatEditor.Viewmodeling;
using System.Windows;
using System.Windows.Controls;

namespace SCR.Tools.TranslationEditor.FormatEditor.WPF
{
    /// <summary>
    /// Returns according HierarchyDataTemplate based on the given node type
    /// </summary>
    public class NodeTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Parent node template
        /// </summary>
        public HierarchicalDataTemplate? ParentTemplate { get; set; }

        /// <summary>
        /// String node template
        /// </summary>
        public HierarchicalDataTemplate? StringTemplate { get; set; }

        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            if (item is VmParentNode)
            {
                return ParentTemplate;
            }
            else if (item is VmStringNode)
            {
                return StringTemplate;
            }

            return null;
        }
    }
}
