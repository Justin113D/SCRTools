using SCR.Tools.Dialog.Data.Condition;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;

namespace SCR.Tools.Dialog.Data.Simulation
{
    public class DialogSimulator
    {
        private readonly SimulatorNode _entryNode;

        public ReadOnlyCollection<SimulatorNode> Nodes { get; }

        public SimulatorConditionData ConditionData { get; }

        public SimulatorNode ActiveNode { get; private set; }

        public SimulatorNodeOutput[] ActiveOutputs { get; private set; }

        public bool EndReached { get; private set; }


        private DialogSimulator(SimulatorNode[] nodes, SimulatorNode entryNode, ConditionData data)
        {
            Nodes = new(nodes);
            ConditionData = new(data);
            _entryNode = entryNode;
            ActiveNode = entryNode;
            ActiveOutputs = ActiveNode.GetAvailableOutputs(ConditionData);
        }

        public static DialogSimulator FromDialog(DialogContainer dialog, DialogSettings settings, ConditionData? conditionData)
        {
            if(dialog.EntryNode == null)
            {
                throw new InvalidOperationException("No Entry node!");
            }

            SimulatorNode[] nodes = new SimulatorNode[dialog.Nodes.Count];
            Dictionary<Node, SimulatorNode> nodeLUT = new();

            for(int i = 0; i < dialog.Nodes.Count; i++)
            {
                Node node = dialog.Nodes[i];
                SimulatorNode simNode = new(node.RightPortrait, node.Outputs.Count);

                nodes[i] = simNode;
                nodeLUT.Add(node, simNode);
            }

            for (int i = 0; i < dialog.Nodes.Count; i++)
            {
                nodes[i].FromNodeOutputs(dialog.Nodes[i], nodeLUT, settings);
            }

            return new(nodes, nodeLUT[dialog.EntryNode], conditionData ?? new());
        }


        private void SetEndReached(bool value)
            => TrackValueChange((v) => EndReached = v, EndReached, value);

        private void SetActiveNode(SimulatorNode node)
        {
            BeginChangeGroup();
            TrackValueChange((v) => ActiveNode = v, ActiveNode, node);
            TrackValueChange((v) => ActiveOutputs = v, ActiveOutputs, ActiveNode.GetAvailableOutputs(ConditionData));
            EndChangeGroup();
        }

        public void Next(int pathIndex)
        {
            if (EndReached)
                return;

            SimulatorNodeOutput output = ActiveOutputs[pathIndex];

            BeginChangeGroup();

            output.ExecuteInstructions(ConditionData);

            if(output.NextNode != null)
            {
                SetActiveNode(output.NextNode);
            }
            else
            {
                SetEndReached(true);
            }

            EndChangeGroup();
        }

        public void Reset()
        {
            BeginChangeGroup();

            foreach (SimulatorNode node in Nodes)
            {
                foreach (SimulatorNodeOutput output in node.Outputs)
                {
                    output.Visited = false;
                }
            }

            ConditionData.Reset();
            SetEndReached(false);
            SetActiveNode(_entryNode);

            EndChangeGroup();
        }
    }
}
