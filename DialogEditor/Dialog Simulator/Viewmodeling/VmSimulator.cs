using SCR.Tools.Dialog.Data;
using SCR.Tools.DynamicDataExpression;
using SCR.Tools.UndoRedo.Collections;
using SCR.Tools.WPF.Viewmodeling;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;

namespace SCR.Tools.Dialog.Simulator.Viewmodeling
{
    internal struct ExecutedInstruction
    {
        public string Instruction { get; }

        public object OldValue { get; }

        public object NewValue { get; }

        public ExecutedInstruction(string instruction, object oldValue, object newValue)
        {
            Instruction = instruction;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }

    internal class VmSimulator : BaseViewModel
    {
        private VmNode _activeNode;
        private bool _rightPortraitActive;

        private readonly TrackList<ExecutedInstruction> _executedInstructions;

        public VmMain Main { get; }

        public DialogContainer Data { get; }


        public VmNode EntryNode { get; }

        public ReadOnlyObservableCollection<VmNode> Nodes { get; }

        public ReadOnlyDictionary<Node, VmNode> NodeViewmodelLUT { get; }

        /// <summary>
        /// Used to color code the current outputs in the node table
        /// </summary>
        public Dictionary<VmNode, int> OutputNumbers { get; }

        public ReadOnlyCollection<ExecutedInstruction> ExecutedInstructions { get; }


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

        public RelayCommand CmdNext
            => new(Next);

        public RelayCommand CmdReset
            => new(Reset);


        public VmSimulator(VmMain main, DialogContainer data)
        {
            Main = main;
            Data = data;

            OutputNumbers = new();

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

            ObservableCollection<ExecutedInstruction> internalEI = new();
            _executedInstructions = new(internalEI);
            ExecutedInstructions = new(internalEI);
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

            foreach(DataInstruction instruction in output.DDXInstructions)
            {
                object oldValue = instruction.GetValue(Main.ConditionData);
                instruction.Execute(Main.ConditionData);
                object newValue = instruction.GetValue(Main.ConditionData);
                _executedInstructions.Add(new(instruction.ToString(), oldValue, newValue));
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

        public void Reset()
        {
            BeginChangeGroup();

            foreach (VmNode vmNode in Nodes)
            {
                foreach (VmNodeOutput vmOutput in vmNode.Outputs)
                {
                    vmOutput.Enabled = true;
                }
            }

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
