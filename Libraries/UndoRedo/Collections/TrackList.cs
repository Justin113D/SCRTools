using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SCR.Tools.UndoRedo.Collections
{
    public class TrackList<T> : IList<T>
    {
        private readonly IList<T> _list;


        public T this[int index]
        {
            get => _list[index];
            set
            {
                T previousItem = _list[index];

                ChangeTracker.Global.TrackChange(
                    new Change(
                        () => _list[index] = value,
                        () => _list[index] = previousItem));
            }
        }

        public int Count => _list.Count;

        public bool IsReadOnly => _list.IsReadOnly;


        public TrackList(IList<T> list)
        {
            _list = list;
        }

        public TrackList() : this(new List<T>()) { }


        public void Add(T item)
        {
            ChangeTracker.Global.TrackChange(
                new Change(
                    () => _list.Add(item),
                    () => _list.Remove(item)));
        }

        public void AddRange(IEnumerable<T> range)
        {
            ChangeTracker.Global.BeginGroup();
            foreach (T item in range)
            {
                Add(item);
            }
            ChangeTracker.Global.EndGroup();
        }

        public void Insert(int index, T item)
        {
            ChangeTracker.Global.TrackChange(
                new Change(
                    () => _list.Insert(index, item),
                    () => _list.RemoveAt(index)));
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);

            if (index < 0)
            {
                ChangeTracker.Global.BlankChange();
                return false;
            }

            ChangeTracker.Global.TrackChange(
                new Change(
                    () => _list.RemoveAt(index),
                    () => _list.Insert(index, item)));

            return true;
        }

        public void RemoveAt(int index)
        {
            T item = this[index];

            ChangeTracker.Global.TrackChange(
                new Change(
                    () => _list.RemoveAt(index),
                    () => _list.Insert(index, item)));
        }

        public void Clear()
        {
            T[] contents = _list.ToArray();

            ChangeTracker.Global.TrackChange(
                new Change(
                    () => _list.Clear(),
                    () =>
                    {
                        foreach (T item in contents)
                        {
                            _list.Add(item);
                        }
                    }));
        }



        public void CopyTo(T[] array, int arrayIndex)
            => _list.CopyTo(array, arrayIndex);

        public bool Contains(T item)
            => _list.Contains(item);

        public int IndexOf(T item)
            => _list.IndexOf(item);


        public T? Find(Predicate<T> match)
        {
            foreach (T item in this)
            {
                if (match(item))
                {
                    return item;
                }
            }

            return default;
        }

        public List<T> FindAll(Predicate<T> match)
        {
            List<T> result = new();

            foreach (T item in this)
            {
                if (match(item))
                {
                    result.Add(item);
                }
            }

            return result;
        }

        public int FindIndex(int startIndex, int count, Predicate<T> match)
        {
            for (int i = startIndex; i < count; i++)
            {
                T item = this[i];
                if (match(item))
                {
                    return i;
                }
            }

            return -1;
        }

        public int FindIndex(int startIndex, Predicate<T> match)
            => FindIndex(startIndex, Count, match);

        public int FindIndex(Predicate<T> match)
            => FindIndex(0, Count, match);


        public T? FindLast(Predicate<T> match)
        {
            T? result = default;

            foreach (T item in this)
            {
                if (match(item))
                {
                    result = item;
                }
            }

            return result;
        }

        public int FindLastIndex(int startIndex, int count, Predicate<T> match)
        {
            int result = -1;

            for (int i = startIndex; i < count; i++)
            {
                T item = this[i];
                if (match(item))
                {
                    result = i;
                }
            }

            return result;
        }

        public int FindLastIndex(int startIndex, Predicate<T> match)
            => FindLastIndex(startIndex, Count, match);

        public int FindLastIndex(Predicate<T> match)
            => FindLastIndex(0, Count, match);



        public IEnumerator<T> GetEnumerator()
            => _list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
