using Microsoft.Win32;
using SCRCommon.Viewmodels;
using SCRLanguageEditor.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Linq;

namespace SCRLanguageEditor.Viewmodel
{
    public class VM_HeaderNode : BaseViewModel
    {
        /// <summary>
        /// Main view model access
        /// </summary>
        private readonly VM_Main _mainViewModel;

        /// <summary>
        /// Data object
        /// </summary>
        private readonly HeaderNode _node;

        public ObservableCollection<VM_Node> Children { get; private set; }

        /// <summary>
        /// Filepath to the loaded format file
        /// </summary>
        public string FormatFilePath { get; private set; }

        /// <summary>
        /// Filepath to the loaded language file
        /// </summary>
        private string _contentFilePath;

        /// <summary>
        /// The name of the node
        /// </summary>
        public string FormatTargetName
        {
            get => _node.Name;
            set
            {
                if(string.IsNullOrWhiteSpace(value))
                    return;

                _node.Name = value;
            }
        }

        /// <summary>
        /// The description of the node
        /// </summary>
        public string Description
        {
            get => _node.Description;
            set
            {
                if(string.IsNullOrWhiteSpace(value))
                    return;

                _node.Description = value;
            }
        }

        /// <summary>
        /// Redirects to the loaded language
        /// </summary>
        public string Language
        {
            get => _node.Language;
            set => _node.Language = value;
        }

        /// <summary>
        /// Redirects to the author of the loaded file
        /// </summary>
        public string Author
        {
            get => _node.Author ?? "";
            set => _node.Author = value;
        }

        /// <summary>
        /// Redirects to the format version of the format object
        /// </summary>
        public string FormatVersion
        {
            get => _node.Version.ToString();
            set
            {
                try
                {
                    if(_node.StringNodes.Count == 0)
                        _node.Versions.Clear();
                    else
                    {
                        while(!_node.StringNodes.Any(x => x.Value.VersionID == _node.Versions.Count - 1))
                            _node.Versions.RemoveAt(_node.Versions.Count - 1);
                    }
                    _node.Versions.Add(new Version(value));
                    _mainViewModel.Message = "";
                }
                catch(FormatException)
                {
                    _mainViewModel.Message = "Version not valid! Please follow the pattern: Major.Minor.Build.Revision";
                }
            }
        }

        /// <summary>
        /// The newest format version id
        /// </summary>
        public int NewestFormatID => _node.Versions.Count - 1;

        /// <summary>
        /// Redirects to the loaded file version of the format object
        /// </summary>
        public string FileVersion => _node.LoadedVersion == null ? "No file loaded" : "File version:  " + _node.LoadedVersion;

        /// <summary>
        /// Creates a new empty header node viewmodel
        /// </summary>
        /// <param name="mainViewModel">Main viewmodel</param>
        public VM_HeaderNode(VM_Main mainViewModel)
        {
            _mainViewModel = mainViewModel;
            _node = new HeaderNode();
            Children = new ObservableCollection<VM_Node>();
        }

