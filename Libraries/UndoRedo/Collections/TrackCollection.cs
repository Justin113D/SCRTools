using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SCR.Tools.UndoRedo.Collections
{
    public class TrackCollection<T> : ICollection<T>
    {
        private readonly ICollection<T> _collection;

        public int Count 
            => _collection.Count;

        public bool IsReadOnly 
            => _collection.IsReadOnly;

        public TrackCollection(ICollection<T> collection)
        {
            _collection = collection;
        }

        public void Add(T item)
        {
            ChangeTracker.Global.TrackChange(
                new Change(
                    () => _collection.Add(item),
                    () => _collection.Remove(item)));
        }

        public void Clear()
        {
            T[] contents = _collection.ToArray();

            ChangeTracker.Global.TrackChange(
                new Change(
                    () => _collection.Clear(),
                    () =>
                    {
                        foreach (T item in contents)
                        {
                            _collection.Add(item);
                        }
                    }));
        }

        public bool Contains(T item)
            => _collection.Contains(item);

        public void CopyTo(T[] array, int arrayIndex)
            => _collection.CopyTo(array, arrayIndex);

        public IEnumerator<T> GetEnumerator()
            => _collection.GetEnumerator();

        public bool Remove(T item)
        {
            if (!Contains(item))
            {
                return false;
            }

            ChangeTracker.Global.TrackChange(
                new Change(
                    () => _collection.Remove(item),
                    () => _collection.Add(item)));

            return true;
        }

        IEnumerator IEnumerable.GetEnumerator()
            => _collection.GetEnumerator();
    }
}
