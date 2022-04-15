namespace SCR.Tools.UndoRedo
{
    internal class TrackGroup : ITrackable
    {
        internal List<ITrackable> Changes { get; }

        internal List<Action> PostGroupActions { get; }

        internal List<(Action<string>, string)> NotifyProperties { get; }


        public TrackGroup()
        {
            Changes = new();
            PostGroupActions = new();
            NotifyProperties = new();
        }

        public void Redo()
        {
            foreach (ITrackable trackable in Changes)
                trackable.Redo();

            foreach (Action a in PostGroupActions)
                a.Invoke();

            foreach ((Action<string> action, string name) in NotifyProperties)
                action.Invoke(name);
        }

        public void Undo()
        {
            foreach (ITrackable trackable in Changes.Reverse<ITrackable>())
                trackable.Undo();

            foreach (Action a in PostGroupActions)
                a.Invoke();

            foreach ((Action<string> action, string name) in NotifyProperties)
                action.Invoke(name);
        }
    }
}
