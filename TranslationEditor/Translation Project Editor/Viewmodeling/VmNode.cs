﻿using SCR.Tools.TranslationEditor.Data;
using SCR.Tools.TranslationEditor.Data.Events;
using SCR.Tools.UndoRedo;
using SCR.Tools.Viewmodeling;

namespace SCR.Tools.TranslationEditor.ProjectEditor.Viewmodeling
{
    public abstract class VmNode : BaseViewModel
    {
        protected readonly Node _node;

        protected readonly VmProject _project;

        public string Name
            => _node.Name;

        public string? Description
            => _node.Description;

        public NodeState State
            => _node.State;

        public virtual bool CanExpand 
            => false;

        public virtual bool Expanded { get; set; }

        public virtual string DefaultValue
            => "";

        protected VmNode(VmProject project, Node node)
        {
            _node = node;
            _project = project;

            _node.NodeStateChanged += StateChanged;
        }

        ~VmNode()
        {
            _node.NodeStateChanged -= StateChanged;
        }

        protected void TrackNotifyProperty(string propertyName)
        {
            ChangeTracker.Global.GroupNotifyPropertyChanged(OnPropertyChanged, propertyName);
        }

        private void StateChanged(Node node, NodeStateChangedEventArgs args)
        {
            TrackNotifyProperty(nameof(State));
        }

        public abstract void RefreshNodeValues();
    }
}
