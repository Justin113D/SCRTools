using SCR.Tools.DialogEditor.Data;
using SCR.Tools.Viewmodeling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCR.Tools.DialogEditor.Viewmodeling
{
    public class VmNodeOutput : BaseViewModel
    {
        /// <summary>
        /// Parent Node Viewmodel
        /// </summary>
        public VmNode Parent { get; }

        /// <summary>
        /// Data model
        /// </summary>
        public NodeOutput Data { get; }

        /// <summary>
        /// Name of the output
        /// </summary>
        public string Name
            => $"{Data.Expression ?? "[None]"} {Data.Character ?? "[None]"}";

        public VmNodeOutput(VmNode parent, NodeOutput data)
        {
            Parent = parent;
            Data = data;
        }
    }
}
