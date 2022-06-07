using SCR.Tools.TranslationEditor.Data;
using SCR.Tools.TranslationEditor.Data.Events;
using SCR.Tools.UndoRedo;
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
        protected readonly Node _node;

        protected readonly VmFormat _format;

        public string Name
        {
            get => _node.Name;
            set
            {
                if (_node.Name == value)
                    return;

                _format.FormatTracker.BeginGroup();

                _node.Name = value;
                TrackNotifyProperty(nameof(Name));

                _format.FormatTracker.EndGroup();
            }
        }

        public string? Description
        {
            get => _node.Description;
            set
            {
                if (_node.Description == value)
                    return;

                _format.FormatTracker.BeginGroup();

                _node.Description = value;
                TrackNotifyProperty(nameof(Description));

                _format.FormatTracker.EndGroup();
            }
        }

        public virtual bool CanExpand => false;

        public virtual bool Expanded { get; set; }

        public RelayCommand CmdRemove
            => new(Remove);

        protected VmNode(VmFormat format, Node node)
        {
            _node = node;
            _format = format;
        }

        protected void TrackNotifyProperty(string propertyName)
        {
            _format.FormatTracker.GroupNotifyPropertyChanged(OnPropertyChanged, propertyName);
        }

        private void Remove() 
            =>_node.SetParent(null);
    }
}
