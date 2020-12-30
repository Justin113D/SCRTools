using Microsoft.Win32;
using SCRCommon.Viewmodels;
using SCRLanguageEditor.Data;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Collections.Generic;

namespace SCRLanguageEditor.Viewmodel
{
    /// <summary>
    /// Viewmodel for the main window
    /// </summary>
    public class VM_Main : BaseViewModel
    {
        #region Relay commands

        /// <summary>
        /// Command for the "new format" button
        /// </summary>
        public RelayCommand Cmd_NewFormat { get; }
        /// <summary>
        /// Command for the "load format" button
        /// </summary>
        public RelayCommand Cmd_LoadFormat { get; }
        /// <summary>
        /// Command for the "Save Format" button
        /// </summary>
        public RelayCommand Cmd_SaveFormat { get; }
        /// <summary>
        /// Command for the "Save format as..." button
        /// </summary>
        public RelayCommand Cmd_SaveFormatAs { get; }
        /// <summary>
        /// Command for the "new file" button
        /// </summary>
        public RelayCommand Cmd_NewFile { get; }
        /// <summary>
        /// Command for the "Open File" button
        /// </summary>
        public RelayCommand Cmd_Open { get; }
        /// <summary>
        /// Command for the "save" button
        /// </summary>
        public RelayCommand Cmd_Save { get; }
        /// <summary>
        /// Command for the "save as" button
        /// </summary>
        public RelayCommand Cmd_SaveAs { get; }
        /// <summary>
        /// Command for the "settings" button
        /// </summary>
        public RelayCommand Cmd_Settings { get; }

        public RelayCommand Cmd_AddStringNode { get; }

        public RelayCommand Cmd_AddParentNode { get; }

        #endregion

        public VM_HeaderNode Format { get; private set; }

        /// <summary>
        /// Settings viewmodel, used for initializing the settings window
        /// </summary>
        public VM_Settings Settings { get; }

        /// <summary>
        /// Error/warning message that is displayed in the main window
        /// </summary>
        public string Message { get; set; }

        public bool Dragging { get; set; }

        /// <summary>
        /// Sets up the viewmodel 
        /// </summary>
        /// <param name="settings">The settings viewmodel, which was created before in order to load the correct settings</param>
        public VM_Main()
        {
            Settings = new VM_Settings();
            
            Cmd_NewFormat = new RelayCommand(() => NewFormat());
            Cmd_LoadFormat = new RelayCommand(() => OpenFormat());
            Cmd_SaveFormat = new RelayCommand(() => Format.SaveFormat(false));
            Cmd_SaveFormatAs = new RelayCommand(() => Format.SaveFormat(true));

            Cmd_NewFile = new RelayCommand(() => Format.ResetContent());
            Cmd_Open = new RelayCommand(() => Format.LoadContentsFromFile());
            Cmd_Save = new RelayCommand(() => Format.SaveContent(false));
            Cmd_SaveAs = new RelayCommand(() => Format.SaveContent(true));

            Cmd_Settings = new RelayCommand(() => OpenSettings());
            Cmd_AddStringNode = new RelayCommand(() => Format.AddStringNode());
            Cmd_AddParentNode = new RelayCommand(() => Format.AddParentNode());

            if(string.IsNullOrWhiteSpace(Properties.Settings.Default.DefaultFormatPath))
                Format = new VM_HeaderNode(this);
            else
                Format = new VM_HeaderNode(this, Properties.Settings.Default.DefaultFormatPath);
        }

        /// <summary>
        /// Creates a new format file
        /// </summary>
        private void NewFormat()
        {
            if(!Properties.Settings.Default.DevMode)
                return;
            Format = new VM_HeaderNode(this);
        }

        /// <summary>
        /// Opens a dialog to open a format file
        /// </summary>
        private void OpenFormat()
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Open format file",
                Filter = "JSON Files (*.json)|*.json"
            };

            if(ofd.ShowDialog() != true)
                return;

            VM_HeaderNode newFormat = new VM_HeaderNode(this, ofd.FileName);
            if(newFormat.FormatFilePath != ofd.FileName)
                return;
            Format = newFormat;
        }

        /// <summary>
        /// Creates a settings dialog
        /// </summary>
        private void OpenSettings() => new SettingsWindow(Settings).ShowDialog();

    }
}
