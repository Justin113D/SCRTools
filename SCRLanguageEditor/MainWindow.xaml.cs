using SCRCommon.WpfStyles;
using SCRLanguageEditor.Viewmodel;
using System;
using System.Windows;

namespace SCRLanguageEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(VM_Main mainViewModel) : this()
        {
            DataContext = mainViewModel;
        }
    }
}
