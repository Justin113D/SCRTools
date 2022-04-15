namespace SCR.Tools.TranslationEditor.Data.Events
{
    public class NodeHeaderChangedEventArgs : EventArgs
    {
        public HeaderNode? OldHeader { get; }
        public HeaderNode? NewHeader { get; }

        public NodeHeaderChangedEventArgs(HeaderNode? oldHeader, HeaderNode? newHeader)
        {
            OldHeader = oldHeader;
            NewHeader = newHeader;
        }
    }

    public delegate void NodeHeaderChangedEventHandler(Node source, NodeHeaderChangedEventArgs args);
}
