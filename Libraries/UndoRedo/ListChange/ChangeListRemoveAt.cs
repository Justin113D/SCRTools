using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCR.Tools.UndoRedo.ListChange
{
    public class ChangeListRemoveAt<T> : ITrackable
    {
        private readonly IList<T> _list;
        private readonly T _item;
        private readonly int _index;

        public ChangeListRemoveAt(IList<T> list, int index)
        {
            _list = list;
            _index = index;
            _item = list[index];
        }

        public IList<T> Get_list()
        {
            return _list;
        }

        public void Redo() => _list.RemoveAt(_index);

        public void Undo() => _list.Insert(_index, _item);
    }
}
