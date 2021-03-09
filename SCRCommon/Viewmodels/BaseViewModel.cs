using Microsoft.Win32;
using System.ComponentModel;
using System.Windows;

namespace SCRCommon.Viewmodels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        private static readonly DependencyObject _dummyDependencyObject = new DependencyObject();

        protected static bool IsDesignMode => !DesignerProperties.GetIsInDesignMode(_dummyDependencyObject);

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public abstract class FileBaseViewModel : BaseViewModel
    {
        public RelayCommand Cmd_Save 
            => new(() => InternalSave(false));
        public RelayCommand Cmd_SaveAs 
            => new(() => InternalSave(true));
        public RelayCommand Cmd_Load 
            => new(InternalLoad);
        public RelayCommand Cmd_Reset 
            => new(InternalReset);

        /// <summary>
        /// Dialog File filter to use
        /// </summary>
        public abstract string FileFilter { get; }

        /// <summary>
        /// Loaded file path
        /// </summary>
        public string LoadedFilePath { get; set; }

        public abstract string FileTypeName { get; }

        public abstract bool Load(string path);

        public abstract void Save(string path);

        public abstract bool ResetConfirmation();

        public abstract void Reset();

        private void InternalLoad()
        {
            if(!ResetConfirmation())
                return;

            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = FileFilter,
                Title = $"Load from {FileTypeName} File"
            };

            if(ofd.ShowDialog() != true)
            {
                return;
            }

            if(Load(ofd.FileName))
                LoadedFilePath = ofd.FileName;
        }

        private void InternalSave(bool newPath)
        {
            if(string.IsNullOrWhiteSpace(LoadedFilePath) || newPath)
            {
                SaveFileDialog sfd = new SaveFileDialog()
                {
                    Filter = FileFilter,
                    Title = $"Save {FileTypeName} to File"
                };

                if(sfd.ShowDialog() == true)
                {
                    LoadedFilePath = sfd.FileName;
                }
                else
                    return;
            }

            Save(LoadedFilePath);
        }

        private void InternalReset()
        {
            if(!ResetConfirmation())
                return;
            Reset();
        }
    }
}
