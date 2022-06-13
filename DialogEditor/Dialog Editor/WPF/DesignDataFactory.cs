using SCR.Tools.DialogEditor.Viewmodeling;

namespace SCR.Tools.DialogEditor.WPF
{
    internal static class DesignDataFactory
    {
        static DesignDataFactory()
        {
            Main = new VmMain();
            Dialog = new VmDialog(Main, new());

        }

        public static VmMain Main { get; }

        public static VmDialog Dialog { get; }
    }
}
