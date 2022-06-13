﻿using SCR.Tools.TranslationEditor.Data;
using SCR.Tools.UndoRedo;
using SCR.Tools.Viewmodeling;
using System.Collections.ObjectModel;
using System.IO;

namespace SCR.Tools.TranslationEditor.ProjectEditor.Viewmodeling
{
    /// <summary>
    /// Project viewmodel handling the format and data surrounding it
    /// </summary>
    public class VmProject : BaseViewModel
    {
        /// <summary>
        /// Headernode holding the format information
        /// </summary>
        public HeaderNode Header { get; }

        /// <summary>
        /// The amount of untranslated nodes in the project
        /// </summary>
        public int UntranslatedNodes { get; private set; }

        /// <summary>
        /// The amount of outdated nodes in the project
        /// </summary>
        public int OutdatedNodes { get; private set; }

        /// <summary>
        /// The amount of translated nodes in the project
        /// </summary>
        public int TranslatedNodes { get; private set; }


        /// <summary>
        /// Author of the project
        /// </summary>
        public string Author
        {
            get => Header.Author;
            set
            {
                if (Header.Author == value)
                    return;

                ChangeTracker.Global.BeginGroup();

                Header.Author = value;
                TrackNotifyProperty(nameof(Author));

                ChangeTracker.Global.EndGroup();
            }
        }

        /// <summary>
        /// Language that the project targets
        /// </summary>
        public string Language
        {
            get => Header.Language;
            set
            {
                if (Header.Language == value)
                    return;

                ChangeTracker.Global.BeginGroup();

                Header.Language = value;
                TrackNotifyProperty(nameof(Language));

                ChangeTracker.Global.EndGroup();
            }
        }

        /// <summary>
        /// Target name specified in the header
        /// </summary>
        public string TargetName
            => Header.Name;

        /// <summary>
        /// Current version in the header
        /// </summary>
        public string Version
            => Header.Version.ToString();

        /// <summary>
        /// Private collection for the node viewmodels
        /// </summary>
        private readonly ObservableCollection<VmNode> _nodes;

        /// <summary>
        /// Top level node viewmodels
        /// </summary>
        public ReadOnlyObservableCollection<VmNode> Nodes { get; }


        public VmProject(HeaderNode data)
        {
            Header = data;

            _nodes = new();
            Nodes = new(_nodes);
            CreateNodes();
            CountNodes();
        }


        /// <summary>
        /// Create the top level node viewmodels
        /// </summary>
        private void CreateNodes()
        {
            foreach (Node node in Header.ChildNodes)
            {
                if (node is ParentNode p)
                {
                    _nodes.Add(new VmParentNode(this, p));
                }
                else if (node is StringNode s)
                {
                    _nodes.Add(new VmStringNode(this, s));
                }
            }
        }

        /// <summary>
        /// Create the base 
        /// </summary>
        private void CountNodes()
        {
            TranslatedNodes = 0;
            OutdatedNodes = 0;
            UntranslatedNodes = 0;

            foreach (StringNode sNode in Header.StringNodes)
            {
                switch (sNode.State)
                {
                    case NodeState.Translated:
                        TranslatedNodes++;
                        break;
                    case NodeState.Outdated:
                        OutdatedNodes++;
                        break;
                    case NodeState.Untranslated:
                        UntranslatedNodes++;
                        break;
                }
            }
        }

        /// <summary>
        /// Tells the current change tracker grouping to notify the viewmodel that a property changed on undo/redo
        /// </summary>
        /// <param name="propertyName"></param>
        private void TrackNotifyProperty(string propertyName)
        {
            ChangeTracker.Global.GroupNotifyPropertyChanged(OnPropertyChanged, propertyName);
        }


        /// <summary>
        /// Increases the node counter for a specific node state
        /// </summary>
        /// <param name="state">The state of which to increase the counter</param>
        public void IncreaseNodeCounter(NodeState state)
        {
            ChangedValue<int> change;

            switch (state)
            {
                case NodeState.Translated:
                    change = new((v) => TranslatedNodes = v,
                        TranslatedNodes,
                        TranslatedNodes + 1);
                    break;
                case NodeState.Outdated:
                    change = new((v) => OutdatedNodes = v,
                        OutdatedNodes,
                        OutdatedNodes + 1);
                    break;
                case NodeState.Untranslated:
                    change = new((v) => UntranslatedNodes = v,
                        UntranslatedNodes,
                        UntranslatedNodes + 1);
                    break;
                default:
                    return;
            }

            ChangeTracker.Global.TrackChange(change);
        }

        /// <summary>
        /// Decreases the node counter for a specific node state
        /// </summary>
        /// <param name="state">The state of which to decrease the counter</param>
        public void DecreaseNodeCounter(NodeState state)
        {
            ChangedValue<int> change;

            switch (state)
            {
                case NodeState.Translated:
                    change = new((v) => TranslatedNodes = v,
                        TranslatedNodes,
                        TranslatedNodes - 1);
                    break;
                case NodeState.Outdated:
                    change = new((v) => OutdatedNodes = v,
                        OutdatedNodes,
                        OutdatedNodes - 1);
                    break;
                case NodeState.Untranslated:
                    change = new((v) => UntranslatedNodes = v,
                        UntranslatedNodes,
                        UntranslatedNodes - 1);
                    break;
                default:
                    return;
            }

            ChangeTracker.Global.TrackChange(change);
        }

        /// <summary>
        /// Notifies all node viewmodels that the value needs to be updated
        /// </summary>
        public void RefreshNodeValues()
        {
            ChangeTracker.Global.BeginGroup();
            foreach (VmNode node in _nodes)
            {
                node.RefreshNodeValues();
            }
            ChangeTracker.Global.EndGroup();
        }

        /// <summary>
        /// Expands all nodes
        /// </summary>
        public void ExpandAll()
        {
            foreach (VmNode node in Nodes)
            {
                if (node is VmParentNode parent)
                {
                    parent.ExpandAll();
                }
            }
        }

        /// <summary>
        /// Collapses all nodes
        /// </summary>
        public void CollapseAll()
        {
            foreach (VmNode node in Nodes)
            {
                if (node is VmParentNode parent)
                {
                    parent.CollapseAll();
                }
            }
        }

    }
}
