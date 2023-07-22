using SCR.Tools.Dialog.Data;
using SCR.Tools.Dialog.Data.Condition;
using SCR.Tools.Dialog.Simulator.Viewmodeling;
using SCR.Tools.Dialog.Simulator.Viewmodeling.Condition;
using JsonFormatHandler = SCR.Tools.Dialog.Data.JsonFormatHandler;

namespace SCR.Tools.Dialog.Simulator.WPF
{
    internal static class DesignDataFactory
    {
        private const string TestDialog = "{\"Nodes\":[{\"LocationX\":1,\"LocationY\":2,\"Outputs\":[{\"Character\":\"Chara1\",\"Expression\":\"Happy\",\"Text\":\"Thisisatestoutput\"}]}],\"ConditionData\":{\"Test\":{\"Flags\":{\"0\":true,\"1\":false},\"DynamicFlags\":{\"0\":true,\"1\":false},\"Rings\":50,\"Items\":{\"0\":0,\"1\":1,\"2\":2},\"Chao\":{\"0\":{\"Count\":1,\"Level\":0}},\"Cards\":[0,1,2],\"TeamMembers\":{\"0\":{\"Level\":1,\"Health\":20,\"MaxHealth\":20,\"PowerPoints\":7,\"MaxPowerPoints\":7,\"Equipment\":[0,1]}},\"Party\":[0,1]}}}";
        private const string TestDialogOptions = "{\"CharacterOptions\": {\"Chara1\":\"FFFF0000\",\"Chara2\":\"FF00FF00\"},\"ExpressionOptions\": {\"Happy\":\"FF00FF00\",\"Sad\":\"FF0000FF\"}}";

        static DesignDataFactory()
        {
            DialogContainer dialog = JsonFormatHandler.ReadDialog(TestDialog);
            DialogOptions options = JsonFormatHandler.ReadDialogOptions(TestDialogOptions);

            Main = new(dialog, options);
            Simulator = Main.Simulator;
            ConditionData = Main.ConditionData;

            Flags = ConditionData.Flags;
            Items = ConditionData.Items;
            TeamMembers = ConditionData.TeamMembers;
            Cards = ConditionData.Cards;
            PartyMembers = ConditionData.PartyMembers;
        }

        public static VmMain Main { get; }

        public static VmSimulator Simulator { get; }

        public static VmConditionData ConditionData { get; }

        public static VmValueDictionary<bool> Flags { get; }

        public static VmValueDictionary<int> Items { get; }

        public static VmSlotDictionary<TeamSlot, VmTeamSlot> TeamMembers { get; }

        public static VmIntSet Cards { get; }

        public static VmIntList PartyMembers { get; }
    }
}
