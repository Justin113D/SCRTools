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

        public RelayCommand CmdRemove
            => new(Remove);

        protected VmNode(VmFormat format, Node node)
        {
            Node = node;
            _format = format;
        }

        protected void TrackNotifyProperty(string propertyName)
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

        public override string ToString()
            => Node.Name;
    }
}
