using SCRCommon.WpfStyles;
using SCRLanguageEditor.Viewmodel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
