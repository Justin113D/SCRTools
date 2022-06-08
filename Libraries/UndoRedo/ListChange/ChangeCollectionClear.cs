namespace SCR.Tools.UndoRedo.ListChange
{
    /// <summary>
    /// <see cref="ITrackable"/> struct for when a <see cref="ICollection{T}"/> is cleared
    /// </summary>
    /// <typeparam name="T">Type of the item</typeparam>
    public struct ChangeCollectionClear<T> : ITrackable
    {
        private readonly ICollection<T> _collection;

        private readonly T[] _contents;

        public ChangeCollectionClear(ICollection<T> list)
        {
            _collection = list;
            _contents = _collection.ToArray();
        }

        public void Redo() => _collection.Clear();

        public void Undo()
        {
            foreach(T item in _contents)
            {
                _collection.Add(item);
            }
        }
    }
}
