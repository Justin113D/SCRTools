using Microsoft.Win32;
using SCRCommon.Viewmodels;
using System;
using System.Diagnostics;
using System.Timers;
using System.Windows;
using System.Windows.Media;

namespace SCRTranslationEditor.Viewmodel
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

        public RelayCommand Cmd_OpenWiki { get; }
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
        public string Message { get; private set; }

        /// <summary>
        /// Color of the message
        /// </summary>
        public SolidColorBrush MessageColor { get; private set; }

        /// <summary>
        /// Used to display the message
        /// </summary>
        public Visibility ShowMessage { get; private set; } = Visibility.Hidden;

        /// <summary>
        /// Whether a node is being dragged
        /// </summary>
        public bool Dragging { get; set; }

        /// <summary>
        /// An action passed down by the mainwindow. Forces the binding to send its data to the viewmodel
        /// </summary>
        public Action UpdateTextBox { get; set; }

        /// <summary>
        /// Shows the wait cursor until the next update
        /// </summary>
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
            Cmd_OpenWiki = new RelayCommand(OpenWiki);

            MessageColor = new SolidColorBrush(Colors.Transparent);

            if(string.IsNullOrWhiteSpace(Properties.Settings.Default.DefaultFormatPath))
                Format = new VM_HeaderNode(this);
            else
                Format = new VM_HeaderNode(this, Properties.Settings.Default.DefaultFormatPath);
        }

        #region File handling

        /// <summary>
        /// Creates a new set of data
        /// </summary>
        private void NewFile()
        {
            if(Properties.Settings.Default.DevMode)
                NewFormat();
            else
                Format.ResetContent();
        }

        /// <summary>
        /// Reads data from a file
        /// </summary>
        private void OpenFile()
        {
            if(Properties.Settings.Default.DevMode)
                OpenFormat();
            else
                Format.LoadContentsFromFile();
        }

        /// <summary>
        /// Save the current data into a file. If no file is validly open, it will jump to <see cref="SaveFileAs"/>
        /// </summary>
        private void SaveFile()
        {
            UpdateTextBox.Invoke();
            if(Properties.Settings.Default.DevMode)
            {
                if (Format.SaveFormat(false))
                {
                    SetMessage("Format Saved!");
                }
            }
            else
            {
                if(Format.SaveContent(false))
                {
                    SetMessage("Content Saved!");
                }
            }
        }

        /// <summary>
        /// Saves the current data into a new file
        /// </summary>
        private void SaveFileAs()
        {
            UpdateTextBox.Invoke();
            if(Properties.Settings.Default.DevMode)
            {
                if(Format.SaveFormat(true))
                {
                    SetMessage("Format Saved!");
                }
            }
            else
            {
                if(Format.SaveContent(true))
                {
                    SetMessage("Content Saved!");
                }
            }
        }

        /// <summary>
        /// Creates a new format file
        /// </summary>
        private void NewFormat()
        {
            UpdateTextBox.Invoke();
            if(!Format.OverwriteConfirmation())
                return;
            Format = new VM_HeaderNode(this);
            ResetMessage();
        }

        /// <summary>
        /// Opens a dialog to open a format file
        /// </summary>
        private void OpenFormat()
        {
            UpdateTextBox.Invoke();
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
            ResetMessage();
        }

        #endregion

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
            if(Format.Tracker.Undo())
            {
                SetMessage("Performed Undo!");
            }
        }

        /// <summary>
        /// Performs an Undo. If a textbox is focused, the binding will be updated
        /// </summary>
        private void Redo()
        {
            UpdateTextBox.Invoke();
            if(Format.Tracker.Redo())
            {
                SetMessage("Performed Redo!");
            }
        }

        /// <summary>
        /// Resets the message
        /// </summary>
        public void ResetMessage()
        {
            if(Message == "")
                return;
            Message = "";
            ShowMessage = Visibility.Hidden;
            MessageColor = new SolidColorBrush(Colors.Transparent);
        }

        /// <summary>
        /// Sets the message and the message type
        /// </summary>
        /// <param name="newMessage">New message string</param>
        /// <param name="warning">Whether the message is a warning</param>
        public void SetMessage(string newMessage, bool warning = false)
        {
            Message = newMessage;
            ShowMessage = Visibility.Visible;

            if(warning)
            {
                // red
                MessageColor = new SolidColorBrush(SCRCommon.WpfStyles.Colors.Red);
            }
            else
            {
                // green
                MessageColor = new SolidColorBrush(SCRCommon.WpfStyles.Colors.Green);
            }

            OnPropertyChanged(nameof(ShowMessage));
        }
    
        private void OpenWiki()
        {
            var psi = new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = "https://github.com/Justin113D/SCRTools/wiki/Language-Editor"
            };
            Process.Start(psi);   
        }
    }
}