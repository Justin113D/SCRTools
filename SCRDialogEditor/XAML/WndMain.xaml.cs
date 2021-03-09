using SCRDialogEditor.Viewmodel;
using System.Windows;

namespace SCRDialogEditor.XAML
{
    public partial class WndMain : Window
    {
        public WndMain()
        {
            DataContext = new VmMain();
            InitializeComponent();
        }
    }
}
