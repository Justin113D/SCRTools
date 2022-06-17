using System;

namespace SCR.Tools.TranslationEditor.Data.Events
{
    public class NodeStateChangedEventArgs : EventArgs
    {
        public NodeState OldState { get; }
        public NodeState NewState { get; }

        public NodeStateChangedEventArgs(NodeState oldState, NodeState newState)
        {
            OldState = oldState;
            NewState = newState;
        }
    }

    public delegate void NodeStateChangedEventHandler(Node source, NodeStateChangedEventArgs args);
}
