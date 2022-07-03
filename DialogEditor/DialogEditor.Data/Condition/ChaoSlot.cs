using System;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;

namespace SCR.Tools.Dialog.Data.Condition
{
    public class ChaoSlot : ICloneable
    {
        private int _count;
        private int _level;

        public int Count
        {
            get => _count;
            set => BlankValueChange((v) => _count = v, _count, value);
        }

        public int Level
        {
            get => _level;
            set => BlankValueChange((v) => _level = v, _level, value);
        }

        public ChaoSlot()
        {

        }

        private ChaoSlot(ChaoSlot slot)
        {
            _count = slot.Count;
            _level = slot.Level;
        }

        public ChaoSlot Clone()
            => new(this);


        object ICloneable.Clone()
            => Clone();
    }
}
