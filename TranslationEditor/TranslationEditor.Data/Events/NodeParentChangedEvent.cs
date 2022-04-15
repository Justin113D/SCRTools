namespace SCR.Tools.TranslationEditor.Data.Events
{
    public class NodeParentChangedEventArgs : EventArgs
    {
        public ParentNode? OldParent { get; }
        public ParentNode? NewParent { get; }

        public NodeParentChangedEventArgs(ParentNode? oldParent, ParentNode? newParent)
        {
            OldParent = oldParent;
            NewParent = newParent;
        }
    }

    public delegate void NodeParentChangedEventHandler(Node source, NodeParentChangedEventArgs args);
}
