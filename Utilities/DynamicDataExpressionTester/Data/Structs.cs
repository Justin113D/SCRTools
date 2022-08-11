using System;

namespace SCR.Tools.DynamicDataExpressionTester.Data
{
    [Serializable]
    public struct ChaoSlot
    {
        public int Count { get; set; }
        public int Level { get; set; }


        public ChaoSlot(int count, int level)
        {
            Count = count;
            Level = level;
        }
    }

    [Serializable]
    public struct TeamSlot
    {
        public int Level { get; set; }

        public int Health { get; set; }

        public int MaxHealth { get; set; }

        public int[] Equipment { get; set; }

        public TeamSlot(int level, int maxhealth, int health, int[] equipment)
        {
            Level = level;
            Health = health;
            MaxHealth = maxhealth;
            Equipment = equipment;
        }
    }
}
