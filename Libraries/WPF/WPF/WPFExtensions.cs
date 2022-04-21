using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SCR.Tools.WPF
{
    public static class WPFExtensions
    {
        public static T? FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            //get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            //we've reached the end of the tree
            if (parentObject == null)
                return null;

            //check if the parent matches the type we're looking for
            if (parentObject is T parent)
                return parent;
            else
                return FindParent<T>(parentObject);
        }

        /// <summary>
        /// Gets the hierarchy depth of the treeview item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static int GetDepth(this TreeViewItem item)
        {
            TreeViewItem? parent = GetParent(item);
            if(parent != null)
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
        private static TreeViewItem? GetParent(TreeViewItem item)
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
