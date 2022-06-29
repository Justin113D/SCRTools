using SCR.Tools.Common;
using System.Collections.Generic;

namespace SCR.Tools.Dialog.Data.Condition.ReadOnly
{
    public class ReadOnlyTeamSlot : IReadOnlyTeamSlot
    {
        internal readonly HashSet<int> _equipment;

        public int Level { get; internal set; }

        public int Health { get; internal set; }

        public int MaxHealth { get; internal set; }

        public int PowerPoints { get; internal set; }

        public int MaxPowerPoints { get; internal set; }

        public ReadOnlySet<int> Equipment { get; }

        IReadOnlySet<int> IReadOnlyTeamSlot.Equipment 
            => Equipment;

        internal ReadOnlyTeamSlot()
        {
            _equipment = new();
            Equipment = new(_equipment);
        }

        public ReadOnlyTeamSlot(IReadOnlyTeamSlot slot)
        {
            _equipment = new(slot.Equipment);
            Equipment = new(_equipment);

            Level = slot.Level;
            Health = slot.Health;
            MaxHealth = slot.MaxHealth;
            PowerPoints = slot.PowerPoints;
            MaxPowerPoints = slot.MaxPowerPoints;
        }
    }
}
