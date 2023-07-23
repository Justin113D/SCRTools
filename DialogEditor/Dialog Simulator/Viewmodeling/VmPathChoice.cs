using SCR.Tools.WPF.Viewmodeling;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;

namespace SCR.Tools.Dialog.Simulator.Viewmodeling
{
    internal class VmPathChoice : BaseViewModel
    {
        public int Index { get; }

        public bool Active { get; private set; }

        public string? IconPath { get; private set; }

        public VmPathChoice(int index, bool active, string? iconPath)
        {
            Index = index;
            Active = active;
            IconPath = iconPath;
        }

        public void Update(bool active, string? iconPath)
        {
            BeginChangeGroup();
            TrackValueChange((v) => Active = v, Active, active);
            TrackValueChange((v) => IconPath = v, IconPath, iconPath);
            EndChangeGroup();
        }
    }
}
