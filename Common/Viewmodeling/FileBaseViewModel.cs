using Microsoft.Win32;
using SCR.Tools.UndoRedo;
using System.Windows;

namespace SCR.Tools.Viewmodeling
{
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
        /// Change tracker to set a pin in on save
        /// </summary>
        public abstract ChangeTracker PinTracker { get; }

        private ChangeTracker.Pin? _savePin;

        /// <summary>
        /// Loaded file path
        /// </summary>
        public string LoadedFilePath { get; set; }

        public abstract string FileTypeName { get; }

        /// <summary>
        /// Loads a file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public abstract bool Load(string path);

        /// <summary>
        /// Saves a file
        /// </summary>
        /// <param name="path"></param>
        public abstract void Save(string path);

        /// <summary>
        /// Confirm whether current data can be overwritten
        /// </summary>
        /// <returns></returns>
        public abstract bool ResetConfirmation();

        /// <summary>
        /// Resets the current data
        /// </summary>
        public abstract void Reset();

        private bool InternalResetConfirmation()
        {
            if (_savePin?.CheckValid() != false || ResetConfirmation())
            {
                MessageBoxResult r = MessageBox.Show("Unsaved changes will be reset!\nDo you want to save before?", "Warning!", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);
                switch (r)
                {
                    case MessageBoxResult.Yes:
                        if (!InternalSave(false))
                            return false;
                        break;
                    case MessageBoxResult.No:
                        break;
                    case MessageBoxResult.None:
                    case MessageBoxResult.Cancel:
                    default:
                        return false;
                }
                return true;
            }
            return true;
        }

        private void InternalLoad()
        {
            if (!ResetConfirmation())
                return;

            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = FileFilter,
                Title = $"Load from {FileTypeName} File"
            };

            if (ofd.ShowDialog() != true)
            {
                return;
            }

            if (Load(ofd.FileName))
            {
                LoadedFilePath = ofd.FileName;
                _savePin = PinTracker?.PinCurrent();
            }

        }

        private bool InternalSave(bool newPath)
        {
            if (string.IsNullOrWhiteSpace(LoadedFilePath) || newPath)
            {
                SaveFileDialog sfd = new()
                {
                    Filter = FileFilter,
                    Title = $"Save {FileTypeName} to File"
                };

                if (sfd.ShowDialog() == true)
                {
                    Save(sfd.FileName);
                    LoadedFilePath = sfd.FileName;
                }
                else
                    return false;
            }
            else
                Save(LoadedFilePath);
            _savePin = PinTracker?.PinCurrent();
            return true;
        }

        private void InternalReset()
        {
            if (!InternalResetConfirmation())
                return;
            Reset();
            _savePin = PinTracker?.PinCurrent();
        }
    }
}
