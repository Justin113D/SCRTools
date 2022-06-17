using SCR.Tools.TranslationEditor.Data.Events;
using SCR.Tools.UndoRedo;
using SCR.Tools.UndoRedo.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SCR.Tools.TranslationEditor.Data
{
    /// <summary>
    /// A node which holds more nodes as children (hierarchy node)
    /// </summary>
    public class ParentNode : Node, IEnumerable<Node>
    {
        protected readonly TrackList<Node> _childNodes;

        /// <summary>
        /// The children of this node
        /// </summary>
        public ReadOnlyCollection<Node> ChildNodes { get; }

        public event NodeChildrenChangedEventHandler? ChildrenChanged;

        /// <summary>
        /// Creates a parent node
        /// </summary>
        /// <param name="name">The name of the node</param>
        /// <param name="description">The description of the node</param>
        public ParentNode(string name, string? description = null)
            : base(name, description, NodeState.None)
        {
            _childNodes = new();
            ChildNodes = new(_childNodes);
        }

        public void AddChildNode(Node node, bool updateVersionIndex = true)
            => InternalAddNode(node, _childNodes.Count, updateVersionIndex);
            
        public void RemoveChildNode(Node node)
        {
            if (node.Parent != this)
            {
                ChangeTracker.Global.BlankChange();
                return;
            }

            ChangeTracker.Global.BeginGroup();

            int index = _childNodes.IndexOf(node);
            InternalRemoveNode(index);

            node.InternalSetParent(null, false, index, -1);

            ChangeTracker.Global.EndGroup();
        }

        public void InsertChildNodeAt(Node node, int index)
            => InternalAddNode(node, index);
        
        public void MoveChildNode(int fromIndex, int toIndex)
        {
            if (fromIndex < 0 || fromIndex > _childNodes.Count
                || toIndex < 0 || toIndex > _childNodes.Count)
            {
                throw new IndexOutOfRangeException("One or both indices are out of range!");
            }

            if (fromIndex == toIndex)
            {
                ChangeTracker.Global.BlankChange();
                return;
            }

            if(fromIndex < toIndex)
            {
                toIndex--;
            }

            Node target = _childNodes[fromIndex];

            ChangeTracker.Global.BeginGroup();

            _childNodes.RemoveAt(fromIndex);
            _childNodes.Insert(toIndex, target);

            InvokeChildrenChanged(fromIndex, toIndex);

            ChangeTracker.Global.EndGroup();
        }


        private void InternalAddNode(Node node, int index, bool updateVersionIndex = true)
        {
            if (node.Parent == this)
            {
                ChangeTracker.Global.BlankChange();
                return;
            }

            if (index < 0 || index > _childNodes.Count)
            {
                throw new IndexOutOfRangeException("Index out of range!");
            }

            ChangeTracker.Global.BeginGroup();

            int otherIndex = -1;
            if (node.Parent != null)
            {
                ParentNode otherParent = node.Parent;
                otherIndex = otherParent._childNodes.IndexOf(node);
                otherParent.InternalRemoveNode(otherIndex);
            }

            _childNodes.Insert(index, node);

            if (node.State > State)
            {
                State = node.State;
            }

            node.InternalSetParent(this, updateVersionIndex, otherIndex, index);

            ChangeTracker.Global.EndGroup();
        }

        private void InternalRemoveNode(int index)
        {
            _childNodes.RemoveAt(index);
            EvaluateState();
        }


        internal void EvaluateState(Node changedNode)
        {
            if (changedNode.State > State)
            {
                State = changedNode.State;
            }
            else if (changedNode.State < State)
            {
                EvaluateState();
            }
        }

        /// <summary>
        /// Evaluates the state of the parent node
        /// </summary>
        internal void EvaluateState()
        {
            State = _childNodes.Count > 0
                ? ChildNodes.Max(x => x.State)
                : NodeState.None;
        }


        internal override void InvokeHeaderChanged(NodeHeaderChangedEventArgs args)
        {
            base.InvokeHeaderChanged(args);

            foreach (Node n in _childNodes)
                n.InvokeHeaderChanged(args);
        }

        internal void InvokeChildrenChanged(int fromIndex, int toIndex)
        {
            ChildrenChanged?.Invoke(this, new(fromIndex, toIndex));
        }

        public IEnumerator<Node> GetEnumerator()
            => new NodeHierarchyEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
