using SCR.Tools.TranslationEditor.Data;
using SCR.Tools.UndoRedo;
using SCR.Tools.WPF.Viewmodeling;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;

namespace SCR.Tools.TranslationEditor.FormatEditor.Viewmodeling
{
    /// <summary>
    /// Viewmodel for a node
    /// </summary>
    public abstract class VmNode : BaseViewModel
    {
        /// <summary>
        /// Assigned format
        /// </summary>
        protected readonly VmFormat _format;

        #region Properties

        /// <summary>
        /// Data source for this viewmodel
        /// </summary>
        public Node Node { get; }

        /// <summary>
        /// Name of the node (see <see cref="Node.Name"/>
        /// </summary>
        public string Name
        {
            get => Node.Name;
            set
            {
                if (Node.Name == value)
                    return;

                BeginChangeGroup();

                Node.Name = value;
                TrackNotifyProperty(nameof(Name));

                EndChangeGroup();
            }
        }

        /// <summary>
        /// Description of the node (see <see cref="Node.Description"/>
        /// </summary>
        public string? Description
        {
            get => Node.Description;
            set
            {
                if (Node.Description == value)
                    return;

                BeginChangeGroup();

                Node.Description = value;
                TrackNotifyProperty(nameof(Description));

                EndChangeGroup();
            }
        }


        /// <summary>
        /// Whether this node can be expanded
        /// </summary>
        public virtual bool CanExpand => false;

        /// <summary>
        /// Whether the children of this node are visible
        /// </summary>
        public virtual bool Expanded { get; set; }

        /// <summary>
        /// Whether the node is selected
        /// </summary>
        public bool Selected
        {
            get => _format.SelectedNodes.Contains(Node);
            set
            {
                if (Selected == value)
                    return;

                if (value)
                {
                    _format.SelectedNodes.Add(Node);
                }
                else
                {
                    _format.SelectedNodes.Remove(Node);
                }
            }
        }

        /// <summary>
        /// Whether the node is the active selected one
        /// </summary>
        public bool Active
        {
            get => _format.ActiveNode == this;
            set
            {
                if (Active == value)
                {
                    return;
                }

                if (value)
                {
                    var previous = _format.ActiveNode;
                    _format.ActiveNode = this;
                    previous?.OnPropertyChanged(nameof(Active));
                }
                else
                {
                    _format.ActiveNode = null;
                }
            }
        }

        #endregion

        #region Commands

        public RelayCommand CmdRemove
            => new(Remove);

        #endregion

        protected VmNode(VmFormat format, Node node)
        {
            Node = node;
            _format = format;
        }

        public void NotifyActiveChanged()
            => OnPropertyChanged(nameof(Active));

        public override string ToString()
            => Node.Name;

        #region Action methods

        /// <summary>
        /// Removes this node from the format
        /// </summary>
        public void Remove()
            => Node.SetParent(null);

        /// <summary>
        /// Selects this node
        /// </summary>
        /// <param name="multi">Whether to keep previous selected nodes</param>
        public void Select(bool multi)
        {
            if (!multi)
            {
                _format.DeselectAll();
                Selected = true;
            }
            else
            {
                Selected = !Selected;
            }

            Active = true;
        }

        /// <summary>
        /// Selects all nodes from the active node to this node
        /// </summary>
        /// <param name="multi">Whether to keep previous selected nodes</param>
        public void SelectRegion(bool multi)
        {
            if (_format.ActiveNode == null)
            {
                Select(false);
                return;
            }

            _format.SelectRange(this, multi);
        }

        /// <summary>
        /// Reroute for deselecting all nodes of the format
        /// </summary>
        public void DeselectAll()
            => _format.DeselectAll();

        /// <summary>
        /// Checks whether the selected nodes can be inserted above or below this node
        /// </summary>
        /// <returns></returns>
        public bool CanInsert()
        {
            // this is only true if the parent is the header, which can always be inserted in
            if (Node.Parent is not ParentNode pn)
            {
                return true;
            }

            // check if pn is inside the hierarchy of any selected node, to prevent recursion
            foreach (Node node in _format.SelectedNodes)
            {
                if (node is not ParentNode check)
                {
                    continue;
                }

                ParentNode current = pn;
                while (current.Parent is ParentNode next)
                {
                    if (current == check)
                    {
                        return false;
                    }
                    current = next;
                }
            }

            return true;
        }

        /// <summary>
        /// Move selected nodes above this node
        /// </summary>
        public void InsertAbove()
        {
            if (Node.Parent == null)
            {
                return;
            }

            int insertIndex = Node.Parent.ChildNodes.IndexOf(Node);
            _format.MoveSelected(Node.Parent, insertIndex);
        }

        /// <summary>
        /// Move selected nodes below this node 
        /// </summary>
        public virtual void InsertBelow()
        {
            if (Node.Parent == null)
            {
                return;
            }

            int insertIndex = Node.Parent.ChildNodes.IndexOf(Node) + 1;
            _format.MoveSelected(Node.Parent, insertIndex);
        }

        public virtual void DeselectHierarchy()
        {
            Selected = false;
            Active = false;
        }

        #endregion

    }
}
