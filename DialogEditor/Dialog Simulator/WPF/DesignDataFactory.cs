using SCR.Tools.Dialog.Data;
using SCR.Tools.Dialog.Simulator.Viewmodeling;

namespace SCR.Tools.Dialog.Simulator.WPF
{
    internal static class DesignDataFactory
    {
        private const string TestDialog = "{\"Nodes\": [{\"LocationX\": 1, \"LocationY\": 2, \"Outputs\": [{\"Character\": \"Chara1\", \"Expression\": \"Happy\", \"Text\": \"This is a test output\"}]}]}";
        private const string TestDialogOptions = "{\"CharacterOptions\": {\"Chara1\":\"FFFF0000\",\"Chara2\":\"FF00FF00\"},\"ExpressionOptions\": {\"Happy\":\"FF00FF00\",\"Sad\":\"FF0000FF\"}}";

        static DesignDataFactory()
        {
            DialogContainer dialog = JsonFormatHandler.ReadDialog(TestDialog);
            DialogOptions options = JsonFormatHandler.ReadDialogOptions(TestDialogOptions);

            Simulator = new(dialog, options);
        }

        public static VmSimulator Simulator { get; }
    }
}
