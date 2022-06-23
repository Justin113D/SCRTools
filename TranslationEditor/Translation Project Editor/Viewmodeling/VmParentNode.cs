using SCR.Tools.TranslationEditor.Data;
using System.Collections.ObjectModel;

namespace SCR.Tools.TranslationEditor.ProjectEditor.Viewmodeling
{
    internal class VmParentNode : VmNode
    {
        private ParentNode ParentNode
            => (ParentNode)_node;

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

        public VmParentNode(VmProject project, ParentNode node)
            : base(project, node)
        {
            _childNodes = new();
            ChildNodes = new(_childNodes);
        }

        private void CreateChildViewModels()
        {
            foreach (Node node in ParentNode.ChildNodes)
            {
                if (node is ParentNode p)
                {
                    _childNodes.Add(new VmParentNode(_project, p));
                }
                else if (node is StringNode s)
                {
                    _childNodes.Add(new VmStringNode(_project, s));
                }
            }
        }

        public override void RefreshNodeValues()
        {
            foreach (VmNode node in _childNodes)
            {
                node.RefreshNodeValues();
            }
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
    }
}
