using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SCR.Tools.UndoRedo.Collections
{
    public class TrackDictionary<TKey, TValue> : IDictionary<TKey, TValue> where TKey : notnull
    {
        private readonly IDictionary<TKey, TValue> _dictionary;

        public TValue this[TKey key] 
        {
            get => _dictionary[key];
            set
            {
                TValue previousItem = _dictionary[key];

                TrackChange(
                    () => _dictionary[key] = value,
                    () => _dictionary[key] = previousItem);
            }
        }

        public ICollection<TKey> Keys 
            => _dictionary.Keys;

        public ICollection<TValue> Values 
            => _dictionary.Values;

        public int Count 
            => _dictionary.Count;

        public bool IsReadOnly 
            => _dictionary.IsReadOnly;

        public TrackDictionary()
        {
            _dictionary = new Dictionary<TKey, TValue>();
        }

        public TrackDictionary(IDictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary;
        }

        public void Add(TKey key, TValue value)
        {
            TrackChange(
                () => _dictionary.Add(key, value),
                () => _dictionary.Remove(key));
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            TrackChange(
                () => _dictionary.Add(item),
                () => _dictionary.Remove(item));
        }

        public void Clear()
        {
            KeyValuePair<TKey, TValue>[] contents = _dictionary.ToArray();

            TrackChange(
                () => _dictionary.Clear(),
                () =>
                {
                    foreach (KeyValuePair<TKey, TValue> item in contents)
                    {
                        _dictionary.Add(item);
                    }
                });
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
            => _dictionary.Contains(item);

        public bool ContainsKey(TKey key)
            => _dictionary.ContainsKey(key);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
            => _dictionary.CopyTo(array, arrayIndex);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            => _dictionary.GetEnumerator();

        public bool Remove(TKey key)
        {
            if(!TryGetValue(key, out TValue? value))
            {
                return false;
            }

            TrackChange(
                () => _dictionary.Remove(key),
                () => _dictionary.Add(key, value));

            return true;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if(!Contains(item))
            {
                return false;
            }

            TrackChange(
                () => _dictionary.Remove(item),
                () => _dictionary.Add(item));

            return true;
        }

        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
            => _dictionary.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
