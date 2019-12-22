using System.Windows.Controls;
using System.Windows.Media;

namespace SCRCommon.Viewmodels
{
    /// <summary>
    /// Extension methods for the TreeViewItem class
    /// </summary>
    public static class TreeViewItemExtensions
    {
        /// <summary>
        /// Gets the hierarchy depth of the treeview item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static int GetDepth(this TreeViewItem item)
        {
            TreeViewItem parent;
            while ((parent = GetParent(item)) != null)
            {
                return GetDepth(parent) + 1;
            }
            return 0;
        }

        /// <summary>
        /// Gets the parent of the tree view item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private static TreeViewItem GetParent(TreeViewItem item)
        {
            var parent = VisualTreeHelper.GetParent(item);
            while (!(parent is TreeViewItem || parent is TreeView))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent as TreeViewItem;
        }
    }
}
