using SCR.Tools.Dialog.Data.Condition;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;

namespace SCR.Tools.Dialog.Simulator.Viewmodeling.Condition
{
    internal class VmChaoSlot : VmlotDictionaryItem<ChaoSlot, VmChaoSlot>
    {
        public int Count
        {
            get => Slot.Count;
            set
            {
                BeginChangeGroup();
                Slot.Count = value;
                TrackNotifyProperty(nameof(Count));
                EndChangeGroup();
            }
        }

        public int Level
        {
            get => Slot.Level;
            set
            {
                BeginChangeGroup();
                Slot.Level = value;
                TrackNotifyProperty(nameof(Level));
                EndChangeGroup();
            }
        }

        public bool Expanded { get; set; }

        public VmChaoSlot(VmSlotDictionary<ChaoSlot, VmChaoSlot> parent, int index) : base(parent, index)
        {
            
        }
    }
}
