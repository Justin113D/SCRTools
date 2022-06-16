using SCR.Tools.DialogEditor.Viewmodeling;
using SCR.Tools.WPF.IO;
using System.Windows;
using System.Windows.Controls;

namespace SCR.Tools.DialogEditor.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for UcMenuBar.xaml
    /// </summary>
    public partial class UcMenuBar : UserControl
    {
        private TextFileHandler? _dialogFileHandler;
        private TextFileHandler? _dialogOptionsFileHandler;
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
                _dialogFileHandler = new(
                    "Dialog File (.json)|*.json", 
                    "Dialog Json File", 
                    vm.DialogTracker,
                    (path) => vm.WriteDialog(), 
                    (format, path) => vm.LoadDialog(format), 
                    vm.NewDialog);

                _dialogOptionsFileHandler = new(
                    "Dialog Options File (.json)|*.json",
                    "Dialog Options Json File",
                    null,
                    (path) => _viewModel.DialogOptions.Write(path),
                    (data, path) => _viewModel.DialogOptions.Read(data, path),
                    _viewModel.DialogOptions.Reset);
            }
        }

        public void NewDialog(object sender, RoutedEventArgs e)
        {
            _dialogFileHandler?.Reset();
        }

        public void LoadDialog(object sender, RoutedEventArgs e)
        {
            _dialogFileHandler?.Open();
        }

        public void SaveDialog(object sender, RoutedEventArgs e)
        {
            _dialogFileHandler?.Save(false);
        }

        public void SaveDialogAs(object sender, RoutedEventArgs e)
        {
            _dialogFileHandler?.Save(true);
        }

        private void SettingsOpen(object sender, RoutedEventArgs e)
        {
            new Windows.WndSettings().ShowDialog();
        }

        private void OpenDialogOptions(object sender, RoutedEventArgs e)
        {
            if(_viewModel == null)
            {
                return;
            }

            new Windows.WndDialogOptions(_viewModel.DialogOptions, _dialogOptionsFileHandler).ShowDialog();
        }
    }
}
