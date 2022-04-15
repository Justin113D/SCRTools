namespace SCR.Tools.TranslationEditor.Data.Events
{
    public class NodeChildrenMovedEvent : EventArgs
    {
        public int FromIndex { get; }

        public int ToIndex { get; }

        public NodeChildrenMovedEvent(int fromIndex, int toIndex)
        {
            FromIndex = fromIndex;
            ToIndex = toIndex;
        }
    }

    public delegate void NodeChildrenMovedEventHandler(ParentNode source, NodeChildrenMovedEvent args);
}
