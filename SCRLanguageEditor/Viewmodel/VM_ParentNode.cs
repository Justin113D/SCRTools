using SCRCommon.Viewmodels;
using SCRLanguageEditor.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
                _isExpanded = value;
                if(Children[0] == null)
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
                switch (n.Type)
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
            StringNode node = VMHeader.AddStringNode(ParentNode);
            Children.Add(new VM_StringNode(this, node, VMHeader));
        }

        /// <summary>
        /// Adds a new parent node
        /// </summary>
        private void AddParentNode()
        {
            ParentNode node = new ParentNode("New Parent");
            ParentNode.ChildNodes.Add(node);
            Children.Add(new VM_ParentNode(this, node, VMHeader));
        }

        /// <summary>
        /// Removes a child from the nodes children
        /// </summary>
        /// <param name="node"></param>
        public void RemoveChild(VM_Node node)
        {
            ParentNode.ChildNodes.Remove(node.Node);
            Children.Remove(node);
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

            base.Remove();
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
