using System;

namespace SCR.Tools.TranslationEditor.Data.Events
{
    public class NodeNameChangedEventArgs : EventArgs
    {
        public string OldName { get; }
        public string NewName { get; }

        public NodeNameChangedEventArgs(string oldName, string newName)
        {
            OldName = oldName;
            NewName = newName;
        }
    }

    public delegate void NodeNameChangedEventHandler(Node source, NodeNameChangedEventArgs args);
}
