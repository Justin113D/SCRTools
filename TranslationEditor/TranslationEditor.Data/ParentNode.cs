using SCR.Tools.TranslationEditor.Data.Events;
using SCR.Tools.UndoRedo;
using SCR.Tools.UndoRedo.ListChange;
using System.Collections.ObjectModel;

namespace SCR.Tools.TranslationEditor.Data
{
    /// <summary>
    /// A node which holds more nodes as children (hierarchy node)
    /// </summary>
    public class ParentNode : Node
    {
        protected readonly List<Node> _childNodes;

        /// <summary>
        /// The children of this node
        /// </summary>
        public ReadOnlyCollection<Node> ChildNodes { get; }

        public event NodeChildrenMovedEventHandler? ChildrenMoved;

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
        {
            if (node.Parent == this)
            {
                ChangeTracker.Global.BlankChange();
                return;
            }

            ChangeTracker.Global.BeginGroup();

            if (node.Parent != null)
            {
                node.Parent.InternalRemoveNode(node);
            }

            ChangeTracker.Global.TrackChange(new ChangeListAdd<Node>(
                _childNodes, node));

            if (node.State > State)
            {
                State = node.State;
            }

            node.InternalSetParent(this, updateVersionIndex);

            ChangeTracker.Global.EndGroup();
        }

        public void RemoveChildNode(Node node)
        {
            if (node.Parent != this)
            {
                ChangeTracker.Global.BlankChange();
                return;
            }

            ChangeTracker.Global.BeginGroup();

            InternalRemoveNode(node);

            // if the node is the same state (contributed to the current nodes state)
            // then reevaluate the state across all children
            if (node.State == State)
            {
                EvaluateState();
            }

            node.InternalSetParent(null, false);

            ChangeTracker.Global.EndGroup();
        }

        public void InsertChildNodeAt(Node node, int index)
        {
            ChangeTracker.Global.BeginGroup();

            AddChildNode(node);
            MoveChildNode(_childNodes.Count - 1, index);

            ChangeTracker.Global.EndGroup();
        }

        public void MoveChildNode(int fromIndex, int toIndex)
        {
            if (fromIndex < 0 || fromIndex >= _childNodes.Count
                || toIndex < 0 || toIndex >= _childNodes.Count)
            {
                throw new IndexOutOfRangeException("One or both indices are out of range!");
            }

            if (fromIndex == toIndex)
            {
                ChangeTracker.Global.BlankChange();
                return;
            }

            Node target = _childNodes[fromIndex];

            ChangeTracker.Global.BeginGroup();

            ChangeTracker.Global.TrackChange(new Change(
                () =>
                {
                    _childNodes.RemoveAt(fromIndex);
                    _childNodes.Insert(toIndex, target);
                },
                () =>
                {
                    _childNodes.RemoveAt(toIndex);
                    _childNodes.Insert(fromIndex, target);
                }
            ));

            ChildrenMoved?.Invoke(this, new(fromIndex, toIndex));

            ChangeTracker.Global.EndGroup();
        }

        internal void InternalRemoveNode(Node node)
        {
            ChangeTracker.Global.TrackChange(new ChangeListRemove<Node>(
                _childNodes, node));

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
                ? ChildNodes.Min(x => x.State)
                : NodeState.None;
        }

        internal override void InvokeHeaderChanged(NodeHeaderChangedEventArgs args)
        {
            base.InvokeHeaderChanged(args);

            foreach (Node n in _childNodes)
                n.InvokeHeaderChanged(args);
        }

    }
}
