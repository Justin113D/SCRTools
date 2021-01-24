using SCRDialogEditor.Viewmodel;
using System.Windows;
using System.Windows.Controls;

namespace SCRDialogEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new VMMain();
            InitializeComponent();
            
        }
    }
}
