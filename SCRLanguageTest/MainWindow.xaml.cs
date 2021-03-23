using System.Windows;

namespace SCRLanguageTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow() : base()
        {
            DataContext = new MainViewModel();
            InitializeComponent();
        }
    }
}
