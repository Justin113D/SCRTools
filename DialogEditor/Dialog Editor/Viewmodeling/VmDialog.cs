using SCR.Tools.DialogEditor.Data;
using SCR.Tools.UndoRedo;
using SCR.Tools.UndoRedo.Collections;
using SCR.Tools.Viewmodeling;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;

namespace SCR.Tools.DialogEditor.Viewmodeling
{
    public class VmDialog : BaseViewModel
    {
        private readonly TrackList<VmNode> _nodes;

        private VmNode? _activeNode;

        public VmMain Main { get; }

        public Dialog Data { get; }


        private readonly TrackDictionary<Node, VmNode> _nodeTable;

        public ReadOnlyObservableCollection<VmNode> Nodes { get; private set; }

        #region Wrapper Properties

        public string Name
        {
            get => Data.Name;
            set
            {
                BeginChangeGroup();
                Data.Name = value;
                PostChangeGroupAction(() => OnPropertyChanged(nameof(Name)));
                EndChangeGroup();
            }
        }

        public string Author
        {
            get => Data.Author;
            set
            {
                BeginChangeGroup();
                Data.Author = value;
                PostChangeGroupAction(() => OnPropertyChanged(nameof(Author)));
                EndChangeGroup();
            }
        }

        public string Description
        {
            get => Data.Description;
            set
            {
                BeginChangeGroup();
                Data.Description = value;
                PostChangeGroupAction(() => OnPropertyChanged(nameof(Description)));
                EndChangeGroup();
            }
        }

        #endregion

        #region Interaction Properties

        public VmNode? ActiveNode
        {
            get => _activeNode;
            set
            {
                if (_activeNode == value)
                {
                    return;
                }

                VmNode? oldValue = _activeNode;

                _activeNode = value;

                oldValue?.NotifyActiveChanged();
                _activeNode?.NotifyActiveChanged();
            }
        }

        public ObservableCollection<VmNode> Selected { get; set; }

        #endregion

        #region Relay Commands

        public RelayCommand CmdSortNodes
            => new(SortNodes);

        #endregion

        public VmDialog(VmMain mainVM, Dialog dialog)
        {
            Main = mainVM;
            Data = dialog;

            Selected = new();

            Dictionary<Node, VmNode> internalNodeTable = new();
            ObservableCollection<VmNode> internalNodes = new();

            foreach (Node node in dialog.Nodes)
            {
                VmNode vmnode = new(this, node);
                internalNodes.Add(vmnode);
                internalNodeTable.Add(node, vmnode);
            }
            _nodeTable = new(internalNodeTable);


            _nodes = new(internalNodes);
            Nodes = new(internalNodes);

            foreach (VmNode vmnode in Nodes)
            {
                foreach (VmNodeOutput vmout in vmnode.Outputs)
                {
                    if (vmout.Data.Output == null)
                        continue;

                    vmout.Connected = _nodeTable[vmout.Data.Output];
                }
            }

        }

        private void SortNodes()
        {
            BeginChangeGroup();

            Data.Sort();

            _nodes.Clear();
            _nodes.AddRange(
                Nodes.OrderBy(x => Data.Nodes.IndexOf(x.Data))
                );

            EndChangeGroup();
        }

        public void DeselectAll()
        {
            foreach (VmNode vmNode in Selected.ToArray())
            {
                vmNode.Selected = false;
            }
        }
    }
}
