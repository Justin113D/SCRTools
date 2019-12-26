using SCRLanguageEditor.Data;
using SCRCommon.Viewmodels;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System;
using System.Windows;

namespace SCRLanguageEditor.Viewmodel
{
    public class VM_Main : BaseViewModel
    {
        private HeaderNode format;

        public ObservableCollection<VM_Node> Nodes { get; private set; }

        public RelayCommand Cmd_LoadFormat { get; private set; }
        public RelayCommand Cmd_NewFile { get; private set; }
        public RelayCommand Cmd_Open { get; private set; }
        public RelayCommand Cmd_Save { get; private set; }
        public RelayCommand Cmd_SaveAs { get; private set; }
        public RelayCommand Cmd_Settings { get; private set; }

        private readonly VM_Settings settings;


        public string FormatVersion
        {
            get
            {
                return "Format Version:  " + format.Version;
            }
        }

        public string FileVersion
        {
            get
            {
                if (currentOpenFile == null)
                    return "No file loaded";
                else return "File version:  " + format.LoadedVersion;
            }
        }

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

        private string currentOpenFile = null;

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
            }
        }

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

        private void SetFormat()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "XML Files (*.xml)|*.xml";
            if (ofd.ShowDialog() == true)
            {
                LoadFormat(ofd.FileName);
            }
        }

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

        private void OpenFile()
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
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

        private void NewFile(bool reset)
        {
            MessageBoxResult r = MessageBox.Show("Do you want to save the current progress?\n All strings will be resettet!", "Warning!", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);
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
                    return;
            }

            CurrentOpenFile = null;
            if(reset)
            {
                format.ResetAllStrings();
            }
        }

        private void Save()
        {
            if(CurrentOpenFile == null)
            {
                SaveAs();
                return;
            }
            format.SaveContentsToFile(CurrentOpenFile);
        }

        private void SaveAs()
        {
            SaveFileDialog sfd = new SaveFileDialog()
            {
                Filter = "Language file|*.lang"
            };
            if(sfd.ShowDialog() == true)
            {
                CurrentOpenFile = sfd.FileName;
                Save();
            }
        }

        private void OpenSettings()
        {
            new SettingsWindow(settings).ShowDialog();
        }

    }
}
