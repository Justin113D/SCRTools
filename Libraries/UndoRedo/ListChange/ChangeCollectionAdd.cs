namespace SCR.Tools.UndoRedo.ListChange
{
    public struct ChangeCollectionAdd<T> : ITrackable
    {
        private readonly ICollection<T> __collection;
        private readonly T _item;

        public ChangeCollectionAdd(ICollection<T> list, T item)
        {
            __collection = list;
            _item = item;
        }

        public void Redo() => __collection.Add(_item);

        public void Undo() => __collection.Remove(_item);
    }
}
