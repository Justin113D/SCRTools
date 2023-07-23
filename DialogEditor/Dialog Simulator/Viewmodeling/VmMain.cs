using SCR.Tools.Dialog.Data;
using SCR.Tools.WPF.Viewmodeling;
using SCR.Tools.UndoRedo;

namespace SCR.Tools.Dialog.Simulator.Viewmodeling
{
    internal class VmMain : BaseViewModel
    {
        public DialogContainer Dialog { get; }
        public DialogSettings Options { get; }
        public ChangeTracker Tracker { get; }

        public VmSimulator Simulator { get; }

        public VmMain(DialogContainer dialog, DialogSettings options)
        {
            Dialog = dialog;
            Options = options;
            Tracker = new();
            Simulator = new(this);
        }
    }
}
