using SCRCommon.Viewmodels;
using SCRDialogEditor.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace SCRDialogEditor.Viewmodel
{
    public class VmGrid : BaseViewModel
    {
        #region Commands

        /// <summary>
        /// Deletes the active node from the grid
        /// </summary>
        public RelayCommand Cmd_DeleteNode
            => new(DeleteActiveNode);

        public RelayCommand Cmd_RecenterContents
            => new(RecenterContents);

        public RelayCommand Cmd_SortNodes
            => new(Sort);

        public RelayCommand Cmd_Undo
            => new(Undo);

        public RelayCommand Cmd_Redo
            => new(Redo);

        #endregion

        #region Private Fields

        private VmNode _active;

        private VmNode _grabbed;

        private VmNodeOutput _connecting;

        private VmNode _wasConnected;

        private readonly ChangeTracker _tracker;

        #endregion

        #region Properties

        #region Source Properties
        public VmMain Main { get; }

        /// <summary>
        /// Dialog data
        /// </summary>
        public Dialog Data { get; }

        /// <summary>
        /// Nodes to display
        /// </summary>
        public ObservableCollection<VmNode> Nodes { get; private set; }

        /// <summary>
        /// All note outputs
        /// </summary>
        public ObservableCollection<VmNodeOutput> Outputs { get; }

        public ChangeTracker Tracker
        {
            get
            {
                if(ChangeTracker.Global != _tracker)
                    _tracker.Use();
                return _tracker;
            }
        }

        #endregion

        #region Wrapper Properties

        public string Name
        {
            get => Data.Name;
            set
            {
                Tracker.BeginGroup();
                Data.Name = value;
                Tracker.PostGroupAction(() => OnPropertyChanged(nameof(Name)));
                Tracker.EndGroup();
            }
        }

        public string Author
        {
            get => Data.Author;
            set
            {
                Tracker.BeginGroup();
                Data.Author = value;
                Tracker.PostGroupAction(() => OnPropertyChanged(nameof(Author)));
                Tracker.EndGroup();
            }
        }

        public string Description
        {
            get => Data.Description;
            set
            {
                Tracker.BeginGroup();
                Data.Description = value;
                Tracker.PostGroupAction(() => OnPropertyChanged(nameof(Description)));
                Tracker.EndGroup();
            }
        }

        #endregion

        #region Interaction Properties

        /// <summary>
        /// Selected Node to edit
        /// </summary>
        public VmNode Active
        {
            get => _active;
            set
            {
                VmNode oldActive = _active;

                Tracker.BeginGroup();

                Tracker.TrackChange(new ChangedValue<VmNode>(
                    (v) => _active = v,
                    oldActive,
                    value
                ));

                Tracker.PostGroupAction(() =>
                {
                    oldActive?.RefreshActive();
                    value?.RefreshActive();
                });

                Tracker.EndGroup();
            }
        }

        /// <summary>
        /// Grabbed Node
        /// </summary>
        public VmNode Grabbed
        {
            get => _grabbed;
            set
            {
                if(_grabbed == value)
                    return;

                _grabbed?.EndGrab();
                _grabbed = value;
            }
        }

        /// <summary>
        /// The nodeoutput that is currently being "dragged"
        /// </summary>
        public VmNodeOutput Connecting
        {
            get => _connecting;
            set
            {
                if(value == _connecting)
                    return;


                if(value != null)
                {
                    Tracker.BeginGroup();
                    _wasConnected = value.VmOutput;
                    value.VmOutput = null;
                    value.Displaying = true;
                }
                else
                {
                    if(_wasConnected != null && _connecting.VmOutput == null)
                    {
                        _connecting.Displaying = false;
                    }

                    // discard only if it was and still is null
                    Tracker.EndGroup(_wasConnected == null && _connecting.VmOutput == null);
                }

                _connecting = value;
            }
        }

        #endregion

        #endregion

        public VmGrid(VmMain mainVM, Dialog dialog)
        {
            _tracker = new();
            Main = mainVM;
            Data = dialog;

            Outputs = new();
            Nodes = new();

            Dictionary<Node, VmNode> viewmodelPairs = new();
            foreach(Node node in dialog.Nodes)
            {
                VmNode vmnode = new(this, node);
                Nodes.Add(vmnode);
                viewmodelPairs.Add(node, vmnode);
            }

            foreach(VmNode vmnode in Nodes)
                foreach(VmNodeOutput vmout in vmnode.Outputs)
                    if(vmout.Data.Output != null)
                    {
                        vmout.VmOutput = viewmodelPairs[vmout.Data.Output];
                    }

        }


        /// <summary>
        /// Lets go of the grabbed connection/node
        /// </summary>
        public void LetGo()
        {
            Grabbed = null;
            Connecting = null;
        }

        /// <summary>
        /// Moves whatever is grabbed <br/>
        /// nodes require difference, while connections the absolute position
        /// </summary>
        /// <param name="dif"></param>
        /// <param name="absolute"></param>
        public void MoveGrabbed(Point dif, Point absolute)
        {
            Grabbed?.Move(dif);
            Connecting?.UpdateEndPosition(absolute);
        }

        /// <summary>
        /// Adds a new node at Mouse position
        /// </summary>
        public void CreateNode(Point position)
        {
            if(Grabbed != null || Connecting != null)
                return;

            Tracker.BeginGroup();

            Node node = Data.CreateNode();

            Tracker.TrackChange(new ChangedListSingleEntry<VmNode>(
                Nodes,
                new(this, node, position),
                Nodes.Count,
                null
            ));

            Tracker.EndGroup();
        }

        /// <summary>
        /// Deletes a node from the grid
        /// </summary>
        /// <param name="node"></param>
        public void DeleteNode(VmNode node)
        {
            Tracker.BeginGroup();

            node.Disconnect();

            Tracker.TrackChange(new ChangedListSingleEntry<VmNode>(
                Nodes,
                node,
                null,
                null
            ));

            Data.RemoveNode(node.Data);

            Tracker.EndGroup();
        }

        /// <summary>
        /// Deletes the active noe
        /// </summary>
        private void DeleteActiveNode()
        {
            if(Grabbed != null || Connecting != null)
                return;

            if(Active == null)
                return;

            Tracker.BeginGroup();

            DeleteNode(Active);
            Active = null;

            Tracker.EndGroup();
        }


        public void RegisterOutput(VmNodeOutput vmOutput)
        {
            Tracker.TrackChange(new ChangedListSingleEntry<VmNodeOutput>(
                Outputs,
                vmOutput,
                Outputs.Count,
                null
            ));
        }

        public void DeregisterOutput(VmNodeOutput vmOutput)
        {
            Tracker.TrackChange(new ChangedListSingleEntry<VmNodeOutput>(
                Outputs,
                vmOutput,
                null,
                null
            ));
        }

        /// <summary>
        /// Recenters the contents by the start of the node structure
        /// </summary>
        private void RecenterContents()
        {
            if(Nodes.Count == 0)
                return;

            // find the starter node
            VmNode start = Nodes.First(x => x.Data == Data.StartNode);
            Point offset = new(-start.Position.X, -start.Position.Y);

            Tracker.BeginGroup();

            foreach(var n in Nodes)
            {
                n.Move(offset);
                n.UpdateDataPosition();
            }

            Tracker.EndGroup();
        }

        /// <summary>
        /// Sorts the nodes
        /// </summary>
        private void Sort()
        {
            Tracker.BeginGroup();

            Data.Sort();

            var oldValue = Nodes;

            Tracker.TrackChange(new ChangedValue<ObservableCollection<VmNode>>(
                (v) => Nodes = v,
                oldValue,
                new ObservableCollection<VmNode>(Nodes.OrderBy(x => Data.Nodes.IndexOf(x.Data)))
            ));

            Tracker.EndGroup();
        }

        public void Undo()
        {
            if(Grabbed != null || Connecting != null)
                return;
            if(Tracker.Undo())
            {
                // TODO Notify that the undo was successfull
            }
        }

        public void Redo()
        {
            if(Grabbed != null || Connecting != null)
                return;
            if(Tracker.Redo())
            {
                // TODO Notify that the redo was successfull
            }
        }
    }
}

