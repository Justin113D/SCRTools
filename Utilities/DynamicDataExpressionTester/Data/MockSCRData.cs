using System;
using System.Collections.Generic;

namespace SCR.Tools.DynamicDataExpressionTester.Data
{
    /// <summary>
    /// Mockup SCR data for testing stuff
    /// </summary>
    [Serializable]
    public partial class MockSCRData
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
