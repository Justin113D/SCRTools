using SCR.Tools.Dialog.Editor.Viewmodeling;
using SCR.Tools.Dialog.Simulator.Viewmodeling;
using SCR.Tools.Viewmodeling;
using SCR.Tools.WPF.IO;
using System.Windows;
using System.Windows.Controls;

namespace SCR.Tools.Dialog.Editor.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for UcMenuBar.xaml
    /// </summary>
    public partial class UcMenuBar : UserControl
    {
        public static readonly DependencyProperty CmdAddNodeProperty =
            DependencyProperty.Register(
                nameof(CmdAddNode),
                typeof(RelayCommand),
                typeof(UcMenuBar)
            );

        public RelayCommand CmdAddNode
        {
            get => (RelayCommand)GetValue(CmdAddNodeProperty);
            set => SetValue(CmdAddNodeProperty, value);
        }


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
                    vm.DialogOptions.DialogOptionsTracker,
                    (path) => _viewModel.DialogOptions.Write(path),
                    (data, path) => _viewModel.DialogOptions.Read(data, path),
                    _viewModel.DialogOptions.Reset);
            }
        }

        public bool CloseConfirmation()
            => _dialogFileHandler?.ResetConfirmation() ?? true;

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
            if (_viewModel == null || _dialogOptionsFileHandler == null)
            {
                return;
            }

            _viewModel.DialogOptions.DialogOptionsTracker.Use();
            new Windows.WndDialogOptions(_viewModel.DialogOptions, _dialogOptionsFileHandler).ShowDialog();
            _viewModel.DialogTracker.Use();
        }

        private void OpenSimulator(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null)
            {
                return;
            }

            try
            {
                Simulator.WPF.Windows.WndSimulatorDialog.RunSimulator(_viewModel.Dialog.Data, _viewModel.DialogOptions.Data);
            }
            catch (SimulatorException sx)
            {
                if (sx.Node != null)
                {
                    VmNode vmNode = _viewModel.Dialog.GetViewmodel(sx.Node);
                    vmNode.Select(false, true);

                    foreach (var t in vmNode.Outputs)
                    {
                        t.IsExpanded = sx.Output == t.Data;
                    }
                }
            }
        }
    }
}
