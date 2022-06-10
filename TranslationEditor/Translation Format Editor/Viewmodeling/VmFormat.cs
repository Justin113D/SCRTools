using SCR.Tools.TranslationEditor.Data;
using SCR.Tools.TranslationEditor.Data.Events;
using SCR.Tools.TranslationEditor.FormatEditor.CopyPasteData;
using SCR.Tools.UndoRedo;
using SCR.Tools.UndoRedo.ListChange;
using SCR.Tools.Viewmodeling;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCR.Tools.TranslationEditor.FormatEditor.Viewmodeling
{
    public class VmFormat : BaseViewModel
    {
        #region Private fields

        private readonly VmMain _main;

        /// <summary>
        /// Headernode holding the format information
        /// </summary>
        private readonly HeaderNode _header;

        private VmNode? _activeNode;

        private ICPNode[] _copyPasteNodes = Array.Empty<ICPNode>();


        #endregion

        #region Properties

        /// <summary>
        /// Change tracker for the viewmodels
        /// </summary>
        public ChangeTracker FormatTracker { get; }

        /// <summary>
        /// Default Language that the project targets
        /// </summary>
        public string Language
        {
            get => _header.Language;
            set
            {
                if (_header.Language == value)
                    return;

                FormatTracker.BeginGroup();

                _header.Language = value;
                TrackNotifyProperty(nameof(Language));

                FormatTracker.EndGroup();
            }
        }

        /// <summary>
        /// Target name specified in the header
        /// </summary>
        public string TargetName
        {
            get => _header.Name;
            set
            {
                if (_header.Name == value)
                    return;

                FormatTracker.BeginGroup();

                _header.Name = value;
                TrackNotifyProperty(nameof(TargetName));

                FormatTracker.EndGroup();
            }
        }

        /// <summary>
        /// Current version in the header
        /// </summary>
        public string Version
        {
            get => _header.Version.ToString();
            set
            {
                Version newVersion;
                try
                {
                    newVersion = new(value);
                }
                catch
                {
                    return;
                }

                if (newVersion <= _header.Version)
                    return;

                FormatTracker.BeginGroup();

                _header.Version = newVersion;
                TrackNotifyProperty(nameof(Version));

                FormatTracker.EndGroup();
            }
        }


        /// <summary>
        /// Private collection for the node viewmodels
        /// </summary>
        private readonly ObservableCollection<VmNode> _nodes;

        /// <summary>
        /// Top level node viewmodels
        /// </summary>
        public ReadOnlyObservableCollection<VmNode> Nodes { get; }

        private readonly Dictionary<Node, VmNode> _nodeTable;


        public HashSet<Node> SelectedNodes { get; private set; }

        public VmNode? ActiveNode
        {
            get => _activeNode;
            set
            {
                if(_activeNode == value)
                {
                    return;
                }

                VmNode? oldValue = _activeNode;

                _activeNode = value;

                oldValue?.NotifyActiveChanged();
                _activeNode?.NotifyActiveChanged();
            }
        }

        #endregion

        #region Commands

        public RelayCommand CmdAddNewStringNode
            => new(AddNewStringNode);

        public RelayCommand CmdAddNewParentNode
            => new(AddNewParentNode);

        public RelayCommand CmdDeselectAll
            => new(DeselectAll);

        public RelayCommand CmdRemoveSelected
            => new(RemoveSelected);

        public RelayCommand CmdCopySelected
            => new(CopySelected);

        public RelayCommand CmdPasteSelected
            => new(PasteSelected);

        public RelayCommand CmdCollapseAll
            => new(CollapseAll);

        #endregion

        public VmFormat(VmMain main, HeaderNode data, ChangeTracker projectTracker)
        {
            _main = main;
            _header = data;
            FormatTracker = projectTracker;

            _nodeTable = new();
            _nodes = new();
            Nodes = new(_nodes);
            CreateNodes();

            SelectedNodes = new();

            _header.ChildrenChanged += ChildrenChanged;
        }


        public VmNode GetNodeViewmodel(Node node)
        {
            if (!_nodeTable.TryGetValue(node, out VmNode? vmNode))
            {
                if (node is ParentNode p)
                {
                    vmNode = new VmParentNode(this, p);
                }
                else if (node is StringNode s)
                {
                    vmNode = new VmStringNode(this, s);
                }
                else
                {
                    throw new NotSupportedException(node.GetType().Name + " is not a valid node type");
                }
                node.HeaderChanged += HeaderChanged;
                _nodeTable.Add(node, vmNode);
            }
            return vmNode;
        }

        /// <summary>
        /// Create the top level node viewmodels
        /// </summary>
        private void CreateNodes()
        {
            foreach (Node node in _header.ChildNodes)
            {
                VmNode vmNode = GetNodeViewmodel(node);
                _nodes.Add(vmNode);
            }
        }

        private void ChildrenChanged(ParentNode node, NodeChildrenChangedEventArgs args)
        {
            FormatTracker.BeginGroup();

            if (args.FromIndex > -1)
            {
                FormatTracker.TrackChange(
                    new ChangeListRemoveAt<VmNode>(
                        _nodes, args.FromIndex));
            }

            if (args.ToIndex > -1)
            {
                VmNode vmNode = GetNodeViewmodel(_header.ChildNodes[args.ToIndex]);
                FormatTracker.TrackChange(
                    new ChangeListInsert<VmNode>(
                        _nodes, vmNode, args.ToIndex));

                FormatTracker.TrackChange(new Change(
                    () => { },
                    () =>
                    {
                        vmNode.Selected = false;
                        vmNode.Active = false;
                    }));
            }

            FormatTracker.EndGroup();
        }

        private void HeaderChanged(Node node, NodeHeaderChangedEventArgs args)
        {
            if (args.NewHeader == _header)
                return;
            
            VmNode vmNode = _nodeTable[node];

            FormatTracker.TrackChange(new Change(
                () =>
                {
                    vmNode.Selected = false;
                    vmNode.Active = false;
                    _nodeTable.Remove(node);
                    node.HeaderChanged -= HeaderChanged;
                },
                () =>
                { 
                    _nodeTable.Add(node, vmNode);
                    node.HeaderChanged += HeaderChanged;
                }
            ));
        }
       
        /// <summary>
        /// Tells the current change tracker grouping to notify the viewmodel that a property changed on undo/redo
        /// </summary>
        /// <param name="propertyName"></param>
        private void TrackNotifyProperty(string propertyName)
        {
            FormatTracker.GroupNotifyPropertyChanged(OnPropertyChanged, propertyName);
        }

        #region Action methods

        private void AddNewStringNode()
        {
            _header.AddChildNode(new StringNode("String", ""));
        }

        private void AddNewParentNode()
        {
            _header.AddChildNode(new ParentNode("Category"));
        }

        private HashSet<Node> GetTopSelected()
        {
            HashSet<Node> topSelected = new();

            foreach (Node node in SelectedNodes)
            {
                Node finalSelected = node;

                Node current = node;
                while (current.Parent is ParentNode pn and not HeaderNode)
                {
                    current = pn;
                    if (SelectedNodes.Contains(current))
                    {
                        finalSelected = current;
                    }
                }

                topSelected.Add(finalSelected);
            }

            return topSelected;
        }

        private Node[] SortNodesByHierarchy(HashSet<Node> nodes)
        {
            Node[] output = new Node[nodes.Count];
            int insertIndex = 0;
            foreach(Node node in _header)
            {
                if(nodes.Contains(node))
                {
                    output[insertIndex] = node;
                    insertIndex++;
                    if (insertIndex == output.Length)
                    {
                        break;
                    }
                }
            }

            return output;
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

        public void RemoveSelected()
        {
            if (SelectedNodes.Count == 0)
            {
                return;
            }

            HashSet<Node> toDelete = GetTopSelected();

            FormatTracker.BeginGroup();

            foreach(Node node in toDelete)
            {
                _nodeTable[node].Remove();
            }

            FormatTracker.EndGroup();
        }

        public void DeselectAll()
        {
            foreach (Node node in SelectedNodes.ToArray())
            {
                _nodeTable[node].Selected = false;
            }
        }

        public void SelectRange(VmNode target, bool multi)
        {
            FormatTracker.BeginGroup();

            if (!multi)
            {
                DeselectAll();
            }

            Stack<(IList<VmNode> children, int index)> treeStack = new();
            treeStack.Push((Nodes, 0));

            bool found = false;

            while (treeStack.Count > 0)
            {
                for ((IList<VmNode> children, int index) = treeStack.Pop();
                    index < children.Count;
                    index++)
                {
                    VmNode node = children[index];
                    if (node == target || node == ActiveNode)
                    {
                        if (!found)
                        {
                            found = true;
                        }
                        else
                        {
                            node.Selected = true;
                            treeStack.Clear();
                            break;
                        }
                    }

                    if (found)
                    {
                        node.Selected = true;
                    }

                    if (node is VmParentNode pn && pn.Expanded)
                    {
                        treeStack.Push((children, index + 1));
                        treeStack.Push((pn.ChildNodes, 0));
                        break;
                    }
                }
            }

            FormatTracker.EndGroup();
        }

        public void MoveSelected(ParentNode parent, int index)
        {
            if (SelectedNodes.Count == 0)
            {
                return;
            }

            Node[] nodes = SortNodesByHierarchy(SelectedNodes);
            
            FormatTracker.BeginGroup();

            foreach (Node node in nodes)
            {
                VmNode vmNode = _nodeTable[node];
                if(parent.ChildNodes.Contains(vmNode.Node))
                {
                    int moveIndex = parent.ChildNodes.IndexOf(vmNode.Node);
                    parent.MoveChildNode(moveIndex, index);
                    index = parent.ChildNodes.IndexOf(vmNode.Node);
                }
                else
                {
                    parent.InsertChildNodeAt(vmNode.Node, index);
                }
                index++;
            }

            FormatTracker.EndGroup();
        }


        private void CopySelected()
        {
            if (SelectedNodes.Count == 0)
            {
                return;
            }

            Node[] nodes = SortNodesByHierarchy(GetTopSelected());
            _copyPasteNodes = new ICPNode[nodes.Length];
            int copyCount = 0;

            for(int i = 0; i < nodes.Length; i++)
            {
                Node node = nodes[i];
                if(node is ParentNode pn)
                {
                    CPParentNode cpPn =  CPParentNode.FromNode(pn);
                    _copyPasteNodes[i] = cpPn;
                    copyCount += cpPn.NodeCount;
                }
                else if(node is StringNode sn)
                {
                    _copyPasteNodes[i] = CPStringNode.FromNode(sn);
                    copyCount++;
                }
            }

            _main.SetMessage($"Copied a total of {copyCount} nodes!", false);
        }

        private void PasteSelected()
        {
            if(_copyPasteNodes.Length == 0)
            {
                return;
            }

            ParentNode parent;
            if(ActiveNode != null && ActiveNode.Selected)
            {
                if (ActiveNode.Node is ParentNode pn)
                {
                    parent = pn;
                }
                else if (ActiveNode.Node is StringNode sn)
                {
                    if (sn.Parent == null)
                    {
                        // can't happen but i want the warnings to shut up
                        throw new InvalidOperationException();
                    }
                    parent = sn.Parent;
                }
                else
                {
                    // can't happen, but necessary
                    throw new InvalidOperationException();
                }
            }
            else
            {
                parent = _header;
            }

            int pasteCount = 0;

            FormatTracker.BeginGroup();

            foreach (ICPNode t in _copyPasteNodes)
            {
                t.CreateNode(parent);
                pasteCount += t.NodeCount;
            }

            FormatTracker.EndGroup();

            _main.SetMessage($"Pasted a total of {pasteCount} nodes!", false);
        }

        #endregion

        public string WriteFormat()
        {
            return _header.WriteFormat();
        }

        /// <summary>
        /// Exports language to a key and values file
        /// </summary>
        /// <param name="filepath"></param>
        public void ExportLanguage(string filepath)
        {
            (string keys, string values) = _header.ExportLanguageData();

            File.WriteAllText(filepath, values);

            string keyFilePath = Path.ChangeExtension(filepath, "langkey");
            File.WriteAllText(keyFilePath, keys);
        }
    }
}
