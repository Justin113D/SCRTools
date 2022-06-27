using SCR.Tools.Dialog.Data;
using SCR.Tools.Dialog.Simulator.Data;
using SCR.Tools.UndoRedo;
using SCR.Tools.UndoRedo.Collections;
using SCR.Tools.Viewmodeling;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;

namespace SCR.Tools.Dialog.Simulator.Viewmodeling
{
    public class VmSimulator : BaseViewModel
    {
        private VmSimulatorNode _activeNode;
        private bool _rightPortraitActive;


        public ChangeTracker SimulatorTracker { get; }

        public DialogContainer Data { get; }

        public DialogOptions Options { get; }

        public VmSimulatorNode EntryNode { get; }

        public ReadOnlyObservableCollection<VmSimulatorNode> Nodes { get; }

        public ReadOnlyDictionary<Node, VmSimulatorNode> NodeViewmodelLUT { get; }

        public TrackSet<int> SharedDisabledIndices { get; }

        public Dictionary<VmSimulatorNode, int> OutputNumbers { get; }

        public ConditionData ConditionData { get; }

        public VmSimulatorNode ActiveNode
        {
            get => _activeNode;
            set
            {
                if (_activeNode == value)
                {
                    return;
                }

                BeginChangeGroup();

                TrackValueChange(
                    (v) => _activeNode = v, _activeNode, value);
                TrackNotifyProperty(nameof(ActiveNode));

                PostChangeGroupAction(InitActiveNode);

                TrackNotifyProperty(nameof(HasNextNode));
                RightPortraitActive = value.Data.RightPortrait;

                EndChangeGroup();
            }
        }

        public VmSimulatorOutput? LeftPortrait { get; set; }

        public VmSimulatorOutput? RightPortrait { get; set; }

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
            ConditionData = new();

            Node? entryNode = data.StartNode;
            if (entryNode == null)
            {
                throw new InvalidOperationException("Dialog has no valid entry point!");
            }

            ObservableCollection<VmSimulatorNode> nodes = new();
            Dictionary<Node, VmSimulatorNode> nodeViewmodelLUT = new();

            foreach (Node node in data.Nodes)
            {
                VmSimulatorNode vmNode = new(this, node);
                nodes.Add(vmNode);
                nodeViewmodelLUT.Add(node, vmNode);
            }

            Nodes = new(nodes);
            NodeViewmodelLUT = new(nodeViewmodelLUT);
            EntryNode = NodeViewmodelLUT[entryNode];
            _activeNode = EntryNode;

            foreach (VmSimulatorNode vmNode in nodes)
            {
                foreach (VmSimulatorOutput vmOutput in vmNode.Outputs)
                {
                    if (vmOutput.Data.Connected != null)
                    {
                        vmOutput.Connected = NodeViewmodelLUT[vmOutput.Data.Connected];
                    }
                }
            }

            EntryNode.InitActive();
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
            VmSimulatorNode? nextNode = ActiveNode.ActiveOutput.Connected;
            if (nextNode == null)
            {
                return;
            }

            BeginChangeGroup();

            VmSimulatorOutput output = ActiveNode.ActiveOutput;
            if (output.Data.DisableReuse)
            {
                output.Enabled = false;
            }


            VmSimulatorOutput? previousLeft = LeftPortrait;
            VmSimulatorOutput? previousRight = RightPortrait;

            ActiveNode = nextNode;

            if (ActiveNode.Data.RightPortrait)
            {
                TrackChange(
                    () => { },
                    () =>
                    {
                        RightPortrait = previousRight;
                    });
            }
            else
            {
                TrackChange(
                    () => { },
                    () =>
                    {
                        LeftPortrait = previousLeft;
                    });
            }

            EndChangeGroup();
        }

        public void Jump(VmSimulatorNode node)
        {
            if (ActiveNode == node)
            {
                return;
            }

            BeginChangeGroup();

            VmSimulatorOutput? previousLeft = LeftPortrait;
            VmSimulatorOutput? previousRight = RightPortrait;

            ActiveNode = node;

            if (ActiveNode.Data.RightPortrait)
            {
                TrackChange(
                    () => { },
                    () =>
                    {
                        RightPortrait = previousRight;
                    });
            }
            else
            {
                TrackChange(
                    () => { },
                    () =>
                    {
                        LeftPortrait = previousLeft;
                    });
            }

            EndChangeGroup();
        }

        private void Reset()
        {
            foreach (VmSimulatorNode vmNode in Nodes)
            {
                foreach (VmSimulatorOutput vmOutput in vmNode.Outputs)
                {
                    vmOutput.Enabled = true;
                }
            }

            _activeNode = EntryNode;
            _activeNode.InitActive();

            ResetTracker();
        }
    }
}
