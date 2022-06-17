using SCR.Tools.UndoRedo;
using SCR.Tools.Viewmodeling;
using SCR.Tools.DialogEditor.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace SCR.Tools.DialogEditor.Viewmodel
{
    public class VmGrid : BaseViewModel
    {
        /// <summary>
        /// Grid background cell width
        /// </summary>
        public const int brushDim = 50;

        /// <summary>
        /// Half the width of a background cell
        /// </summary>
        public const int halfBrushDim = brushDim / 2;

        #region Commands

        public RelayCommand Cmd_DeleteNode
            => new(DeleteSelectedNodes);

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

        private bool _moved;

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
                Tracker.BeginChangeGroup();
                Data.Name = value;
                Tracker.PostChangeGroupAction(() => OnPropertyChanged(nameof(Name)));
                Tracker.EndChangeGroup();
            }
        }

        public string Author
        {
            get => Data.Author;
            set
            {
                Tracker.BeginChangeGroup();
                Data.Author = value;
                Tracker.PostChangeGroupAction(() => OnPropertyChanged(nameof(Author)));
                Tracker.EndChangeGroup();
            }
        }

        public string Description
        {
            get => Data.Description;
            set
            {
                Tracker.BeginChangeGroup();
                Data.Description = value;
                Tracker.PostChangeGroupAction(() => OnPropertyChanged(nameof(Description)));
                Tracker.EndChangeGroup();
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
                if(_active == value)
                    return;

                VmNode oldActive = _active;

                Tracker.BeginChangeGroup();

                Tracker.TrackChange(new ChangedValue<VmNode>(
                    (v) => _active = v,
                    oldActive,
                    value
                ));

                Tracker.PostChangeGroupAction(() =>
                {
                    oldActive?.RefreshActive();
                    value?.RefreshActive();
                    OnPropertyChanged(nameof(ListActive));
                });

                Tracker.EndChangeGroup();
            }
        }

        /// <summary>
        /// Selected-element for the node list
        /// </summary>
        public VmNode ListActive
        {
            get => _active;
            set
            {
                if(_active == value)
                    return;

                Select(value, false);
            }
        }

        /// <summary>
        /// The selected nodes
        /// </summary>
        public ObservableCollection<VmNode> Selected { get; }

        /// <summary>
        /// Grabbed Node
        /// </summary>
        public VmNode Grabbed
        {
            get => _grabbed;
            private set
            {
                if(value == _grabbed)
                    return;

                if(value != null)
                    Tracker.BeginChangeGroup();
                else
                {
                    if(_moved)
                    {
                        foreach(VmNode s in Selected)
                            s.UpdateDataPosition();
                        _moved = false;
                    }
                    else if(Selected.Count > 1)
                        Select(_grabbed, false);

                    Tracker.EndChangeGroup();
                }

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
                    Tracker.BeginChangeGroup();
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
            Selected = new();

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
                        vmout.VmOutput = viewmodelPairs[vmout.Data.Output];

            _tracker = new();
        }

        #region Moving nodes

        /// <summary>
        /// Grabs/Ungrabs a node
        /// </summary>
        /// <param name="select">The node to select</param>
        /// <param name="multiModifier">Whether the shift/multi key was pressed</param>
        public void Grab(VmNode select, bool multiModifier)
        {
            if(select != null)
            {
                if(!multiModifier)
                    Grabbed = select;

                if(!select.IsSelected || multiModifier)
                    Select(select, multiModifier);
            }
            else
            {
                Grabbed = null;
            }
        }

        /// <summary>
        /// Selects a single node
        /// </summary>
        /// <param name="select">The node to select</param>
        /// <param name="multiModifier">Whether the shift/multi key was pressed</param>
        private void Select(VmNode select, bool multiModifier)
        {
            Tracker.BeginChangeGroup();

            if(multiModifier)
            {
                if(!select.IsSelected)
                    Active = select;

                Tracker.TrackChange(new ChangedListSingleEntry<VmNode>(
                    Selected,
                    select,
                    select.IsSelected ? null : Selected.Count,
                    () => select.RefreshSelected()));
            }
            else
            {
                Active = select;

                VmNode[] OldContents = Selected.ToArray();

                Tracker.TrackChange(new ChangedList<VmNode>(
                    Selected,
                    new VmNode[] { select },
                    () => select.RefreshSelected()));

                Tracker.PostChangeGroupAction(() =>
                {
                    foreach(var o in OldContents)
                        o.RefreshSelected();
                });

            }
            Tracker.EndChangeGroup();
        }

        /// <summary>
        /// Performs select on multiple nodes (unused rn)
        /// </summary>
        /// <param name="select"></param>
        /// <param name="multiModifier"></param>
        public void SelectMultiple(VmNode[] select, bool multiModifier)
        {
            if(!multiModifier)
            {
                VmNode[] changedContents = select.Concat(Selected)
                    .GroupBy(x => x)
                    .Where(x => !x.Skip(1).Any())
                    .Select(x => x.Key)
                    .ToArray();

                Tracker.TrackChange(new ChangedList<VmNode>(
                    Selected,
                    select,
                    () =>
                    {
                        foreach(var c in changedContents)
                            c.RefreshSelected();
                    }
                ));
            }
            else
            {
                VmNode[] newContents = select
                    .Where(x => !Selected.Contains(x))
                    .ToArray();

                VmNode[] concated = Selected.Concat(newContents).ToArray();

                Tracker.TrackChange(new ChangedList<VmNode>(
                    Selected,
                    concated,
                    () =>
                    {
                        foreach(var c in newContents)
                            c.RefreshSelected();
                    }
                ));
            }
        }

        /// <summary>
        /// Lets go of the grabbed connection/node
        /// </summary>
        public void LetGo()
        {
            Connecting = null;
            Grab(null, default);
        }

        /// <summary>
        /// Moves whatever is grabbed <br/>
        /// nodes require difference, while connections the absolute position
        /// </summary>
        /// <param name="dif"></param>
        /// <param name="absolute"></param>
        public void MoveGrabbed(Point dif)
        {
            _moved = true;
            foreach(VmNode s in Selected)
                s.Move(dif);
        }

        /// <summary>
        /// Moves a connection
        /// </summary>
        /// <param name="absolute"></param>
        public void MoveConnection(Point absolute)
            => Connecting?.UpdateEndPosition(absolute);

        #endregion

        #region Handling node data

        /// <summary>
        /// Adds a new node at Mouse position
        /// </summary>
        public void CreateNode(Point position)
        {
            if(Grabbed != null || Connecting != null)
                return;

            Tracker.BeginChangeGroup();

            Node node = Data.CreateNode();

            Tracker.TrackChange(new ChangedListSingleEntry<VmNode>(
                Nodes,
                new(this, node, position),
                Nodes.Count,
                null
            ));

            Tracker.EndChangeGroup();

            Main.SetFeedback("Added node", true);
        }

        /// <summary>
        /// Deletes all selected nodes
        /// </summary>
        private void DeleteSelectedNodes()
        {
            if(Grabbed != null || Connecting != null || Selected.Count == 0)
                return;

            Tracker.BeginChangeGroup();

            if(Active.IsSelected)
                Active = null;

            VmNode[] newContents = Selected.Concat(Nodes)
                .GroupBy(x => x)
                .Where(x => !x.Skip(1).Any())
                .Select(x => x.Key)
                .ToArray();

            foreach(VmNode n in Selected)
            {
                n.Disconnect();
                Data.RemoveNode(n.Data);
            }

            VmNode[] toRemove = Selected.ToArray();

            Tracker.TrackChange(new ChangedList<VmNode>(
                Selected,
                Array.Empty<VmNode>(),
                () =>
                {
                    foreach(var c in toRemove)
                        c.RefreshSelected();
                }
            ));

            Tracker.TrackChange(new ChangedList<VmNode>(
                Nodes,
                newContents,
                null
            ));

            Tracker.EndChangeGroup();

            if(toRemove.Length > 1)
                Main.SetFeedback($"Deleted {toRemove.Length} nodes", true);
            else
                Main.SetFeedback("Deleted node", true);
        }

        /// <summary>
        /// Registers an Output to display the line of
        /// </summary>
        /// <param name="vmOutput"></param>
        public void RegisterOutput(VmNodeOutput vmOutput)
        {
            Tracker.TrackChange(new ChangedListSingleEntry<VmNodeOutput>(
                Outputs,
                vmOutput,
                Outputs.Count,
                null
            ));
        }

        /// <summary>
        /// Deregisters an Output of which the line is being displayed
        /// </summary>
        /// <param name="vmOutput"></param>
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

            Tracker.BeginChangeGroup();

            foreach(var n in Nodes)
            {
                n.Move(offset);
                n.UpdateDataPosition();
            }

            Tracker.EndChangeGroup();

            Main.SetFeedback("Recentered node tree", true);
        }

        /// <summary>
        /// Sorts the nodes
        /// </summary>
        private void Sort()
        {
            Tracker.BeginChangeGroup();

            Data.Sort();

            var oldValue = Nodes;

            Tracker.TrackChange(new ChangedValue<ObservableCollection<VmNode>>(
                (v) => Nodes = v,
                oldValue,
                new ObservableCollection<VmNode>(Nodes.OrderBy(x => Data.Nodes.IndexOf(x.Data)))
            ));

            Tracker.EndChangeGroup();

            Main.SetFeedback("Sorted nodes", true);
        }

        #endregion

        /// <summary>
        /// Performs an Undo
        /// </summary>
        public void Undo()
        {
            if(Grabbed != null || Connecting != null)
                return;

            if(Tracker.Undo())
                Main.SetFeedback("Performed Undo", true);
        }

        /// <summary>
        /// Performs a Redo
        /// </summary>
        public void Redo()
        {
            if(Grabbed != null || Connecting != null)
                return;

            if(Tracker.Redo())
                Main.SetFeedback("Performed Redo", true);
        }


        public static Point FromGridSpace(int x, int y)
        {
            return new(
                x * brushDim + halfBrushDim,
                y * brushDim + halfBrushDim
                );
        }

        public static Point ToGridSpace(Point TransformSpace)
        {
            return new(
               ((int)TransformSpace.X - halfBrushDim * (TransformSpace.X < 0 ? 2 : 0)) / brushDim,
               ((int)TransformSpace.Y - halfBrushDim * (TransformSpace.Y < 0 ? 2 : 0)) / brushDim
           );
        }

    }
}