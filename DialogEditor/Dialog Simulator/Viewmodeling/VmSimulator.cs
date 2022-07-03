using SCR.Tools.Dialog.Data;
using SCR.Tools.Dialog.Simulator.Viewmodeling.Condition;
using SCR.Tools.UndoRedo;
using SCR.Tools.UndoRedo.Collections;
using SCR.Tools.Viewmodeling;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;

namespace SCR.Tools.Dialog.Simulator.Viewmodeling
{
    internal class VmSimulator : BaseViewModel
    {
        private VmNode _activeNode;
        private bool _rightPortraitActive;


        public ChangeTracker SimulatorTracker { get; }

        public DialogContainer Data { get; }

        public DialogOptions Options { get; }

        public VmNode EntryNode { get; }

        public ReadOnlyObservableCollection<VmNode> Nodes { get; }

        public ReadOnlyDictionary<Node, VmNode> NodeViewmodelLUT { get; }

        public TrackSet<int> SharedDisabledIndices { get; }

        /// <summary>
        /// Used to color code the current outputs in the node table
        /// </summary>
        public Dictionary<VmNode, int> OutputNumbers { get; }

        public VmConditionData ConditionData { get; }

        public VmNode ActiveNode
            => _activeNode;

        public VmNodeOutput? LeftPortrait { get; set; }

        public VmNodeOutput? RightPortrait { get; set; }

        public bool RightPortraitActive
        {
            get => _rightPortraitActive;
            set
            {
                if (_rightPortraitActive == value)
                {
                    return;
                }

                BeginChangeGroup();

                TrackValueChange(
                    (v) => _rightPortraitActive = v, _rightPortraitActive, value);
                TrackNotifyProperty(nameof(RightPortraitActive));

                EndChangeGroup();
            }
        }

        public bool HasNextNode
            => ActiveNode.ActiveOutput.Connected != null;


        public RelayCommand CmdReset
            => new(Reset);

        public RelayCommand CmdNext
            => new(Next);


        public VmSimulator(DialogContainer data, DialogOptions options)
        {
            SimulatorTracker = new();
            Data = data;
            Options = options;

            SharedDisabledIndices = new();
            OutputNumbers = new();
            ConditionData = new(new());

            Node? entryNode = data.StartNode;
            if (entryNode == null)
            {
                throw new InvalidOperationException("Dialog has no valid entry point!");
            }

            ObservableCollection<VmNode> nodes = new();
            Dictionary<Node, VmNode> nodeViewmodelLUT = new();

            foreach (Node node in data.Nodes)
            {
                VmNode vmNode = new(this, node);
                nodes.Add(vmNode);
                nodeViewmodelLUT.Add(node, vmNode);
            }

            Nodes = new(nodes);
            NodeViewmodelLUT = new(nodeViewmodelLUT);
            EntryNode = NodeViewmodelLUT[entryNode];
            _activeNode = EntryNode;

            foreach (VmNode vmNode in nodes)
            {
                foreach (VmNodeOutput vmOutput in vmNode.Outputs)
                {
                    if (vmOutput.Data.Connected != null)
                    {
                        vmOutput.Connected = NodeViewmodelLUT[vmOutput.Data.Connected];
                    }
                }
            }

            EntryNode.InitActive();
        }

        private void SetActiveNode(VmNode node)
        {

            VmNodeOutput? previousLeft = LeftPortrait;
            VmNodeOutput? previousRight = RightPortrait;
            bool previousSide = ActiveNode.Data.RightPortrait;

            BeginChangeGroup();

            TrackValueChange(
                (v) => _activeNode = v, _activeNode, node);
            TrackNotifyProperty(nameof(ActiveNode));

            PostChangeGroupAction(InitActiveNode);

            TrackNotifyProperty(nameof(HasNextNode));
            RightPortraitActive = node.Data.RightPortrait;

            if (previousSide != ActiveNode.Data.RightPortrait)
            {
                if (ActiveNode.Data.RightPortrait)
                {
                    TrackChange(
                        () => { },
                        () => RightPortrait = previousRight);
                }
                else
                {
                    TrackChange(
                        () => { },
                        () => LeftPortrait = previousLeft);
                }
            }

            EndChangeGroup();
        }

        public void RefreshHasNextNode()
        {
            OnPropertyChanged(nameof(HasNextNode));
        }

        private void InitActiveNode()
        {
            ActiveNode.InitActive();
        }

        public void Next()
        {
            VmNode? nextNode = ActiveNode.ActiveOutput.Connected;
            if (nextNode == null)
            {
                return;
            }

            BeginChangeGroup();

            VmNodeOutput output = ActiveNode.ActiveOutput;
            if (output.Data.DisableReuse)
            {
                output.Enabled = false;
            }

            SetActiveNode(nextNode);

            EndChangeGroup();
        }

        public void Jump(VmNode node)
        {
            if (ActiveNode == node)
            {
                return;
            }

            SetActiveNode(node);
        }

        private void Reset()
        {
            BeginChangeGroup();

            foreach (VmNode vmNode in Nodes)
            {
                foreach (VmNodeOutput vmOutput in vmNode.Outputs)
                {
                    vmOutput.Enabled = true;
                }
            }

            SharedDisabledIndices.Clear();

            SetActiveNode(EntryNode);

            if (ActiveNode.Data.RightPortrait)
            {
                TrackValueChange((v) => LeftPortrait = v, LeftPortrait, null);
            }
            else
            {
                TrackValueChange((v) => RightPortrait = v, RightPortrait, null);
            }

            EndChangeGroup();
        }
    }
}
