namespace SCR.Tools.TranslationEditor.Data.Events
{
    public class NodeChildrenChangedEventArgs : EventArgs
    {
        public int FromIndex { get; }

        public int ToIndex { get; }

        public NodeChildrenChangedEventArgs(int fromIndex, int toIndex)
        {
            FromIndex = fromIndex;
            ToIndex = toIndex;
        }
    }

    public delegate void NodeChildrenChangedEventHandler(ParentNode source, NodeChildrenChangedEventArgs args);
}
