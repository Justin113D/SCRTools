using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCR.Tools.UndoRedo.Collections
{
    public class TrackSet<T> : ISet<T>
    {
        private readonly ISet<T> _set;

        public int Count
            => _set.Count;

        public bool IsReadOnly
            => _set.IsReadOnly;

        public TrackSet()
        {
            _set = new HashSet<T>();
        }

        public TrackSet(ISet<T> set)
        {
            _set = set;
        }

        public bool Add(T item)
        {
            if(Contains(item))
            {
                return false;
            }

            TrackChange(
                () => _set.Add(item),
                () => _set.Remove(item));

            return true;
        }

        public void Clear()
        {
            T[] contents = _set.ToArray();

            TrackChange(
                () => _set.Clear(),
                () =>
                {
                    foreach (T item in contents)
                    {
                        _set.Add(item);
                    }
                });
        }

        public bool Contains(T item)
            => _set.Contains(item);

        public void CopyTo(T[] array, int arrayIndex)
            => _set.CopyTo(array, arrayIndex);

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

        public bool Remove(T item)
        {
            if (!Contains(item))
            {
                return false;
            }

            TrackChange(
                () => _set.Remove(item),
                () => _set.Add(item));

            return true;
        }

        public bool SetEquals(IEnumerable<T> other)
            => _set.SetEquals(other);


        public void ExceptWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public void UnionWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }


        void ICollection<T>.Add(T item)
            => Add(item);

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
