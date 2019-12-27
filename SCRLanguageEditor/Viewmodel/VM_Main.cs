using SCRLanguageEditor.Data;
using SCRCommon.Viewmodels;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System;
using System.Windows;

namespace SCRLanguageEditor.Viewmodel
{
    /// <summary>
    /// Viewmodel for the main window
    /// </summary>
    public class VM_Main : BaseViewModel
    {
        /// <summary>
        /// The Header node, which contains all necessary data for the format and the current editing progress
        /// </summary>
        private HeaderNode format;

        /// <summary>
        /// The nodes used for the treeview
        /// </summary>
        public ObservableCollection<VM_Node> Nodes { get; private set; }

        /// <summary>
        /// Command for the "load format" button
        /// </summary>
        public RelayCommand Cmd_LoadFormat { get; private set; }
        /// <summary>
        /// Command for the "new file" button
        /// </summary>
        public RelayCommand Cmd_NewFile { get; private set; }
        /// <summary>
        /// Command for the "Open File" button
        /// </summary>
        public RelayCommand Cmd_Open { get; private set; }
        /// <summary>
        /// Command for the "save" button
        /// </summary>
        public RelayCommand Cmd_Save { get; private set; }
        /// <summary>
        /// Command for the "save as" button
        /// </summary>
        public RelayCommand Cmd_SaveAs { get; private set; }
        /// <summary>
        /// Command for the "settings" button
        /// </summary>
        public RelayCommand Cmd_Settings { get; private set; }

        /// <summary>
        /// Settings viewmodel, used for initializing the settings window
        /// </summary>
        private readonly VM_Settings settings;

        /// <summary>
        /// Redirects to the format version of the format object
        /// </summary>
        public string FormatVersion
        {
            get
            {
                return "Format Version:  " + format.Version;
            }
        }

        /// <summary>
        /// Redirects to the format target name of the format object
        /// </summary>
        public string FormatTargetName
        {
            get
            {
                return "Target Name:  " + format.targetName;
            }
        }

        /// <summary>
        /// Redirects to the loaded file version of the format object
        /// </summary>
        public string FileVersion
        {
            get
            {
                if (currentOpenFile == null)
                    return "No file loaded";
                else return "File version:  " + format.LoadedVersion;
            }
        }

        /// <summary>
        /// Redirects to the language of the loaded file 
        /// </summary>
        public string Language
        {
            get
            {
                return format.Language;
            }
            set
            {
                format.Language = value;
            }
        }

        /// <summary>
        /// Redirects to the author of the loaded file
        /// </summary>
        public string Author
        {
            get
            {
                return format.Author ?? "";
            }
            set
            {
                format.Author = value;
            }
        }

        /// <summary>
        /// stores the path of the currentlly open file. If no file has been open/saved to, it will be null
        /// </summary>
        private string currentOpenFile = null;

        /// <summary>
        /// Redirects to <see cref="currentOpenFile"/> with a custom setter
        /// </summary>
        private string CurrentOpenFile
        {
            get
            {
                return currentOpenFile;
            }
            set
            {
                currentOpenFile = value;
                OnPropertyChanged(nameof(FileVersion));
                OnPropertyChanged(nameof(Author));
                OnPropertyChanged(nameof(Language));
                UpdateNodes();
            }
        }

        /// <summary>
        /// Sets up the viewmodel 
        /// </summary>
        /// <param name="settings">The settings viewmodel, which was created before in order to load the correct settings</param>
        public VM_Main(VM_Settings settings)
        {
            this.settings = settings;
            Cmd_LoadFormat = new RelayCommand(() => SetFormat());
            Cmd_NewFile = new RelayCommand(() => NewFile(true));
            Cmd_Open = new RelayCommand(() => OpenFile());
            Cmd_Save = new RelayCommand(() => Save());
            Cmd_SaveAs = new RelayCommand(() => SaveAs());
            Cmd_Settings = new RelayCommand(() => OpenSettings());
            LoadFormat(settings.DefaultFormatPath);
        }

