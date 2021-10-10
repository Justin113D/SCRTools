using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SCRDynamicDataCheckTester.Data
{
    [Serializable]
    public struct ChaoSlot
    {
        public int count { get; set; }
        public int level { get; set; }


        public ChaoSlot(int count, int level)
        {
            this.count = count;
            this.level = level;
        }
    }

    [Serializable]
    public struct TeamSlot
    {
        public int level { get; set; }
        public int[] equipment { get; set; }

        public TeamSlot(int level, int[] equipment)
        {
            this.level = level;
            this.equipment = equipment;
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
