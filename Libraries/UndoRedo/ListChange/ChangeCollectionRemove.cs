using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCR.Tools.UndoRedo.ListChange
{
    public struct ChangeCollectionRemove<T> : ITrackable
    {
        private readonly ICollection<T> __collection;
        private readonly T _item;

        public ChangeCollectionRemove(ICollection<T> list, T item)
        {
            __collection = list;
            _item = item;
        }

        public void Redo() => __collection.Remove(_item);

        public void Undo() => __collection.Add(_item);
    }
}
