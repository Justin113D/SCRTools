using SCR.Tools.DialogEditor.Data;
using SCR.Tools.UndoRedo.Collections;
using SCR.Tools.Viewmodeling;
using System.Collections.ObjectModel;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;

namespace SCR.Tools.DialogEditor.Viewmodeling
{
    public class VmNode : BaseViewModel
    {


        /// <summary>
        /// Dialog that the node belongs to
        /// </summary>
        public VmDialog Dialog { get; }

        /// <summary>
        /// Data object
        /// </summary>
        public Node Data { get; }


        private readonly TrackList<VmNodeOutput> _outputs;

        /// <summary>
        /// Viewmodel objects for the outputs
        /// </summary>
        public ReadOnlyObservableCollection<VmNodeOutput> Outputs { get; }

        private readonly TrackList<VmNodeOutput> _inputs;

        /// <summary>
        /// Outputs attached to this node
        /// </summary>
        public ReadOnlyObservableCollection<VmNodeOutput> Inputs { get; }


        public string Name
            => _outputs[0].Name;

        public string InOutInfo
            => $"[ {_inputs.Count} ; {_outputs.Count} ]";

        public bool RightPortrait
        {
            get => Data.RightPortrait;
            set
            {
                if(value == RightPortrait)
                {
                    return;
                }

                BeginChangeGroup();

                Data.RightPortrait = value;
                TrackNotifyProperty(nameof(RightPortrait));

                EndChangeGroup();
            }
        }


        public RelayCommand CmdAddOutput
            => new(AddOutput);


        public VmNode(VmDialog dialog, Node data)
        {
            Dialog = dialog;
            Data = data;

            ObservableCollection<VmNodeOutput> internalInputs = new();
            _inputs = new(internalInputs);
            Inputs = new(internalInputs);

            ObservableCollection<VmNodeOutput> internalOutputs = new();
            foreach (NodeOutput output in data.Outputs)
            {
                VmNodeOutput VmOutput = new(this, output);
                internalOutputs.Add(VmOutput);
            }
            _outputs = new(internalOutputs);
            Outputs = new(internalOutputs);
        }
    

        private void AddOutput()
        {
            BeginChangeGroup();

            NodeOutput output = Data.CreateOutput();
            VmNodeOutput vmOutput = new(this, output);
            _outputs.Add(vmOutput);
            TrackNotifyProperty(nameof(InOutInfo));

            EndChangeGroup();
        }

        public void DeleteOutput(VmNodeOutput vmOutput)
        {
            if (_outputs.Count < 2)
                return;

            BeginChangeGroup();

            vmOutput.Disconnect();
            _outputs.Remove(vmOutput);

            Data.RemoveOutput(vmOutput.Data);
            TrackNotifyProperty(nameof(InOutInfo));

            EndChangeGroup();
        }


        public void AddInput(VmNodeOutput vmInput)
        {
            BeginChangeGroup();

            _inputs.Add(vmInput);
            TrackNotifyProperty(nameof(InOutInfo));

            EndChangeGroup();
        }

        public void RemoveInput(VmNodeOutput vmInput)
        {
            BeginChangeGroup();

            _inputs.Remove(vmInput);
            TrackNotifyProperty(nameof(InOutInfo));

            EndChangeGroup();
        }
    }
}
