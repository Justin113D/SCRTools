using SCR.Tools.TranslationEditor.Data;
using SCR.Tools.TranslationEditor.Data.Events;
using SCR.Tools.UndoRedo;
using SCR.Tools.UndoRedo.ListChange;
using SCR.Tools.Viewmodeling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCR.Tools.TranslationEditor.FormatEditor.Viewmodeling
{
    public abstract class VmNode : BaseViewModel
    {
        public Node Node { get; }

        protected readonly VmFormat _format;

        private bool _selected;


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

        public virtual bool CanExpand => false;

        public virtual bool Expanded { get; set; }

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

        public bool Active => _format.ActiveNode == this;
        

        public RelayCommand CmdRemove
            => new(Remove);

        protected VmNode(VmFormat format, Node node)
        {
            Node = node;
            _format = format;
        }

        public void TrackNotifyProperty(string propertyName)
        {
            _format.FormatTracker.GroupNotifyPropertyChanged(OnPropertyChanged, propertyName);
        }

        public void Remove()
            => Node.SetParent(null);

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

        public void SelectRegion(bool multi)
        {
            if (_format.ActiveNode == null)
            {
                Select(false);
                return;
            }

            _format.SelectRange(this, multi);
        }

        public void DeselectAll()
            => _format.DeselectAll();

        public bool CanInsert()
        {
            if(Node.Parent == null)
            {
                throw new InvalidOperationException("Node has no parent");
            }

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

        public void InsertAbove()
        {
            if(Node.Parent == null)
            {
                return;
            }

            int insertIndex = Node.Parent.ChildNodes.IndexOf(Node);
            _format.MoveSelected(Node.Parent, insertIndex);
        }

        public virtual void InsertBelow()
        {
            if (Node.Parent == null)
            {
                return;
            }

            int insertIndex = Node.Parent.ChildNodes.IndexOf(Node) + 1;
            _format.MoveSelected(Node.Parent, insertIndex);
        }

        public override string ToString()
            => Node.Name;
    }
}
