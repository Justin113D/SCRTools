using SCR.Tools.DialogEditor.Data;
using SCR.Tools.UndoRedo;
using SCR.Tools.UndoRedo.Collections;
using SCR.Tools.Viewmodeling;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;

namespace SCR.Tools.DialogEditor.Viewmodeling
{
    public class VmDialog : BaseViewModel
    {
        private readonly TrackList<VmNode> _nodes;

        private VmNode? _activeNode;


        public VmMain Main { get; }

        public Dialog Data { get; }


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


        private readonly TrackDictionary<Node, VmNode> _nodeTable;

        public ReadOnlyObservableCollection<VmNode> Nodes { get; }

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

        public RelayCommand CmdDeleteSelectedNodes
            => new(DeleteSelectedNodes);

        public RelayCommand CmdOrganizeNodes
            => new(OrganizeNodes);

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
                vmnode.InitConnections();
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

        public VmNode GetViewmodel(Node node)
        {
            return _nodeTable[node];
        }

        public void CreateNode(int locationX, int locationY)
        {
            BeginChangeGroup();

            Node node = Data.CreateNode();
            node.LocationX = locationX;
            node.LocationY = locationY;

            VmNode vmNode = new(this, node);

            _nodeTable.Add(node, vmNode);
            _nodes.Add(vmNode);

            EndChangeGroup();

            Main.SetMessage($"Created node!", false);
        }

        private void DeleteSelectedNodes()
        {
            if(Selected.Count == 0)
            {
                return;
            }

            BeginChangeGroup();

            VmNode[] selectedNodes = Selected.ToArray();
            foreach (VmNode node in selectedNodes)
            {   
                TrackChange(
                    () => {
                        node.Active = false;
                        node.Selected = false;
                    },
                    () => { });

                Data.RemoveNode(node.Data);
                _nodes.Remove(node);
                _nodeTable.Remove(node.Data);
            }

            EndChangeGroup();

            Main.SetMessage($"Deleted {selectedNodes.Length} nodes", false);
        }

        private void OrganizeNodes()
        {
            if (Nodes.Count == 0)
                return;

            BeginChangeGroup();

            // find the starter node
            VmNode start = _nodeTable[Data.StartNode];
            int offsetX = start.LocationX;
            int offsetY = start.LocationY;

            foreach (var n in Nodes)
            {
                n.LocationX -= offsetX;
                n.LocationY -= offsetY;
            }

            EndChangeGroup();

            Main.SetMessage("Recentered node tree", false);
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
