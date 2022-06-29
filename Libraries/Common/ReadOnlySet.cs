using System.Collections;
using System.Collections.Generic;

namespace SCR.Tools.Common
{
    public class ReadOnlySet<T> : IReadOnlySet<T>
    {
        private readonly IReadOnlySet<T> _set;

        public int Count => _set.Count;

        public ReadOnlySet(IReadOnlySet<T> set)
        {
            _set = set;
        }

        public bool Contains(T item)
            => _set.Contains(item);

        public IEnumerator<T> GetEnumerator()
            => _set.GetEnumerator();

        public bool IsProperSubsetOf(IEnumerable<T> other)
            => _set.IsProperSubsetOf(other);

        public bool IsProperSupersetOf(IEnumerable<T> other)
            => _set.IsProperSupersetOf(other);

        public bool IsSubsetOf(IEnumerable<T> other)
            => _set.IsSubsetOf(other);

        public bool IsSupersetOf(IEnumerable<T> other)
            => _set.IsSupersetOf(other);

        public bool Overlaps(IEnumerable<T> other)
            => _set.Overlaps(other);

        public bool SetEquals(IEnumerable<T> other)
            => _set.SetEquals(other);

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
