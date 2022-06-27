using SCR.Tools.Dialog.Editor.Viewmodeling;
using System.Drawing;

namespace SCR.Tools.Dialog.Editor.WPF
{
    internal static class DesignDataFactory
    {
        private const string TestDialog = "{\"Nodes\": [{\"LocationX\": 1, \"LocationY\": 2, \"Outputs\": [{\"Character\": \"Chara1\", \"Expression\": \"Happy\", \"Text\": \"This is a test output\"}]}]}";
        private const string TestDialogOptions = "{\"CharacterOptions\": {\"Chara1\":\"FFFF0000\",\"Chara2\":\"FF00FF00\"},\"ExpressionOptions\": {\"Happy\":\"FF00FF00\",\"Sad\":\"FF0000FF\"}}";

        static DesignDataFactory()
        {
            Main = new VmMain();
            Main.LoadDialog(TestDialog);
            Dialog = Main.Dialog;
            Node = Dialog.Nodes[0];
            Output = Node.Outputs[0];
            Output.IsExpanded = true;
            Options = Main.DialogOptions;
            Options.Read(TestDialogOptions, null);
            NodeOptions = Options.CharacterOptions;
            NodeIcons = Options.NodeIcons;
        }

        public static VmMain Main { get; }

        public static VmDialog Dialog { get; }

        public static VmNode Node { get; }

        public static VmNodeOutput Output { get; }

        public static VmDialogOptions Options { get; }

        public static VmNodeOptions<Color> NodeOptions { get; }

        public static VmNodeOptions<string> NodeIcons { get; }
    }
}
