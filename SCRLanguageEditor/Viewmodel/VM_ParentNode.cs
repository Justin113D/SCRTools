using SCRCommon.Viewmodels;
using SCRLanguageEditor.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace SCRLanguageEditor.Viewmodel
{
    /// <summary>
    /// Viewmodel for a parent node
    /// </summary>
    public class VM_ParentNode : VM_Node
    {
        /// <summary>
        /// The parent node which the viewmodel accesses and modifies
        /// </summary>
        private ParentNode ParentNode => (ParentNode)Node;

        private bool _isExpanded;

        /// <summary>
        /// The node children of the parent node
        /// </summary>
        public ObservableCollection<VM_Node> Children { get; private set; }

        /// <summary>
        /// Command for adding a new string node
        /// </summary>
        public RelayCommand Cmd_AddStringNode { get; }

        /// <summary>
        /// Command for adding a new parent node
        /// </summary>
        public RelayCommand Cmd_AddParentNode { get; }

        /// <summary>
        /// Expands and collapses the children properly
        /// </summary>
        public override bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if(ParentNode.ChildNodes.Count == 0)
                    return;

                _isExpanded = value;
                if(value && Children[0] == null)
                    Expand();
            }
        }

        /// <summary>
        /// Base constructor
        /// </summary>
        /// <param name="node">The parent node</param>
        public VM_ParentNode(VM_ParentNode parent, ParentNode node, VM_HeaderNode vm) : base(node, vm, parent)
        {
            Cmd_AddStringNode = new RelayCommand(() => AddStringNode());
            Cmd_AddParentNode = new RelayCommand(() => AddParentNode());
            Children = new ObservableCollection<VM_Node>();
            if(ParentNode.ChildNodes.Count > 0)
                Children.Add(null);
        }

        /// <summary>
        /// Creates new node viewmodels from the node children to display
        /// </summary>
        private void Expand()
        {
            List<VM_Node> children = new List<VM_Node>();
            foreach(Node n in ParentNode.ChildNodes)
            {
                switch(n.Type)
                {
                    case Node.NodeType.ParentNode:
                        children.Add(new VM_ParentNode(this, (ParentNode)n, VMHeader));
                        break;
                    case Node.NodeType.StringNode:
                        children.Add(new VM_StringNode(this, (StringNode)n, VMHeader));
                        break;
                }
            }
            Children = new ObservableCollection<VM_Node>(children);
        }

        protected override void ExpandParents()
        {
            IsExpanded = true;
            base.ExpandParents();
        }

        /// <summary>
        /// Updates the viewmodels properties and those of the loaded children
        /// </summary>
        public override void UpdateProperties()
        {
            OnPropertyChanged(nameof(NodeState));
            foreach(VM_Node n in Children)
            {
                n?.UpdateProperties();
            }
        }

        public void UpdateNodeState()
        {
            OnPropertyChanged(nameof(NodeState));
            Parent?.UpdateNodeState();
        }

        /// <summary>
        /// Adds a new string node
        /// </summary>
        private void AddStringNode()
        {
            VMHeader.Tracker.BeginGroup();
            StringNode node = VMHeader.NewKey();
            VM_StringNode vmNode = new VM_StringNode(this, node, VMHeader);

            VMHeader.Tracker.TrackChange(new ChangedListSingleEntry<Node>(ParentNode.ChildNodes, node, ParentNode.ChildNodes.Count, null));
            VMHeader.Tracker.TrackChange(new ChangedListSingleEntry<VM_Node>(Children, vmNode, Children.Count, () => OnPropertyChanged(nameof(Children))));
            VMHeader.Tracker.EndGroup();

            ParentNode.ChildNodes.Add(node);
            Children.Add(new VM_StringNode(this, node, VMHeader));
            IsExpanded = true;
        }

        /// <summary>
        /// Adds a new parent node
        /// </summary>
        private void AddParentNode()
        {
            ParentNode node = new ParentNode("New Parent");
            VM_ParentNode vmNode = new VM_ParentNode(this, node, VMHeader);

            VMHeader.Tracker.BeginGroup();
            VMHeader.Tracker.TrackChange(new ChangedListSingleEntry<Node>(ParentNode.ChildNodes, node, ParentNode.ChildNodes.Count, null));
            VMHeader.Tracker.TrackChange(new ChangedListSingleEntry<VM_Node>(Children, vmNode, Children.Count, () => OnPropertyChanged(nameof(Children))));
            VMHeader.Tracker.EndGroup();

            ParentNode.ChildNodes.Add(node);
            Children.Add(vmNode);
            IsExpanded = true;
        }

        public void InsertChild(VM_Node vmNode, int index)
        {
            VMHeader.Tracker.BeginGroup();
            VMHeader.Tracker.TrackChange(new ChangedListSingleEntry<Node>(ParentNode.ChildNodes, vmNode.Node, index, null));
            VMHeader.Tracker.TrackChange(new ChangedListSingleEntry<VM_Node>(Children, vmNode, index, () => OnPropertyChanged(nameof(Children))));
            VMHeader.Tracker.EndGroup();

            ParentNode.ChildNodes.Insert(index, vmNode.Node);
            Children.Insert(index, vmNode);
        }

        /// <summary>
        /// Removes a child from the nodes children
        /// </summary>
        /// <param name="vmNode"></param>
        public void RemoveChild(VM_Node vmNode)
        {
            VMHeader.Tracker.BeginGroup();
            VMHeader.Tracker.TrackChange(new ChangedListSingleEntry<Node>(ParentNode.ChildNodes, vmNode.Node, null, null));
            VMHeader.Tracker.TrackChange(new ChangedListSingleEntry<VM_Node>(Children, vmNode, null, () => OnPropertyChanged(nameof(Children))));
            VMHeader.Tracker.EndGroup();

            ParentNode.ChildNodes.Remove(vmNode.Node);
            Children.Remove(vmNode);

            if(ParentNode.ChildNodes.Count == 0)
                _isExpanded = false;
        }

        public override void Remove()
        {
            // get all string nodes
            List<StringNode> stringNodes = new List<StringNode>();
            Queue<ParentNode> nodes = new Queue<ParentNode>();
            nodes.Enqueue(ParentNode);

            while(nodes.Count > 0)
            {
                ParentNode current = nodes.Dequeue();
                foreach(var n in current.ChildNodes)
                {
                    if(n.Type == Node.NodeType.ParentNode)
                        nodes.Enqueue((ParentNode)n);
                    else
                        stringNodes.Add((StringNode)n);
                }
            }

            if(stringNodes.Count > 0)
            {
                // warning dialog
                MessageBoxResult r = MessageBox.Show($"Deleting this parent will remove {stringNodes.Count} Nodes, proceed?", "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                switch(r)
                {
                    case MessageBoxResult.No:
                    case MessageBoxResult.None:
                        return;
                }
            }

            VMHeader.Tracker.BeginGroup();

            // now remove all string nodes
            foreach(var n in stringNodes)
            {
                VMHeader.RemoveKey(n.Name);
            }

            base.Remove();
            VMHeader.Tracker.EndGroup();
        }

        protected override void InsertNode(VM_Node insertNode)
        {
            if(!IsExpanded)
            {
                base.InsertNode(insertNode);
                return;
            }

            if(insertNode.Parent == null)
                VMHeader.RemoveChild(insertNode);
            else
                insertNode.Parent.RemoveChild(insertNode);

            Children.Insert(0, insertNode);
            ParentNode.ChildNodes.Insert(0, insertNode.Node);
        }
    }
}
