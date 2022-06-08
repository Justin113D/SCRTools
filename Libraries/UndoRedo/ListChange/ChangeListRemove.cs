using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCR.Tools.UndoRedo.ListChange
{
    /// <summary>
    /// <see cref="ITrackable"/> struct for when an item is removed from an <see cref="IList{T}"/>
    /// </summary>
    /// <typeparam name="T">Type of the item</typeparam>
    public struct ChangeListRemove<T> : ITrackable
    {
        private readonly IList<T> _list;
        private readonly T _item;
        private readonly int _index;

        public ChangeListRemove(IList<T> list, T item)
        {
            _list = list;
            _item = item;
            _index = _list.IndexOf(item);
        }

        public void Redo() => _list.Remove(_item);

        public void Undo() => _list.Insert(_index, _item);
    }
}
