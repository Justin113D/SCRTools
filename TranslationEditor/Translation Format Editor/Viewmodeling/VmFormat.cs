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

namespace SCR.Tools.TranslationEditor.FormatEditor.Viewmodeling
{
    public class VmFormat : BaseViewModel
    {
        #region Private fields

        private readonly VmMain _main;

        private VmNode? _activeNode;

        private ICPNode[] _copyPasteNodes 
            = Array.Empty<ICPNode>();

        #endregion

        #region Properties

        /// <summary>
        /// Headernode holding the format information
        /// </summary>
        public HeaderNode Header { get; }


        /// <summary>
        /// Default Language that the project targets
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
        {
            get => Header.Name;
            set
            {
                if (Header.Name == value)
                    return;

                ChangeTracker.Global.BeginGroup();

                Header.Name = value;
                TrackNotifyProperty(nameof(TargetName));

                ChangeTracker.Global.EndGroup();
            }
        }

        /// <summary>
        /// Current version in the header
        /// </summary>
        public string Version
        {
            get => Header.Version.ToString();
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

                if (newVersion <= Header.Version)
                    return;

                ChangeTracker.Global.BeginGroup();

                Header.Version = newVersion;
                TrackNotifyProperty(nameof(Version));

                ChangeTracker.Global.EndGroup();
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
                if (_activeNode == value)
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

        public VmFormat(VmMain main, HeaderNode data)
        {
            _main = main;
            Header = data;

            _nodeTable = new();
            _nodes = new();
            Nodes = new(_nodes);
            CreateNodes();

            SelectedNodes = new();

            Header.ChildrenChanged += ChildrenChanged;
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
            foreach (Node node in Header.ChildNodes)
            {
                VmNode vmNode = GetNodeViewmodel(node);
                _nodes.Add(vmNode);
            }
        }

        private void ChildrenChanged(ParentNode node, NodeChildrenChangedEventArgs args)
        {
            ChangeTracker.Global.BeginGroup();

            if (args.FromIndex > -1)
            {
                ChangeTracker.Global.TrackChange(
                    new ChangeListRemoveAt<VmNode>(
                        _nodes, args.FromIndex));
            }

            if (args.ToIndex > -1)
            {
                VmNode vmNode = GetNodeViewmodel(Header.ChildNodes[args.ToIndex]);
                ChangeTracker.Global.TrackChange(
                    new ChangeListInsert<VmNode>(
                        _nodes, vmNode, args.ToIndex));

                ChangeTracker.Global.TrackChange(new Change(
                    () => { },
                    () =>
                    {
                        vmNode.Selected = false;
                        vmNode.Active = false;
                    }));
            }

            ChangeTracker.Global.EndGroup();
        }

        private void HeaderChanged(Node node, NodeHeaderChangedEventArgs args)
        {
            if (args.NewHeader == Header)
                return;

            VmNode vmNode = _nodeTable[node];

            ChangeTracker.Global.TrackChange(new Change(
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
            ChangeTracker.Global.GroupNotifyPropertyChanged(OnPropertyChanged, propertyName);
        }

        #region Action methods

        private void AddNewStringNode()
        {
            Header.AddChildNode(new StringNode("String", ""));
        }

        private void AddNewParentNode()
        {
            Header.AddChildNode(new ParentNode("Category"));
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
            foreach (Node node in Header)
            {
                if (nodes.Contains(node))
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

            ChangeTracker.Global.BeginGroup();

            foreach (Node node in toDelete)
            {
                _nodeTable[node].Remove();
            }

            ChangeTracker.Global.EndGroup();
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
            ChangeTracker.Global.BeginGroup();

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

            ChangeTracker.Global.EndGroup();
        }

        public void MoveSelected(ParentNode parent, int index)
        {
            if (SelectedNodes.Count == 0)
            {
                return;
            }

            Node[] nodes = SortNodesByHierarchy(SelectedNodes);

            ChangeTracker.Global.BeginGroup();

            foreach (Node node in nodes)
            {
                VmNode vmNode = _nodeTable[node];
                if (parent.ChildNodes.Contains(vmNode.Node))
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

            ChangeTracker.Global.EndGroup();
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

            for (int i = 0; i < nodes.Length; i++)
            {
                Node node = nodes[i];
                if (node is ParentNode pn)
                {
                    CPParentNode cpPn = CPParentNode.FromNode(pn);
                    _copyPasteNodes[i] = cpPn;
                    copyCount += cpPn.NodeCount;
                }
                else if (node is StringNode sn)
                {
                    _copyPasteNodes[i] = CPStringNode.FromNode(sn);
                    copyCount++;
                }
            }

            _main.SetMessage($"Copied a total of {copyCount} nodes!", false);
        }

        private void PasteSelected()
        {
            if (_copyPasteNodes.Length == 0)
            {
                return;
            }

            ParentNode parent;
            if (ActiveNode != null && ActiveNode.Selected)
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
                parent = Header;
            }

            int pasteCount = 0;

            ChangeTracker.Global.BeginGroup();

            foreach (ICPNode t in _copyPasteNodes)
            {
                t.CreateNode(parent);
                pasteCount += t.NodeCount;
            }

            ChangeTracker.Global.EndGroup();

            _main.SetMessage($"Pasted a total of {pasteCount} nodes!", false);
        }

        #endregion

    }
}
