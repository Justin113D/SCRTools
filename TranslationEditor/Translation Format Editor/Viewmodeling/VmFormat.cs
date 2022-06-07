using SCR.Tools.TranslationEditor.Data;
using SCR.Tools.TranslationEditor.Data.Events;
using SCR.Tools.UndoRedo;
using SCR.Tools.UndoRedo.ListChange;
using SCR.Tools.Viewmodeling;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCR.Tools.TranslationEditor.FormatEditor.Viewmodeling
{
    public class VmFormat : BaseViewModel
    {
        /// <summary>
        /// Headernode holding the format information
        /// </summary>
        private readonly HeaderNode _header;

        /// <summary>
        /// Change tracker for the viewmodels
        /// </summary>
        public ChangeTracker FormatTracker { get; }

        /// <summary>
        /// Default Language that the project targets
        /// </summary>
        public string Language
        {
            get => _header.Language;
            set
            {
                if (_header.Language == value)
                    return;

                FormatTracker.BeginGroup();

                _header.Language = value;
                TrackNotifyProperty(nameof(Language));

                FormatTracker.EndGroup();
            }
        }

        /// <summary>
        /// Target name specified in the header
        /// </summary>
        public string TargetName
        {
            get => _header.Name;
            set
            {
                if (_header.Name == value)
                    return;

                FormatTracker.BeginGroup();

                _header.Name = value;
                TrackNotifyProperty(nameof(TargetName));

                FormatTracker.EndGroup();
            }
        }

        /// <summary>
        /// Current version in the header
        /// </summary>
        public string Version
        {
            get => _header.Version.ToString();
            set
            {
                Version newVersion;
                try
                {
                    newVersion = new(value);
                }
                catch
                {
                    return;
                }

                if (newVersion <= _header.Version)
                    return;

                FormatTracker.BeginGroup();

                _header.Version = newVersion;
                TrackNotifyProperty(nameof(Version));

                FormatTracker.EndGroup();
            }
        }

        /// <summary>
        /// Private collection for the node viewmodels
        /// </summary>
        private readonly ObservableCollection<VmNode> _nodes;

        /// <summary>
        /// Top level node viewmodels
        /// </summary>
        public ReadOnlyObservableCollection<VmNode> Nodes { get; }

        private readonly Dictionary<Node, VmNode> _nodeTable;

        public VmFormat(HeaderNode data, ChangeTracker projectTracker)
        {
            _header = data;
            FormatTracker = projectTracker;

            _nodeTable = new();
            _nodes = new();
            Nodes = new(_nodes);
            CreateNodes();

            _header.ChildrenChanged += OnChildrenChanged;
        }

        public VmNode GetNodeViewmodel(Node node)
        {
            if (!_nodeTable.TryGetValue(node, out VmNode? vmNode))
            {
                if (node is ParentNode p)
                {
                    vmNode = new VmParentNode(this, p);
                }
                else if (node is StringNode s)
                {
                    vmNode = new VmStringNode(this, s);
                }
                else
                {
                    throw new NotSupportedException(node.GetType().Name + " is not a valid node type");
                }
                node.HeaderChanged += OnHeaderChanged;
                _nodeTable.Add(node, vmNode);
            }
            return vmNode;
        }

        /// <summary>
        /// Create the top level node viewmodels
        /// </summary>
        private void CreateNodes()
        {
            foreach (Node node in _header.ChildNodes)
            {
                VmNode vmNode = GetNodeViewmodel(node);
                _nodes.Add(vmNode);
            }
        }

        private void OnChildrenChanged(ParentNode node, NodeChildrenChangedEventArgs args)
        {
            FormatTracker.BeginGroup();

            if (args.FromIndex > -1)
            {
                FormatTracker.TrackChange(
                    new ChangeListRemoveAt<VmNode>(
                        _nodes, args.FromIndex));
            }

            if (args.ToIndex > -1)
            {
                VmNode vmNode = GetNodeViewmodel(_header.ChildNodes[args.ToIndex]);
                FormatTracker.TrackChange(
                    new ChangeListInsert<VmNode>(
                        _nodes, vmNode, args.ToIndex));
            }

            FormatTracker.EndGroup();
        }

        private void OnHeaderChanged(Node node, NodeHeaderChangedEventArgs args)
        {
            if (args.NewHeader == _header)
                return;
            
            FormatTracker.BeginGroup();
            VmNode vmNode = _nodeTable[node];
            FormatTracker.TrackChange(new Change(
                () =>
                {
                    _nodeTable.Remove(node);
                    node.HeaderChanged -= OnHeaderChanged;
                },
                () =>
                { 
                    _nodeTable.Add(node, vmNode);
                    node.HeaderChanged += OnHeaderChanged;
                }
            ));
            FormatTracker.EndGroup();
        }

        /// <summary>
        /// Expands all nodes
        /// </summary>
        public void ExpandAll()
        {
            foreach (VmNode node in Nodes)
            {
                if (node is VmParentNode parent)
                {
                    parent.ExpandAll();
                }
            }
        }

        /// <summary>
        /// Collapses all nodes
        /// </summary>
        public void CollapseAll()
        {
            foreach (VmNode node in Nodes)
            {
                if (node is VmParentNode parent)
                {
                    parent.CollapseAll();
                }
            }
        }


        public string WriteFormat()
        {
            return _header.WriteFormat();
        }

        /// <summary>
        /// Tells the current change tracker grouping to notify the viewmodel that a property changed on undo/redo
        /// </summary>
        /// <param name="propertyName"></param>
        private void TrackNotifyProperty(string propertyName)
        {
            FormatTracker.GroupNotifyPropertyChanged(OnPropertyChanged, propertyName);
        }
    }
}