        /// <summary>
        /// Opens a file dialog to chose and load a format file
        /// </summary>
        private void SetFormat()
        {
            if (!NewFile(false)) return;
            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Open format file",
                Filter = "XML Files (*.xml)|*.xml"
            };
            if (ofd.ShowDialog() == true)
            {
                LoadFormat(ofd.FileName);
            }
        }

        /// <summary>
        /// Loads a format from a file
        /// </summary>
        /// <param name="path">The path to the file</param>
        private void LoadFormat(string path)
        {
            try
            {
                format = FileLoader.LoadXMLFile(path);
            }
            catch(Exception e)
            {
                string excName = e.GetType().ToString();
                int index = excName.LastIndexOf('.') + 1;
                excName = excName.Substring(index, excName.Length - index);
                MessageBox.Show("An error occured: " + excName + "\n " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            currentOpenFile = null;
            Nodes = new ObservableCollection<VM_Node>();
            foreach (Node n in format?.ChildNodes)
            {
                switch (n.Type)
                {
                    case Node.NodeType.ParentNode:
                        Nodes.Add(new VM_ParentNode((ParentNode)n));
                        break;
                    case Node.NodeType.StringNode:
                        Nodes.Add(new VM_StringNode((StringNode)n));
                        break;
                    default:
                        Nodes.Add(null);
                        break;
                }
            }
        }

        /// <summary>
        /// Opens a file dialog to choose a file to open
        /// </summary>
        private void OpenFile()
        {
            if (!NewFile(false)) return;

            OpenFileDialog ofd = new OpenFileDialog
            {
                Title ="Open language file",
                Filter = "Lang Files (*.lang)|*.lang;*.lang.base"
            };
            if (ofd.ShowDialog() == true)
            {
                try
                {
                    format.LoadContentsFromFile(ofd.FileName);
                    CurrentOpenFile = ofd.FileName;
                }
                catch(Exception e)
                {
                    string excName = e.GetType().ToString();
                    int index = excName.LastIndexOf('.') + 1;
                    excName = excName.Substring(index, excName.Length - index);
                    MessageBox.Show("An error occured: " + excName + "\n " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Asks the user whether they want to save the current changes, and then sets the opened file to null
        /// </summary>
        /// <param name="reset">Whether the changes should be reverted</param>
        /// <returns>Whether the operation was not cancelled</returns>
        private bool NewFile(bool reset)
        {
            MessageBoxResult r = MessageBox.Show("Unsaved changes will be reset!\nDo you want to save before?", "Warning!", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);
            switch (r)
            {
                case MessageBoxResult.Yes:
                    Save();
                    break;
                case MessageBoxResult.No:
                    break;
                case MessageBoxResult.None:
                case MessageBoxResult.Cancel:
                default:
                    return false;
            }

            CurrentOpenFile = null;
            if(reset)
            {
                format.ResetAllStrings();
            }
            return true;
        }

        /// <summary>
        /// Saves the changes to the currently set path. if no path is set, <see cref="SaveAs"/> will be called
        /// </summary>
        private void Save()
        {
            if(CurrentOpenFile == null)
            {
                SaveAs();
                return;
            }
            format.SaveContentsToFile(CurrentOpenFile);
        }

        /// <summary>
        /// Opens a file dialog to select a location to save the changes to
        /// </summary>
        private void SaveAs()
        {
            SaveFileDialog sfd = new SaveFileDialog()
            {
                Title = "Save to language file",
                Filter = "Language file|*.lang"
            };
            if(sfd.ShowDialog() == true)
            {
                CurrentOpenFile = sfd.FileName;
                Save();
            }
        }

        /// <summary>
        /// Creates a settings dialog
        /// </summary>
        private void OpenSettings()
        {
            new SettingsWindow(settings).ShowDialog();
        }

        /// <summary>
        /// Updates all currently existing node viewmodels
        /// </summary>
        private void UpdateNodes()
        {
            foreach(VM_Node n in Nodes)
            {
                n.UpdateProperties();
            }
        }
    }
}
