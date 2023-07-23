using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SCR.Tools.Dialog.Data.Simulation
{
    public class SimulatorNode
    {
        private readonly SimulatorNodeOutput[] _outputs;

        public ReadOnlyCollection<SimulatorNodeOutput> Outputs { get; }

        internal SimulatorNode(int outputNum)
        {
            _outputs = new SimulatorNodeOutput[outputNum];
            Outputs = new(_outputs);
        }

        internal void FromNode(Node node, Dictionary<Node, SimulatorNode> nodes, DialogSettings settings)
        {
            for(int i = 0; i < _outputs.Length; i++)
            {
                _outputs[i] = SimulatorNodeOutput.FromNodeOutput(node.Outputs[i], nodes, settings);
            }
        }

        public SimulatorNodeOutput[] GetAvailableOutputs(SimulatorConditionData data)
        {
            List<SimulatorNodeOutput> result = new();

            foreach (SimulatorNodeOutput output in Outputs)
            {
                if (!output.Visited && !output.IsFallback && output.EvaluateCondition(data))
                {
                    result.Add(output);
                    if (result.Count == 4)
                    {
                        break;
                    }
                }
            }

            if (result.Count == 0)
            {
                foreach (SimulatorNodeOutput output in Outputs)
                {
                    if (output.IsFallback && output.EvaluateCondition(data))
                    {
                        result.Add(output);
                        break;
                    }
                }
            }

            if (result.Count == 0)
            {
                result.Add(Outputs[^1]);
            }



            return result.ToArray();
        }
    }
}
