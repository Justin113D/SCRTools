namespace SCR.Tools.TranslationEditor.Data.Events
{
    public class NodeChildrenChangedEvent : EventArgs
    {
        public int FromIndex { get; }

        public int ToIndex { get; }

        public NodeChildrenChangedEvent(int fromIndex, int toIndex)
        {
            FromIndex = fromIndex;
            ToIndex = toIndex;
        }
    }

    public delegate void NodeChildrenChangedEventHandler(ParentNode source, NodeChildrenChangedEvent args);
}
