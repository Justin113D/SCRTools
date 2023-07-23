using SCR.Tools.DynamicDataExpression;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;
using System.Collections.Generic;
using System.IO;

namespace SCR.Tools.Dialog.Data.Simulation
{
    public class SimulatorNodeOutput
    {
        private bool _visited;
        private readonly DataExpression? _DDXCondition;
        private readonly DataInstruction[] _DDXInstructions;

        public bool Visited
        {
            get => _visited;
            set
            {
                if (value == Visited)
                    return;

                TrackValueChange((v) => _visited = v, _visited, value);
            }
        }

        public bool IsFallback { get; }

        public string Label { get; }

        public string Text { get; }

        public string? PortraitPath { get; }

        public string? IconPath { get; }

        public SimulatorNode? NextNode { get; }


        private SimulatorNodeOutput(bool isFallback, string label, string text, string? portraitPath, string? iconPath, SimulatorNode? nextNode, DataExpression? condition, DataInstruction[] instructions)
        {
            IsFallback = isFallback;
            Label = label;
            Text = text;
            PortraitPath = portraitPath;
            IconPath = iconPath;
            NextNode = nextNode;
            _DDXCondition = condition;
            _DDXInstructions = instructions;
        }

        internal static SimulatorNodeOutput FromNodeOutput(NodeOutput output, Dictionary<Node, SimulatorNode> nodes, DialogSettings settings)
        {
            string? portraitPath = null;
            if(!string.IsNullOrWhiteSpace(output.Character) 
                && !string.IsNullOrWhiteSpace(output.Expression)
                && !string.IsNullOrWhiteSpace(settings.PortraitsPath))
            {
                portraitPath = Path.Combine(settings.PortraitsPath, $"{settings.PortraitsPath}_{output.Expression}.png");
                if(!File.Exists(portraitPath))
                {
                    portraitPath = null;
                }
            }

            string? iconPath = null;
            if (!string.IsNullOrWhiteSpace(output.Icon))
            {
                settings.NodeIcons.TryGetValue(output.Icon, out iconPath);
            }

            SimulatorNode? nextNode = output.Connected == null ? null : nodes[output.Connected];

            DataExpression? ddxCondition = null;
            if (!string.IsNullOrWhiteSpace(output.Condition))
            {
                try
                {
                    ddxCondition = DataExpression.ParseExpression(output.Condition, SimulatorConditionData.AccessKeys, KeyType.Boolean);
                }
                catch (DynamicDataExpressionException e)
                {
                    throw new SimulatorException(e.Message, output.Parent, output);
                }
            }

            DataInstruction[] instructions = new DataInstruction[output.Instructions.Count];
            for(int i = 0; i < instructions.Length; i++)
            {
                try
                {
                    instructions[i] = DataInstruction.ParseInstruction(output.Condition, SimulatorConditionData.SetterKeys, SimulatorConditionData.AccessKeys);
                }
                catch (DynamicDataExpressionException e)
                {
                    throw new SimulatorException(e.Message, output.Parent, output);
                }
            }

            return new SimulatorNodeOutput(
                output.IsFallback,
                $"{output.Expression ?? "[None]"} {output.Character ?? "[None]"}",
                output.Text,
                portraitPath,
                iconPath,
                nextNode,
                ddxCondition,
                instructions
                );
        }


        public bool EvaluateCondition(SimulatorConditionData data)
            => _DDXCondition?.EvaluateBoolean(data) ?? true;

        public void ExecuteInstructions(SimulatorConditionData data)
        {
            for(int i = 0; i < _DDXInstructions.Length; i++)
            {
                _DDXInstructions[i].Execute(data);
            }
        }

        public override string ToString()
            => Label;
    }
}
