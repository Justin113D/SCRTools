using System;

namespace SCR.Tools.Dialog.Data.Events
{
    public class OutputConnectionChangedEventArgs : EventArgs
    {
        public Node? OldNode { get; }

        public Node? NewNode { get; }

        public OutputConnectionChangedEventArgs(Node? oldNode, Node? newNode)
        {
            OldNode = oldNode;
            NewNode = newNode;
        }
    }

    public delegate void OutputConnectionChangedEventHandler(NodeOutput source, OutputConnectionChangedEventArgs args);
}
