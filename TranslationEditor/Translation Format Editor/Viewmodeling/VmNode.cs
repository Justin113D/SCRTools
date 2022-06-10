using SCR.Tools.TranslationEditor.Data;
using SCR.Tools.UndoRedo;
using SCR.Tools.UndoRedo.ListChange;
using SCR.Tools.Viewmodeling;

namespace SCR.Tools.TranslationEditor.FormatEditor.Viewmodeling
{
    public abstract class VmNode : BaseViewModel
    {

        protected readonly VmFormat _format;

        private bool _selected;

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

                _format.FormatTracker.BeginGroup();

                Node.Name = value;
                TrackNotifyProperty(nameof(Name));

                _format.FormatTracker.EndGroup();
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

                _format.FormatTracker.BeginGroup();

                Node.Description = value;
                TrackNotifyProperty(nameof(Description));

                _format.FormatTracker.EndGroup();
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
            get => _selected;
            set
            {
                if (_selected == value)
                    return;

                _format.FormatTracker.BeginGroup();

                ChangeTracker.Global.TrackChange(
                    new ChangedValue<bool>(
                        (v) => _selected = v,
                        _selected,
                        value
                ));

                if (value)
                {
                    _format.FormatTracker.TrackChange(
                        new ChangeCollectionAdd<VmNode>(
                            _format.SelectedNodes, this));
                }
                else
                {
                    _format.FormatTracker.TrackChange(
                        new ChangeCollectionRemove<VmNode>(
                            _format.SelectedNodes, this));
                }

                TrackNotifyProperty(nameof(Selected));

                _format.FormatTracker.EndGroup();
            }
        }

        /// <summary>
        /// Whether the node is the active selected one
        /// </summary>
        public bool Active => _format.ActiveNode == this;

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

        public void TrackNotifyProperty(string propertyName)
        {
            _format.FormatTracker.GroupNotifyPropertyChanged(OnPropertyChanged, propertyName);
        }

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
            _format.FormatTracker.BeginGroup();

            if (!multi)
            {
                _format.DeselectAll();
                Selected = true;
            }
            else
            {
                Selected = !Selected;
            }

            _format.ActiveNode = this;
            _format.FormatTracker.EndGroup();
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
            if(Node.Parent is not ParentNode pn)
            {
                return true;
            }

            // check if pn is inside the hierarchy of any selected node, to prevent recursion
            foreach(VmNode node in _format.SelectedNodes)
            {
                if(node is not VmParentNode vmPn)
                {
                    continue;
                }

                ParentNode check = vmPn.ParentNode;

                ParentNode current = pn;
                while(current.Parent is ParentNode next)
                {
                    if(current == check)
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
            if(Node.Parent == null)
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

        #endregion

    }
}
