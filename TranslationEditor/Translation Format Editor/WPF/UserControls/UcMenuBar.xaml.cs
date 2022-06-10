using SCR.Tools.TranslationEditor.FormatEditor.Viewmodeling;
using SCR.Tools.WPF.IO;
using System.Windows;
using System.Windows.Controls;

namespace SCR.Tools.TranslationEditor.FormatEditor.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for UcMenuBar.xaml
    /// </summary>
    public partial class UcMenuBar : UserControl
    {
        private TextFileHandler? _formatFileHandler;
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
                _formatFileHandler = null;
            }
            else
            {
                _viewModel = vm;
                _formatFileHandler = new("Format File (.json)|*.json", "Format Json File", vm.FormatTracker,
                    () => vm.WriteFormat(), (format) => vm.LoadFormat(format), vm.NewFormat);
            }
        }

        public void NewFormat(object sender, RoutedEventArgs e)
        {
            _formatFileHandler?.Reset();
        }

        public void LoadFormat(object sender, RoutedEventArgs e)
        {
            _formatFileHandler?.Open();
        }

        public void SaveFormat(object sender, RoutedEventArgs e)
        {
            _formatFileHandler?.Save(false);
        }

        public void SaveFormatAs(object sender, RoutedEventArgs e)
        {
            _formatFileHandler?.Save(true);
        }

        private void ExpandAll(object sender, RoutedEventArgs e)
        {
            var t = MessageBox.Show("Depending on how big the format is, this operation could take a while if not crash the application.\nProceed?", "Expand All?", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

            if (t != MessageBoxResult.OK)
                return;

            _viewModel?.Format.ExpandAll();
        }

        private void SettingsOpen(object sender, RoutedEventArgs e)
        {
            new Windows.WndSettings().ShowDialog();
        }

    }
}
