using SCR.Tools.Dialog.Data.Condition.ReadOnly;
using SCR.Tools.UndoRedo.Collections;
using System.Collections.Generic;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;
using System.Linq;
using System;

namespace SCR.Tools.Dialog.Data.Condition
{
    public class TeamSlot : IReadOnlyTeamSlot, ICloneable
    {
        private int _level;
        private int _health;
        private int _maxHealth;
        private int _powerPoints;
        private int _maxPowerPoints;


        public int Level
        {
            get => _level;
            set => BlankValueChange((v) => _level = v, _level, value);
        }

        public int Health
        {
            get => _health;
            set => BlankValueChange((v) => _health = v, _health, value);
        }

        public int MaxHealth
        {
            get => _maxHealth;
            set => BlankValueChange((v) => _maxHealth = v, _maxHealth, value);
        }

        public int PowerPoints
        {
            get => _powerPoints;
            set => BlankValueChange((v) => _powerPoints = v, _powerPoints, value);
        }

        public int MaxPowerPoints
        {
            get => _maxPowerPoints;
            set => BlankValueChange((v) => _maxPowerPoints = v, _maxPowerPoints, value);
        }

        public TrackSet<int> Equipment { get; }

        IReadOnlySet<int> IReadOnlyTeamSlot.Equipment
            => Equipment;

        public TeamSlot()
        {
            Equipment = new();
        }

        public TeamSlot(IReadOnlyTeamSlot slot)
        {
            HashSet<int> internalEquipment = new(slot.Equipment);
            Equipment = new(internalEquipment);

            _level = slot.Level;
            _health = slot.Health;
            _maxHealth = slot.MaxHealth;
            _powerPoints = slot.PowerPoints;
            _maxHealth = slot.MaxPowerPoints;
        }

        public object Clone()
            => new TeamSlot(this);
    }
}
