namespace SCR.Tools.UndoRedo.ListChange
{
    public struct ChangeCollectionContent<T> : ITrackable
    {

        private readonly ICollection<T> _collection;

        private readonly T[] _oldContents;

        private readonly T[] _newContents;

        public ChangeCollectionContent(ICollection<T> collection, T[] newContents)
        {
            _collection = collection;
            _oldContents = collection.ToArray();
            _newContents = newContents;
        }

        public void Undo()
        {
            _collection.Clear();
            foreach (var v in _oldContents)
                _collection.Add(v);
        }

        public void Redo()
        {
            _collection.Clear();
            foreach (var v in _newContents)
                _collection.Add(v);
        }

    }
}
