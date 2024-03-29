﻿using SCR.Tools.TranslationEditor.FormatEditor.Viewmodeling;

namespace SCR.Tools.TranslationEditor.FormatEditor.WPF
{
    internal static class DesignDataFactory
    {
        const string _format = "{\"Name\":\"New Format\",\"DefaultLanguage\":\"English\",\"Author\":\"\",\"Versions\":[\"0.0.1\"],\"ChildNodes\":[{\"Name\":\"Single\",\"DefaultValue\":\"Test\"},{\"Name\":\"Parent\",\"ChildNodes\":[{\"Name\":\"ChildNode1\",\"DefaultValue\":\"Test\"},{\"Name\":\"ChildNode2\",\"DefaultValue\":\"Test\"},{\"Name\":\"Subparent\",\"ChildNodes\":[{\"Name\":\"SubChildNode1\",\"DefaultValue\":\"Test\"},{\"Name\":\"SubChildNode2\",\"DefaultValue\":\"Test\"}]}]}]}";

        static DesignDataFactory()
        {
            Main = new VmMain();
            Main.LoadFormat(_format);
            Format = Main.Format;
            Node = Format.Nodes[0];
        }

        public static VmMain Main { get; }

        public static VmFormat Format { get; }

        public static VmNode Node { get; }
    }
}
