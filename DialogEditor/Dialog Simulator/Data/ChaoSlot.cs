using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;

namespace SCR.Tools.Dialog.Simulator.Data
{
    public class ChaoSlot
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

        public ChaoSlot(int count, int level)
        {
            _count = count;
            _level = level;
        }
    }
}
