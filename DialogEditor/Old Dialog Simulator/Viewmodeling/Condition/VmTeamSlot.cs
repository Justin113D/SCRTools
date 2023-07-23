using SCR.Tools.Dialog.Data.Condition;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;

namespace SCR.Tools.Dialog.Simulator.Viewmodeling.Condition
{
    internal class VmTeamSlot : VmSlotDictionaryItem<TeamSlot, VmTeamSlot>
    {
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

        public int Health
        {
            get => Slot.Health;
            set
            {
                BeginChangeGroup();
                Slot.Health = value;
                TrackNotifyProperty(nameof(Health));
                EndChangeGroup();
            }
        }

        public int MaxHealth
        {
            get => Slot.MaxHealth;
            set
            {
                BeginChangeGroup();
                Slot.MaxHealth = value;
                TrackNotifyProperty(nameof(MaxHealth));
                EndChangeGroup();
            }
        }

        public int PowerPoints
        {
            get => Slot.PowerPoints;
            set
            {
                BeginChangeGroup();
                Slot.PowerPoints = value;
                TrackNotifyProperty(nameof(PowerPoints));
                EndChangeGroup();
            }
        }

        public int MaxPowerPoints
        {
            get => Slot.MaxPowerPoints;
            set
            {
                BeginChangeGroup();
                Slot.MaxPowerPoints = value;
                TrackNotifyProperty(nameof(MaxPowerPoints));
                EndChangeGroup();
            }
        }

        public VmIntSet Equipment { get; }

        public bool Expanded { get; set; }

        public VmTeamSlot(VmSlotDictionary<TeamSlot, VmTeamSlot> parent, int index) : base(parent, index)
        {
            Equipment = new(Slot.Equipment);
        }
    }
}