        /// <summary>
        /// Creates a viewmodel off a format file
        /// </summary>
        /// <param name="mainViewModel">Main viewmodel</param>
        /// <param name="formatPath">Path to the format file</param>
        public VM_HeaderNode(VM_Main mainViewModel, string formatPath)
        {
            _mainViewModel = mainViewModel;

            try
            {
                _node = HeaderNode.LoadFormatFromFile(formatPath);
            }
            catch(Exception e)
            {
                _node = new HeaderNode();
                Children = new ObservableCollection<VM_Node>();
                MessageBox.Show("An error occured while loading the format: " + e.GetType().Name + "\n " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            FormatFilePath = formatPath;

            UpdateChildren();
        }

        public void UpdateChildren()
        {
            List<VM_Node> nodes = new List<VM_Node>();
            foreach(Node n in _node.ChildNodes)
            {
                switch(n.Type)
                {
                    case Node.NodeType.ParentNode:
                        nodes.Add(new VM_ParentNode(null, (ParentNode)n, this));
                        break;
                    case Node.NodeType.StringNode:
                        nodes.Add(new VM_StringNode(null, (StringNode)n, this));
                        break;
                }
            }
            Children = new ObservableCollection<VM_Node>(nodes);
        }

        /// <summary>
        /// Checks if the new key already exists. If not, the stringnode list will be adjusted
        /// </summary>
        /// <param name="newKey">New key</param>
        /// <param name="node">The node that is being changed</param>
        /// <returns></returns>
        public bool ChangeKey(string newKey, StringNode node)
        {
            newKey = newKey.ToLower();
            if(!_node.StringNodes.ContainsKey(newKey))
            {
                _node.StringNodes.Remove(node.Name.ToLower());
                _node.StringNodes.Add(newKey, node);
                _mainViewModel.Message = "";
                return true;
            }
            _mainViewModel.Message = $"Key \"{newKey}\" already exists";
            return false;
        }

        public void AddStringNode()
        {
            _node.NewStringNode();
            UpdateChildren();
        }

        public StringNode AddStringNode(ParentNode parent) 
            => _node.NewStringNode(parent);

        public void RemoveStringNode(StringNode node)
            => _node.StringNodes.Remove(node.Name.ToLower());

        public void AddParentNode()
        {
            _node.NewParentNode();
            UpdateChildren();
        }

        public void InsertChild(VM_Node node, int index)
        {
            _node.ChildNodes.Insert(index, node.Node);
            Children.Insert(index, node);
        }

        public void RemoveChild(VM_Node node)
        {
            _node.ChildNodes.Remove(node.Node);
            Children.Remove(node);
        }

        /// <summary>
        /// Writes the format to a file
        /// </summary>
        /// <param name="newPath">Whether to open a SafeFileDialog to change the file path</param>
        public void SaveFormat(bool newPath)
        {
            if(!Properties.Settings.Default.DevMode)
                return;

            if(newPath || FormatFilePath == null)
            {
                SaveFileDialog sfd = new SaveFileDialog()
                {
                    Title = "Save to format file",
                    Filter = "Json File|*.json",

                };

                if(sfd.ShowDialog() != true)
                    return;

                FormatFilePath = sfd.FileName;
            }

            _node.SaveFormatToFile(FormatFilePath);
        }

        private bool ContentConfirmation()
        {
            // TODO check if things changed at all

            MessageBoxResult r = MessageBox.Show("Unsaved changes will be reset!\nDo you want to save before?", "Warning!", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);
            switch(r)
            {
                case MessageBoxResult.Yes:
                    SaveContent(false);
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
        
        /// <summary>
        /// Resets the strings to their default values
        /// </summary>
        public void ResetContent()
        {
            if(!ContentConfirmation())
                return;
            _node.ResetAllStrings();
        }

        /// <summary>
        /// Loads a language file
        /// </summary>
        public void LoadContentsFromFile()
        {
            if(!ContentConfirmation())
                return;

            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Open language file",
                Filter = "Lang Files (*.lang)|*.lang;*.lang.base"
            };

            if(ofd.ShowDialog() != true)
                return;

            try
            {
                _node.LoadContentsFromFile(ofd.FileName);
                _contentFilePath = ofd.FileName;
            }
            catch(Exception e)
            {
                MessageBox.Show("An error occured while loading the file: " + e.GetType().Name + "\n " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            OnPropertyChanged(nameof(FileVersion));
            OnPropertyChanged(nameof(Author));
            OnPropertyChanged(nameof(Language));

            foreach(VM_Node n in Children)
            {
                n.UpdateProperties();
            }
        }

        /// <summary>
        /// Writes the language data to a file
        /// </summary>
        /// <param name="newPath">Whether to open a SafeFileDialog to change the file path</param>
        public void SaveContent(bool newPath)
        {
            if(newPath || _contentFilePath == null)
            {
                SaveFileDialog sfd = new SaveFileDialog()
                {
                    Title = "Save to language file",
                    Filter = "Language file|*.lang"
                };

                if(sfd.ShowDialog() != true)
                    return;

                _contentFilePath = sfd.FileName;
            }

            _node.SaveContentsToFile(_contentFilePath);
        }
    }
}
