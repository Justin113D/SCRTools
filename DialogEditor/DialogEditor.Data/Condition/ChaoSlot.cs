using System;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;

namespace SCR.Tools.Dialog.Data.Condition
{
    public class ChaoSlot : IReadOnlyChaoSlot, ICloneable
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

        public ChaoSlot(IReadOnlyChaoSlot slot)
        {
            _count = slot.Count;
            _level = slot.Level;
        }

        public object Clone()
            => new ChaoSlot(this);
    }
}
