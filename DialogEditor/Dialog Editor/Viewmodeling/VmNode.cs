using SCR.Tools.DialogEditor.Data;
using SCR.Tools.Viewmodeling;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private readonly ObservableCollection<VmNodeOutput> _outputs;

        /// <summary>
        /// Viewmodel objects for the outputs
        /// </summary>
        public ReadOnlyObservableCollection<VmNodeOutput> Outputs { get; }

        private readonly ObservableCollection<VmNodeOutput> _inputs;

        /// <summary>
        /// Outputs attached to this node
        /// </summary>
        public ReadOnlyObservableCollection<VmNodeOutput> Inputs { get; }

        public string Name
            => _outputs[0].Name;

        public string InOutInfo
            => $"[ {_inputs.Count} ; {_outputs.Count} ]";

        public VmNode(VmDialog dialog, Node data)
        {
            Dialog = dialog;
            Data = data;

            _inputs = new();
            Inputs = new(_inputs);


            _outputs = new();
            foreach (NodeOutput output in data.Outputs)
            {
                VmNodeOutput VmOutput = new(this, output);
                _outputs.Add(VmOutput);
            }
            Outputs = new(_outputs);
        }
    }
}
