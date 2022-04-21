using SCR.Tools.TranslationEditor.WPF.Viewmodeling;
using SCR.Tools.WPF.IO;
using System;
using System.Windows;
using System.Windows.Controls;

namespace SCR.Tools.TranslationEditor.WPF.XAML.UserControls
{
    /// <summary>
    /// Interaction logic for UcMenuBar.xaml
    /// </summary>
    public partial class UcMenuBar : UserControl
    {
        private TextFileHandler? _formatFileHandler;
        private TextFileHandler? _projectFileHandler;
        private FileHandler? _importExportHandler;
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
                _projectFileHandler = null;
                _formatFileHandler = null;
            }
            else
            {
                _viewModel = vm;
                _projectFileHandler = new("Language Project (.langproj)|*.langproj", "Language project file",
                    vm.ProjectTracker, vm.WriteProject, vm.LoadProject, vm.NewProject);

                _importExportHandler = new("Language Files (.lang & .langkey)|*.lang", "Language Files",
                    vm.ProjectTracker, vm.ExportLanguage, vm.ImportLanguage, null);

                _formatFileHandler = new("Format File (.json)|*.json", "Format Json File", vm.ProjectTracker,
                    null, (format) => vm.LoadFormat(format), null);
            }
        }

        public void LoadFormat(object sender, RoutedEventArgs e)
        {
            if (_formatFileHandler == null)
            {
                MessageBox.Show("Error: Viewmodel not loaded! please contact developer!", "Can't perform action", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _formatFileHandler.Open();
            _projectFileHandler?.CopyPin(_formatFileHandler);
            _importExportHandler?.CopyPin(_formatFileHandler);
        }

        public void NewProject(object sender, RoutedEventArgs e)
        {
            if (_projectFileHandler == null || _viewModel?.Format == null)
                return;

            _projectFileHandler.Reset();
            _formatFileHandler?.CopyPin(_projectFileHandler);
            _importExportHandler?.CopyPin(_projectFileHandler);
        }

        public void OpenProject(object sender, RoutedEventArgs e)
        {
            if (_projectFileHandler == null || _viewModel?.Format == null)
                return;

            _projectFileHandler.Open();
            _formatFileHandler?.CopyPin(_projectFileHandler);
            _importExportHandler?.CopyPin(_projectFileHandler);
        }

        public void SaveProject(object sender, RoutedEventArgs e)
        {
            if (_projectFileHandler == null || _viewModel?.Format == null)
                return;

            _projectFileHandler.Save(false);
            _formatFileHandler?.CopyPin(_projectFileHandler);
            _importExportHandler?.CopyPin(_projectFileHandler);
        }

        public void SaveProjectAs(object sender, RoutedEventArgs e)
        {
            if (_projectFileHandler == null || _viewModel?.Format == null)
                return;

            _projectFileHandler.Save(true);
            _formatFileHandler?.CopyPin(_projectFileHandler);
        }

        private void ExportLanguageFiles(object sender, RoutedEventArgs e)
        {
            if (_projectFileHandler == null || _importExportHandler == null || _viewModel?.Format == null)
                return;

            _importExportHandler.Save(true);
            _importExportHandler.CopyPin(_projectFileHandler);
        }

        private void ImportLanguageFiles(object sender, RoutedEventArgs e)
        {
            if (_projectFileHandler == null || _importExportHandler == null || _viewModel?.Format == null)
                return;

            _importExportHandler.Open();
            _projectFileHandler?.CopyPin(_importExportHandler);
            _formatFileHandler?.CopyPin(_importExportHandler);
        }
    }
}
