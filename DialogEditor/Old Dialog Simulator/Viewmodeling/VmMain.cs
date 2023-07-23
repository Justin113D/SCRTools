using SCR.Tools.Dialog.Data;
using SCR.Tools.Dialog.Simulator.Viewmodeling.Condition;
using SCR.Tools.UndoRedo;
using SCR.Tools.WPF.Viewmodeling;

namespace SCR.Tools.Dialog.Simulator.Viewmodeling
{
    internal class VmMain : BaseViewModel
    {
        public ChangeTracker SimulatorTracker { get; }

        public DialogSettings Options { get; }

        public VmSimulator Simulator { get; }

        public VmConditionData ConditionData { get; }

        public VmMain(DialogContainer data, DialogSettings options)
        {
            SimulatorTracker = new();
            Options = options;
            ConditionData = new(new());
            Simulator = new(this, data);
        }
    }
}
