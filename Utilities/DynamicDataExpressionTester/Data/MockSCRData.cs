using System;
using System.Collections.Generic;

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

    /// <summary>
    /// Mockup SCR data for testing stuff
    /// </summary>
    [Serializable]
    public class MockSCRData
    {
        public Dictionary<int, bool> Flags { get; set; }

        public int Rings { get; set; }

        public Dictionary<int, int> Items { get; set; }

        public Dictionary<int, ChaoSlot> Chao { get; set; }

        public HashSet<int> Cards { get; set; }

        /// <summary>
        /// id => level
        /// </summary>
        public Dictionary<int, TeamSlot> TeamMembers { get; set; }

        public List<int> PartyMembers { get; set; }

    }
}
