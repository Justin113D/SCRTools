using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCR.Tools.UndoRedo.ListChange
{
    public class ChangeListInsert<T> : ITrackable
    {
        private readonly IList<T> _list;
        private readonly T _item;
        private readonly int _index;

        public ChangeListInsert(IList<T> list, T item, int index)
        {
            _list = list;
            _index = index;
            _item = item;
        }

        public IList<T> Get_list()
        {
            return _list;
        }

        public void Redo() => _list.Insert(_index, _item);

        public void Undo() => _list.RemoveAt(_index);
    }
}
