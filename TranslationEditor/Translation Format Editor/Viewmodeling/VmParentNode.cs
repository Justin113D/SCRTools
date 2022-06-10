using SCR.Tools.TranslationEditor.Data;
using SCR.Tools.TranslationEditor.Data.Events;
using SCR.Tools.UndoRedo;
using SCR.Tools.UndoRedo.ListChange;
using SCR.Tools.Viewmodeling;
using System;
using System.Collections.ObjectModel;

namespace SCR.Tools.TranslationEditor.FormatEditor.Viewmodeling
{
    public class VmParentNode : VmNode
    {
        public ParentNode ParentNode
            => (ParentNode)Node;

        private readonly ObservableCollection<VmNode> _childNodes;

        private bool _expanded;

        public ReadOnlyObservableCollection<VmNode> ChildNodes { get; }

        /// <summary>
        /// Whether the node is expanded or collapsed
        /// </summary>
        public override bool Expanded
        {
            get => _expanded;
            set
            {
                if (_expanded == value)
                    return;

                _expanded = value;

                if (_expanded && _childNodes.Count != ParentNode.ChildNodes.Count)
                {
                    CreateChildViewModels();
                }

            }
        }

        /// <summary>
        /// Whether the node can be expanded at all
        /// </summary>
        public override bool CanExpand
            => ParentNode.ChildNodes.Count > 0;

        public RelayCommand CmdAddNewStringNode
            => new(AddNewStringNode);

        public RelayCommand CmdAddNewParentNode
            => new(AddNewParentNode);

        public VmParentNode(VmFormat format, ParentNode node)
            : base(format, node)
        {
            _childNodes = new();
            ChildNodes = new(_childNodes);
            node.ChildrenChanged += ChildrenChanged;
            node.HeaderChanged += HeaderChanged;
        }

        private void ChildrenChanged(ParentNode node, NodeChildrenChangedEventArgs args)
        {
            _format.FormatTracker.BeginGroup();

            if (args.FromIndex > -1)
            {
                _format.FormatTracker.TrackChange(
                    new ChangeListRemoveAt<VmNode>(
                        _childNodes, args.FromIndex));
            }

            if(args.ToIndex > -1)
            {
                VmNode vmNode = _format.GetNodeViewmodel(ParentNode.ChildNodes[args.ToIndex]);
                _format.FormatTracker.TrackChange(
                    new ChangeListInsert<VmNode>(
                        _childNodes, vmNode, args.ToIndex));
            }

            TrackNotifyProperty(nameof(CanExpand));

            _format.FormatTracker.EndGroup();
        }

        private void HeaderChanged(Node node, NodeHeaderChangedEventArgs args)
        {
            if (args.NewHeader == ParentNode.Header)
                return;

            _format.FormatTracker.TrackChange(new Change(
                () =>
                {
                    ParentNode.ChildrenChanged -= ChildrenChanged;
                    ParentNode.HeaderChanged -= HeaderChanged;
                },
                () =>
                {
                    ParentNode.ChildrenChanged += ChildrenChanged;
                    ParentNode.HeaderChanged += HeaderChanged;
                }
            ));
        }

        private void CreateChildViewModels()
        {
            foreach (Node node in ParentNode.ChildNodes)
            {
                VmNode vmNode = _format.GetNodeViewmodel(node);
                _childNodes.Add(vmNode);
            }
        }


        private void AddNewStringNode()
        {
            ParentNode.AddChildNode(new StringNode("String", ""));
            Expanded = true;
        }

        private void AddNewParentNode()
        {
            ParentNode.AddChildNode(new ParentNode("Category"));
            Expanded = true;
        }

        public void ExpandAll()
        {
            Expanded = true;
            foreach (VmNode node in ChildNodes)
            {
                if (node is VmParentNode parent)
                {
                    parent.ExpandAll();
                }
            }
        }

        public void CollapseAll()
        {
            Expanded = false;
            foreach (VmNode node in ChildNodes)
            {
                if (node is VmParentNode parent)
                {
                    parent.CollapseAll();
                }
            }
        }


        public override void InsertBelow()
        {
            if(!Expanded)
            {
                base.InsertBelow();
            }
            else
            {
                _format.MoveSelected(ParentNode, 0);
            }
        }
    }
}
