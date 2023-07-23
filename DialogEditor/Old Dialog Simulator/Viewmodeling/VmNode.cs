using SCR.Tools.Dialog.Data;
using SCR.Tools.WPF.Viewmodeling;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;

namespace SCR.Tools.Dialog.Simulator.Viewmodeling
{
    internal class VmNode : BaseViewModel
    {
        private VmNodeOutput _activeOutput;

        private int _cameBy;

        public VmSimulator Simulator { get; }

        public Node Data { get; }

        public ReadOnlyObservableCollection<VmNodeOutput> Outputs { get; }

        public VmNodeOutput[] ValidOutputs { get; private set; }

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
                if (value == _cameBy)
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
                if (Simulator.OutputNumbers.TryGetValue(this, out int result))
                {
                    return result;
                }

                return 0;
            }
        }

        public VmNodeOutput ActiveOutput
        {
            get => _activeOutput;
            set
            {
                _activeOutput = value;

                if (Data.RightPortrait)
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

        public VmNode(VmSimulator simulator, Node data)
        {
            Simulator = simulator;
            Data = data;

            ObservableCollection<VmNodeOutput> outputs = new();

            foreach (NodeOutput output in Data.Outputs)
            {
                outputs.Add(new(this, output));
            }

            Outputs = new(outputs);
            ValidOutputs = GetValidOutputs();
            _activeOutput = outputs[0];
        }

        private VmNodeOutput[] GetValidOutputs()
        {
            List<VmNodeOutput> result = new();

            foreach (VmNodeOutput output in Outputs)
            {
                if (output.ConditionValid && output.Enabled && !output.Data.IsFallback)
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
                foreach (VmNodeOutput output in Outputs)
                {
                    if (output.ConditionValid && output.Enabled && output.Data.IsFallback)
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
            VmNodeOutput[] outputs = GetValidOutputs();
            ActiveOutput = outputs[0];
            ValidOutputs = outputs;
            OnPropertyChanged(nameof(DisplayOutputs));

            VmNode[] outputNumNodes = Simulator.OutputNumbers.Keys.ToArray();
            Simulator.OutputNumbers.Clear();
            foreach (VmNode outputNumNode in outputNumNodes)
            {
                outputNumNode.OnPropertyChanged(nameof(OutputNumber));
            }

            int i = 1;
            foreach (VmNodeOutput output in ValidOutputs)
            {
                if (output.Connected != null)
                {
                    Simulator.OutputNumbers.Add(output.Connected, i);
                    output.Connected.OnPropertyChanged(nameof(OutputNumber));
                    i++;
                }
            }

        }
    }
}
