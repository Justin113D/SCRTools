using System.Collections.Generic;

namespace SCR.Tools.UndoRedo.ListChange
{
    /// <summary>
    /// <see cref="ITrackable"/> struct for when an item is added to an <see cref="IList{T}"/>
    /// </summary>
    /// <typeparam name="T">Type of the item</typeparam>
    public struct ChangeListAdd<T> : ITrackable
    {
        private readonly IList<T> _list;
        private readonly T _item;

        public ChangeListAdd(IList<T> list, T item)
        {
            _list = list;
            _item = item;
        }

        public void Redo() => _list.Add(_item);

        public void Undo() =>_list.Remove(_item);
    }
}
