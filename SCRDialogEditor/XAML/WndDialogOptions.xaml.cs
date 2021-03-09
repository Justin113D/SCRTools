using SCRDialogEditor.Viewmodel;
using System.Windows;

namespace SCRDialogEditor.XAML
{
    /// <summary>
    /// Interaction logic for DialogOptionsWindow.xaml
    /// </summary>
    public partial class WndDialogOptions : Window
    {
        public WndDialogOptions(VmDialogOptions vmDO)
        {
            DataContext = vmDO;
            InitializeComponent();
        }
    }
}
