using Microsoft.Win32;
using SCR.Tools.UndoRedo;
using System;
using System.Windows;

namespace SCR.Tools.WPF.IO
{
    public abstract class BaseFileHandler
    {
        private readonly string _fileFilter;

        private readonly string _fileTypeName;

        private readonly ChangeTracker? _pinTracker;

        public string? LoadedFilePath { get; private set; }

        private ChangeTracker.Pin? _savePin;

        protected BaseFileHandler(string fileFilter, string fileTypeName, ChangeTracker? pinTracker)
        {
            _fileFilter = fileFilter;
            _fileTypeName = fileTypeName;
            _pinTracker = pinTracker;
            _savePin = pinTracker?.PinCurrent();
        }

        public bool ResetConfirmation()
        {
            if (_savePin?.CheckValid() != true)
            {
                MessageBoxResult r = MessageBox.Show("Unsaved changes will be reset!\nDo you want to save before?", "Warning!", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);
                switch (r)
                {
                    case MessageBoxResult.Yes:
                        if (!Save(false))
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

        public bool Save(bool newPath)
        {
            if (string.IsNullOrWhiteSpace(LoadedFilePath) || newPath)
            {
                SaveFileDialog sfd = new()
                {
                    Filter = _fileFilter,
                    Title = $"Save {_fileTypeName} to File"
                };

                if (sfd.ShowDialog() == true)
                {
                    try
                    {
                        InternalSave(sfd.FileName);
                    }
                    catch(Exception e)
                    {
                        MessageBox.Show($"Failed to load {_fileTypeName}:\n{e.Message}", "Failed to load file!", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }

                    LoadedFilePath = sfd.FileName;
                }
                else
                    return false;
            }
            else
            {
                try
                {
                    InternalSave(LoadedFilePath);
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Failed to load {_fileTypeName}:\n{e.Message}", "Failed to load file!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }

            _savePin = _pinTracker?.PinCurrent();
            return true;
        }

        public bool Open()
        {
            if (!ResetConfirmation())
                return false;

            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = _fileFilter,
                Title = $"Load from {_fileTypeName} File"
            };

            if (ofd.ShowDialog() != true)
            {
                return false;
            }

            try
            {
                InternalLoad(ofd.FileName);
            }
            catch (Exception e)
            {
                MessageBox.Show($"Failed to load {_fileTypeName}:\n{e.Message}", "Failed to load file!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            LoadedFilePath = ofd.FileName;
            _savePin = _pinTracker?.PinCurrent();
            return true;
        }

        public void Reset()
        {
            if (!ResetConfirmation())
                return;
            InternalReset();
            _savePin = _pinTracker?.PinCurrent();
        }

        protected abstract void InternalReset();

        protected abstract void InternalSave(string filePath);

        protected abstract void InternalLoad(string filePath);

        public void CopyPin(BaseFileHandler handler)
        {
            _savePin = handler._savePin;
        }
    }
}
