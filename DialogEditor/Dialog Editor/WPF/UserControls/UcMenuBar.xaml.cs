using SCR.Tools.DialogEditor.Viewmodeling;
using SCR.Tools.WPF.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SCR.Tools.DialogEditor.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for UcMenuBar.xaml
    /// </summary>
    public partial class UcMenuBar : UserControl
    {
        private TextFileHandler? _dialogFileHandler;
        private VmMain? _viewModel;

        public UcMenuBar()
        {
            DataContextChanged += OnDataContextChanged;
            InitializeComponent();
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs eventargs)
        {
            if (DataContext is not VmMain vm)
            {
                _viewModel = null;
                _dialogFileHandler = null;
            }
            else
            {
                _viewModel = vm;
                _dialogFileHandler = new("Dialog File (.json)|*.json", "Dialog Json File", vm.DialogTracker,
                    () => vm.WriteDialog(), (format) => vm.LoadDialog(format), vm.NewDialog);

            }
        }

        public void NewFormat(object sender, RoutedEventArgs e)
        {
            _dialogFileHandler?.Reset();
        }

        public void LoadFormat(object sender, RoutedEventArgs e)
        {
            _dialogFileHandler?.Open();
        }

        public void SaveFormat(object sender, RoutedEventArgs e)
        {
            _dialogFileHandler?.Save(false);
        }

        public void SaveFormatAs(object sender, RoutedEventArgs e)
        {
            _dialogFileHandler?.Save(true);
        }

        private void SettingsOpen(object sender, RoutedEventArgs e)
        {
            new Windows.WndSettings().ShowDialog();
        }
    }
}
