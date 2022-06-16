using SCR.Tools.DialogEditor.Viewmodeling;

namespace SCR.Tools.DialogEditor.WPF
{
    internal static class DesignDataFactory
    {
        private const string TestDialogOptions = "{\"CharacterOptions\": {\"Chara1\":\"FFFF0000\",\"Chara2\":\"FF00FF00\"},\"ExpressionOptions\": {\"Happy\":\"FF00FF00\",\"Sad\":\"FF0000FF\"}}";

        static DesignDataFactory()
        {
            Main = new VmMain();
            Dialog = Main.Dialog;
            Options = Main.DialogOptions;
            Options.Read(TestDialogOptions, null);
            NodeOptions = Options.CharacterOptions;
            NodeIcons = Options.NodeIcons;
        }

        public static VmMain Main { get; }

        public static VmDialog Dialog { get; }

        public static VmDialogOptions Options { get; }

        public static VmNodeOptions NodeOptions { get; }

        public static VmNodeIcons NodeIcons { get; }
    }
}
