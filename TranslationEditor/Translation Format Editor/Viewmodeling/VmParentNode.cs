using SCR.Tools.TranslationEditor.Data;
using SCR.Tools.TranslationEditor.Data.Events;
using SCR.Tools.UndoRedo;
using SCR.Tools.UndoRedo.Collections;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;
using System.Collections.ObjectModel;
using SCR.Tools.WPF.Viewmodeling;

namespace SCR.Tools.TranslationEditor.FormatEditor.Viewmodeling
{
    public class VmParentNode : VmNode
    {
        public ParentNode ParentNode
            => (ParentNode)Node;

        private readonly TrackList<VmNode> _childNodes;

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

                if (!_expanded)
                {
                    foreach (VmNode vmNode in ChildNodes)
                    {
                        vmNode.DeselectHierarchy();
                    }
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
            ObservableCollection<VmNode> internalChildren = new();

            _childNodes = new(internalChildren);
            ChildNodes = new(internalChildren);
            node.ChildrenChanged += ChildrenChanged;
            node.HeaderChanged += HeaderChanged;
        }

        private void ChildrenChanged(ParentNode node, NodeChildrenChangedEventArgs args)
        {
            BeginChangeGroup();

            bool canExpandBefore = ChildNodes.Count > 0;

            if (args.FromIndex > -1)
            {
                _childNodes.RemoveAt(args.FromIndex);
            }

            if (args.ToIndex > -1)
            {
                VmNode vmNode = _format.GetNodeViewmodel(ParentNode.ChildNodes[args.ToIndex]);
                _childNodes.Insert(args.ToIndex, vmNode);
            }

            if(!canExpandBefore && CanExpand)
            {
                TrackChange(
                    () => { },
                    () => Expanded = false);
            }

            TrackNotifyProperty(nameof(CanExpand));

            EndChangeGroup();
        }

        private void HeaderChanged(Node node, NodeHeaderChangedEventArgs args)
        {
            if (args.NewHeader == ParentNode.Header)
                return;

            TrackChange(
                () =>
                {
                    ParentNode.ChildrenChanged -= ChildrenChanged;
                    ParentNode.HeaderChanged -= HeaderChanged;
                },
                () =>
                {
                    ParentNode.ChildrenChanged += ChildrenChanged;
                    ParentNode.HeaderChanged += HeaderChanged;
                });
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

        public override void DeselectHierarchy()
        {
            Selected = false;
            Active = false;

            if (!Expanded)
            {
                return;
            }

            foreach (VmNode vmNode in ChildNodes)
            {
                vmNode.DeselectHierarchy();
            }
        }

        public override void InsertBelow()
        {
            if (!Expanded)
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
