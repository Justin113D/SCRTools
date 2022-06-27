using SCR.Tools.Dialog.Data;
using SCR.Tools.Viewmodeling;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace SCR.Tools.Dialog.Simulator.Viewmodeling
{
    public class VmSimulatorNode : BaseViewModel
    {
        private VmSimulatorOutput _activeOutput;

        private int _cameBy;

        public VmSimulator Simulator { get; }

        public Node Data { get; }

        public ReadOnlyObservableCollection<VmSimulatorOutput> Outputs { get; }

        public VmSimulatorOutput[] ValidOutputs { get; private set; }

        public bool DisplayOutputs
            => ValidOutputs.Length > 1;

        public string Name
            => Outputs[0].Name;

        public string OutInfo
            => Outputs.Count > 1 ? $"[ {Outputs.Count} ]" : "";

        public int CameBy
        {
            get => _cameBy;
            set
            {
                if(value == _cameBy)
                {
                    return;
                }

                BeginChangeGroup();

                TrackValueChange(
                    (v) => _cameBy = v, _cameBy, value);
                TrackNotifyProperty(nameof(CameBy));

                EndChangeGroup();
            }
        }

        public int OutputNumber
        {
            get
            {
                if(Simulator.OutputNumbers.TryGetValue(this, out int result))
                {
                    return result;
                }

                return 0;
            }
        }

        public VmSimulatorOutput ActiveOutput
        {
            get => _activeOutput;
            set
            {
                _activeOutput = value;

                if(Data.RightPortrait)
                {
                    Simulator.RightPortrait = _activeOutput;
                }
                else
                {
                    Simulator.LeftPortrait = _activeOutput;
                }

                Simulator.RefreshHasNextNode();
            }
        }

        public VmSimulatorNode(VmSimulator simulator, Node data)
        {
            Simulator = simulator;
            Data = data;

            ObservableCollection<VmSimulatorOutput> outputs = new();

            foreach(NodeOutput output in Data.Outputs)
            {
                outputs.Add(new(this, output));
            }

            Outputs = new(outputs);
            ValidOutputs = GetValidOutputs();
            _activeOutput = outputs[0];
        }

        private VmSimulatorOutput[] GetValidOutputs()
        {
            List<VmSimulatorOutput> result = new();

            foreach (VmSimulatorOutput output in Outputs)
            {
                if (output.ConditionValid && output.Enabled  && !output.Data.Fallback)
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
                foreach (VmSimulatorOutput output in Outputs)
                {
                    if (output.ConditionValid && output.Enabled && output.Data.Fallback)
                    {
                        result.Add(output);
                        break;
                    }
                }
            }

            return result.ToArray();
        }

        public void InitActive()
        {
            ValidOutputs = GetValidOutputs();
            OnPropertyChanged(nameof(DisplayOutputs));

            VmSimulatorNode[] outputNumNodes = Simulator.OutputNumbers.Keys.ToArray();
            Simulator.OutputNumbers.Clear();
            foreach (VmSimulatorNode outputNumNode in outputNumNodes)
            {
                outputNumNode.OnPropertyChanged(nameof(OutputNumber));
            }

            int i = 1;
            foreach(VmSimulatorOutput output in ValidOutputs)
            {
                if(output.Connected != null)
                {
                    Simulator.OutputNumbers.Add(output.Connected, i);
                    output.Connected.OnPropertyChanged(nameof(OutputNumber));
                    i++;
                }
            }

            ActiveOutput = ValidOutputs[0];
        }
    }
}
