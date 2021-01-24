using Microsoft.Win32;
using SCRCommon.Viewmodels;
using SCRTranslationEditor.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace SCRTranslationEditor.Viewmodel
{
    public class VM_HeaderNode : BaseViewModel
    {
        /// <summary>
        /// Undo/Redo tracker
        /// </summary>
        public ChangeTracker Tracker { get; }

        #region private members

        /// <summary>
        /// Main view model access
        /// </summary>
        public VM_Main MainViewModel { get; }

        /// <summary>
        /// Data object
        /// </summary>
        private readonly HeaderNode _node;

        /// <summary>
        /// Pin for when the program was saved last
        /// </summary>
        private ChangeTracker.Pin _savePin;

        /// <summary>
        /// Whether the savebin is from devmode or contentmode
        /// </summary>
        private bool _devmodePin;

        /// <summary>
        /// Filepath to the loaded language file
        /// </summary>
        private string _contentFilePath;

        /// <summary>
        /// Whether every parent node has been expanded at least once
        /// </summary>
        private bool _expandedAll;

        #endregion

        #region Properties

        public ObservableCollection<VM_Node> Children { get; private set; }

        /// <summary>
        /// Filepath to the loaded format file
        /// </summary>
        public string FormatFilePath { get; private set; }

        /// <summary>
        /// The name of the node
        /// </summary>
        public string FormatTargetName
        {
            get => _node.Name;
            set
            {
                if(string.IsNullOrWhiteSpace(value) || _node.Name == value)
                    return;

                Tracker.TrackChange(new ChangedValue<string>((v) =>
                {
                    _node.Name = v;
                    OnPropertyChanged(nameof(FormatTargetName));
                }, _node.Name, value));

                _node.Name = value;
                MainViewModel.ResetMessage();
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
                if(_node.Description == value)
                    return;

                Tracker.TrackChange(new ChangedValue<string>((v) =>
                {
                    _node.Description = v;
                    OnPropertyChanged(nameof(Description));
                }, _node.Description, value));

                _node.Description = value;
                MainViewModel.ResetMessage();
            }
        }

        /// <summary>
        /// Redirects to the loaded language
        /// </summary>
        public string Language
        {
            get => _node.Language;
            set
            {
                if(string.IsNullOrWhiteSpace(value) || _node.Language == value)
                    return;

                Tracker.TrackChange(new ChangedValue<string>((v) =>
                {
                    _node.Language = v;
                    OnPropertyChanged(nameof(Language));
                }, _node.Language, value));

                _node.Language = value;
                MainViewModel.ResetMessage();
            }
        }

        /// <summary>
        /// Redirects to the author of the loaded file
        /// </summary>
        public string Author
        {
            get => _node.Author ?? "";
            set
            {
                if(_node.Author == (string.IsNullOrWhiteSpace(value) ? null : value))
                    return;

                Tracker.TrackChange(new ChangedValue<string>((v) =>
                {
                    _node.Author = v;
                    OnPropertyChanged(nameof(Author));
                }, _node.Author, value));

                _node.Author = value;
                MainViewModel.ResetMessage();
            }
        }

        /// <summary>
        /// Redirects to the format version of the format object
        /// </summary>
        public string FormatVersion
        {
            get => _node.Version.ToString();
            set
            {
                Version newVersion = null;
                try
                {
                    newVersion = new Version(value);
                }
                catch(FormatException)
                {
                    MainViewModel.SetMessage("Version not valid! Please follow the pattern: Major.Minor.Build.Revision", true);
                }

                var oldState = _node.Versions.ToArray();

                if(_node.StringNodes.Count == 0)
                    _node.Versions.Clear();
                else
                {
                    List<Version> removedVersions = new List<Version>();
                    while(!_node.StringNodes.Any(x => x.Value.VersionID == _node.Versions.Count - 1))
                    {
                        removedVersions.Insert(0, _node.Versions[^1]);
                        _node.Versions.RemoveAt(_node.Versions.Count - 1);
                    }
                }

                _node.Versions.Add(newVersion);
                MainViewModel.ResetMessage();

                Tracker.TrackChange(new ChangedList<Version>(_node.Versions, oldState, () =>
                {
                    OnPropertyChanged(FormatVersion);
                }));
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
        /// Nodes that have been translated already
        /// </summary>
        public int NodesTranslated { get; private set; }

        /// <summary>
        /// Nodes that need to be updated
        /// </summary>
        public int NodesNeedUpdate { get; private set; }

        /// <summary>
        /// Nodes that need to be translated
        /// </summary>
        public int NodesNeedTranslation { get; private set; }

        #endregion

        /// <summary>
        /// Creates a new empty header node viewmodel
        /// </summary>
        /// <param name="mainViewModel">Main viewmodel</param>
        public VM_HeaderNode(VM_Main mainViewModel)
        {
            Tracker = new ChangeTracker();
            UpdatePin();
            MainViewModel = mainViewModel;
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
            Tracker = new ChangeTracker();
            UpdatePin();

            MainViewModel = mainViewModel;

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

            Expand();
        }

        private void UpdatePin()
        {
            _savePin = Tracker.PinCurrent();
            _devmodePin = Properties.Settings.Default.DevMode;
        }

        /// <summary>
        /// Called upon changing to/from dev mode
        /// </summary>
        public void ChangedMode(bool devmode)
        {
            if(devmode == Properties.Settings.Default.DevMode)
                return;

            if(!_savePin.CheckValid() && !Tracker.ResetOnNextChange)
            {
                if(devmode)
                {
                    MessageBoxResult r = MessageBox.Show("You are about to switch to Devmode, and did not save your changes!\nDo you want to save your content before?", "Warning!", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);
                    switch(r)
                    {
                        case MessageBoxResult.Yes:
                            if(!SaveContent(false))
                                return;
                            break;
                        case MessageBoxResult.No:
                            break;
                        case MessageBoxResult.None:
                        case MessageBoxResult.Cancel:
                        default:
                            return;
                    }
                }
                else
                {
                    MessageBoxResult r = MessageBox.Show("You are about to switch to Contentmode, and did not save your changes!\nDo you want to save your format before?", "Warning!", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);
                    switch(r)
                    {
                        case MessageBoxResult.Yes:
                            if(!SaveContent(false))
                                return;
                            break;
                        case MessageBoxResult.No:
                            break;
                        case MessageBoxResult.None:
                        case MessageBoxResult.Cancel:
                        default:
                            return;
                    }
                }
            }

            Properties.Settings.Default.DevMode = devmode;
            Tracker.ResetOnNextChange = !Tracker.ResetOnNextChange;
            RefreshHierarchy();

            if(!Properties.Settings.Default.DevMode)
            {
                NodesTranslated = 0;
                NodesNeedUpdate = 0;
                NodesNeedTranslation = 0;

                foreach(var p in _node.StringNodes)
                {
                    switch(p.Value.NodeState)
                    {
                        case 0:
                            NodesNeedTranslation++;
                            break;
                        case 1:
                            NodesNeedUpdate++;
                            break;
                        case 2:
                        case 3:
                            NodesTranslated++;
                            break;
                    }
                }
            }
            MainViewModel.ResetMessage();
        }

        #region Hierarchy Methods

        private void Expand()
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

            NodesNeedTranslation = _node.StringNodes.Count;
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
            if(_node.StringNodes.ContainsKey(newKey))
            {
                MainViewModel.SetMessage($"Key \"{newKey}\" already exists", true);
                return false;
            }

            string oldKey = node.Name.ToLower();

            Tracker.TrackChange(new Change((b) =>
            {
                if(b) // redo
                {
                    _node.StringNodes.Remove(oldKey);
                    _node.StringNodes.Add(newKey, node);
                }
                else
                {
                    _node.StringNodes.Remove(newKey);
                    _node.StringNodes.Add(oldKey, node);
                }
            }));

            _node.StringNodes.Remove(oldKey);
            _node.StringNodes.Add(newKey, node);
            MainViewModel.ResetMessage();
            return true;
        }

        /// <summary>
        /// Removes a stringnode key from the format
        /// </summary>
        /// <param name="key"></param>
        public void RemoveKey(string key)
        {
            key = key.ToLower();
            StringNode node = _node.StringNodes[key];

            Tracker.TrackChange(new Change((b) =>
            {
                if(b) // redo
                    _node.StringNodes.Remove(key);
                else
                    _node.StringNodes.Add(key, node);
            }));

            _node.StringNodes.Remove(key);
        }

        /// <summary>
        /// Creates a new stringnode 
        /// </summary>
        /// <returns></returns>
        public StringNode NewKey()
        {
            StringNode node = _node.CreateStringNode();

            string key = node.Name.ToLower();

            Tracker.TrackChange(new Change((b) =>
            {
                if(b) // redo
                    _node.StringNodes.Add(key, node);
                else
                    _node.StringNodes.Remove(key);
            }));

            return node;
        }


        public void NodeUpdated(int oldstate, int newstate)
        {
            if(oldstate == newstate)
                return;

            switch(oldstate)
            {
                case 0:
                    NodesNeedTranslation--;
                    break;
                case 1:
                    NodesNeedUpdate--;
                    break;
                case 2:
                case 3:
                    NodesTranslated--;
                    break;
            }

            switch(newstate)
            {
                case 0:
                    NodesNeedTranslation++;
                    break;
                case 1:
                    NodesNeedUpdate++;
                    break;
                case 2:
                case 3:
                    NodesTranslated++;
                    break;
            }
        }

        /// <summary>
        /// Creates a top-level stringnode
        /// </summary>
        public void AddStringNode()
        {
            Tracker.BeginGroup();

            StringNode node = NewKey();
            VM_StringNode vmnode = new VM_StringNode(null, node, this);

            Tracker.TrackChange(new ChangedListSingleEntry<Node>(_node.ChildNodes, node, _node.ChildNodes.Count, null));
            Tracker.TrackChange(new ChangedListSingleEntry<VM_Node>(Children, vmnode, Children.Count, () => OnPropertyChanged(nameof(Children))));

            _node.ChildNodes.Add(node);
            Children.Add(vmnode);

            Tracker.EndGroup();
        }

        /// <summary>
        /// Creates a top-level parent node
        /// </summary>
        public void AddParentNode()
        {
            ParentNode node = new ParentNode("New Parent");
            VM_ParentNode vmnode = new VM_ParentNode(null, node, this);

            Tracker.BeginGroup();
            Tracker.TrackChange(new ChangedListSingleEntry<Node>(_node.ChildNodes, node, _node.ChildNodes.Count, null));
            Tracker.TrackChange(new ChangedListSingleEntry<VM_Node>(Children, vmnode, Children.Count, () => OnPropertyChanged(nameof(Children))));
            Tracker.EndGroup();

            _node.ChildNodes.Add(node);
            Children.Add(vmnode);
        }

        /// <summary>
        /// Moves a node to the top level hierarchy
        /// </summary>
        /// <param name="vmNode"></param>
        /// <param name="index"></param>
        public void InsertChild(VM_Node vmNode, int index)
        {
            Tracker.BeginGroup();
            Tracker.TrackChange(new ChangedListSingleEntry<Node>(_node.ChildNodes, vmNode.Node, index, null));
            Tracker.TrackChange(new ChangedListSingleEntry<VM_Node>(Children, vmNode, index, () => OnPropertyChanged(nameof(Children))));
            Tracker.EndGroup();

            _node.ChildNodes.Insert(index, vmNode.Node);
            Children.Insert(index, vmNode);
        }

        /// <summary>
        /// Removes a node from the top level hierarchy
        /// </summary>
        /// <param name="node"></param>
        public void RemoveChild(VM_Node vmNode)
        {
            Tracker.BeginGroup();
            Tracker.TrackChange(new ChangedListSingleEntry<Node>(_node.ChildNodes, vmNode.Node, null, null));
            Tracker.TrackChange(new ChangedListSingleEntry<VM_Node>(Children, vmNode, null, () => OnPropertyChanged(nameof(Children))));
            Tracker.EndGroup();

            _node.ChildNodes.Remove(vmNode.Node);
            Children.Remove(vmNode);
        }

        /// <summary>
        /// Expands the entire hierarchy
        /// </summary>
        public void ExpandAll()
        {
            if(_node.StringNodes.Count > 100 && !_expandedAll)
            {
                MessageBoxResult r = MessageBox.Show("This format has more than a hundred nodes! It could take a while to expand all of it. \n Proceed?", "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                if(r != MessageBoxResult.Yes)
                    return;
            }
            MainViewModel.ShowWaitCursor.Invoke();
            ChangeAllExpansions(Children, true);
            _expandedAll = true;
        }

        /// <summary>
        /// Collapses the entire tree
        /// </summary>
        public void CollapseAll() => ChangeAllExpansions(Children, false);

        /// <summary>
        /// Changes all expansions
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="state"></param>
        private void ChangeAllExpansions(ObservableCollection<VM_Node> nodes, bool state)
        {
            foreach(var n in nodes)
            {
                if(n == null)
                    return;

                if(n.Type == Node.NodeType.ParentNode)
                {
                    n.IsExpanded = state;
                    ChangeAllExpansions(((VM_ParentNode)n).Children, state);
                }
            }
        }

        /// <summary>
        /// Updates the entire hierarchy (wpf notifications)
        /// </summary>
        public void RefreshHierarchy()
        {
            foreach(VM_Node n in Children)
                n.UpdateProperties();
        }

        #endregion

        #region File methods

        /// <summary>
        /// Writes the format to a file
        /// </summary>
        /// <param name="newPath">Whether to open a SafeFileDialog to change the file path</param>
        public bool SaveFormat(bool newPath)
        {
            if(newPath || FormatFilePath == null)
            {
                SaveFileDialog sfd = new SaveFileDialog()
                {
                    Title = "Save to format file",
                    Filter = "Json File|*.json",
                };

                if(sfd.ShowDialog() != true)
                    return false;

                FormatFilePath = sfd.FileName;
            }

            _node.SaveFormatToFile(FormatFilePath);
            UpdatePin();
            return true;
        }

        /// <summary>
        /// Used to confirm whether the user wants to overwrite the current data
        /// </summary>
        /// <returns></returns>
        public bool OverwriteConfirmation()
        {
            if(_savePin.CheckValid())
                return true;

            MessageBoxResult r = MessageBox.Show("Unsaved changes will be reset!\nDo you want to save before?", "Warning!", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);
            switch(r)
            {
                case MessageBoxResult.Yes:
                    if(!SaveContent(false))
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

        /// <summary>
        /// Resets the strings to their default values
        /// </summary>
        public void ResetContent()
        {
            if(!OverwriteConfirmation())
                return;
            _node.ResetAllStrings();
            Tracker.Reset();

            foreach(var n in Children)
                n.UpdateProperties();

            _contentFilePath = null;
        }

        /// <summary>
        /// Loads a language file
        /// </summary>
        public void LoadContentsFromFile()
        {
            if(!OverwriteConfirmation())
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

            Tracker.Reset();
        }

        /// <summary>
        /// Writes the language data to a file
        /// </summary>
        /// <param name="newPath">Whether to open a SafeFileDialog to change the file path</param>
        public bool SaveContent(bool newPath)
        {
            if(newPath || _contentFilePath == null)
            {
                SaveFileDialog sfd = new SaveFileDialog()
                {
                    Title = "Save to language file",
                    Filter = "Language file|*.lang"
                };

                if(sfd.ShowDialog() != true)
                    return false;

                _contentFilePath = sfd.FileName;
            }

            _node.SaveContentsToFile(_contentFilePath);
            UpdatePin();
            return true;
        }

        #endregion
    }
}
