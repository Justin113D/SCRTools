using Microsoft.Win32;
using SCRCommon.Viewmodels;
using System;

namespace SCRLanguageEditor.Viewmodel
{
    /// <summary>
    /// Viewmodel for the main window
    /// </summary>
    public class VM_Main : BaseViewModel
    {
        #region Relay commands

        /// <summary>
        /// Command for the "load format" button
        /// </summary>
        public RelayCommand Cmd_LoadFormat { get; }
        /// <summary>
        /// Command for the "Save Format" button
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
        /// <summary>
        /// Gets called by the top-level "Add String" button
        /// </summary>
        public RelayCommand Cmd_AddStringNode { get; }
        /// <summary>
        /// Gets called by the top-level "Add Parent" button
        /// </summary>
        public RelayCommand Cmd_AddParentNode { get; }
        /// <summary>
        /// Bound to Ctrl+Z
        /// </summary>
        public RelayCommand Cmd_Undo { get; }
        /// <summary>
        /// Bound to Ctrl+Y
        /// </summary>
        public RelayCommand Cmd_Redo { get; }
        /// <summary>
        /// Used to Expand all nodes. <br/>
        /// Calls <see cref="VM_HeaderNode.ExpandAll"/>
        /// </summary>
        public RelayCommand Cmd_ExpandAll { get; }
        /// <summary>
        /// Used to Collapse all nodes. <br/>
        /// Calls <see cref="VM_HeaderNode.CollapseAll"/>
        /// </summary>
        public RelayCommand Cmd_CollapseAll { get; }
        #endregion

        /// <summary>
        /// Settings viewmodel, used for initializing the settings window
        /// </summary>
        public VM_Settings Settings { get; }

        /// <summary>
        /// currently used format viewmodel
        /// </summary>
        public VM_HeaderNode Format { get; private set; }

        /// <summary>
        /// Error/warning message that is displayed in the main window
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Whether a node is being dragged
        /// </summary>
        public bool Dragging { get; set; }

        public Action UpdateTextBox { get; set; }

        public Action ShowWaitCursor { get; set; }

        /// <summary>
        /// Sets up the viewmodel 
        /// </summary>
        /// <param name="settings">The settings viewmodel, which was created before in order to load the correct settings</param>
        public VM_Main()
        {
            Settings = new VM_Settings(this);
            Cmd_LoadFormat = new RelayCommand(OpenFormat);

            Cmd_NewFile = new RelayCommand(NewFile);
            Cmd_Open = new RelayCommand(OpenFile);
            Cmd_Save = new RelayCommand(SaveFile);
            Cmd_SaveAs = new RelayCommand(SaveFileAs);

            Cmd_Settings = new RelayCommand(OpenSettings);
            Cmd_AddStringNode = new RelayCommand(() => Format.AddStringNode());
            Cmd_AddParentNode = new RelayCommand(() => Format.AddParentNode());

            Cmd_Undo = new RelayCommand(Undo);
            Cmd_Redo = new RelayCommand(Redo);
            Cmd_ExpandAll = new RelayCommand(() => Format.ExpandAll());
            Cmd_CollapseAll = new RelayCommand(() => Format.CollapseAll());

            if(string.IsNullOrWhiteSpace(Properties.Settings.Default.DefaultFormatPath))
                Format = new VM_HeaderNode(this);
            else
                Format = new VM_HeaderNode(this, Properties.Settings.Default.DefaultFormatPath);
        }

        private void NewFile()
        {
            if(Properties.Settings.Default.DevMode)
                NewFormat();
            else
                Format.ResetContent();
        }

        private void OpenFile()
        {
            if(Properties.Settings.Default.DevMode)
                OpenFormat();
            else
                Format.LoadContentsFromFile();
        }

        private void SaveFile()
        {
            if(Properties.Settings.Default.DevMode)
                Format.SaveFormat(false);
            else
                Format.SaveContent(false);
        }

        private void SaveFileAs()
        {
            if(Properties.Settings.Default.DevMode)
                Format.SaveFormat(true);
            else
                Format.SaveContent(true);
        }

        /// <summary>
        /// Creates a new format file
        /// </summary>
        private void NewFormat()
        {
            if(!Format.OverwriteConfirmation())
                return;
            Format = new VM_HeaderNode(this);
        }

        /// <summary>
        /// Opens a dialog to open a format file
        /// </summary>
        private void OpenFormat()
        {
            if(!Format.OverwriteConfirmation())
                return;

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

        /// <summary>
        /// Performs an Undo. If a textbox is focused, the binding will be updated
        /// </summary>
        private void Undo()
        {
            UpdateTextBox.Invoke();
            Format.Tracker.Undo();
        }

        /// <summary>
        /// Performs an Undo. If a textbox is focused, the binding will be updated
        /// </summary>
        private void Redo()
        {
            UpdateTextBox.Invoke();
            Format.Tracker.Redo();
        }

    }
}