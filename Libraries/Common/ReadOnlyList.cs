using System.Collections;
using System.Collections.Generic;

namespace SCR.Tools.Common
{
    public class ReadOnlyList<T> : IReadOnlyList<T>
    {
        private readonly IReadOnlyList<T> _list;

        public T this[int index] => _list[index];

        public int Count => _list.Count;

        public ReadOnlyList(IReadOnlyList<T> list)
        {
            _list = list;
        }

        public IEnumerator<T> GetEnumerator()
            => _list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
