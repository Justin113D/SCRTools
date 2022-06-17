using System;
using System.Collections.Generic;

namespace SCR.Tools.UndoRedo
{
    [Obsolete("Please use a track collection/list")]
    public struct ChangedListSingleEntry<T> : ITrackable
    {
        /// <summary>
        /// The collection that was modified
        /// </summary>
        private readonly IList<T> _collection;

        /// <summary>
        /// The object that was inserted/removed
        /// </summary>
        private readonly T _obj;


        private readonly int? _oldIndex;

        private readonly int? _newIndex;

        private readonly Action _postChange;

        public ChangedListSingleEntry(IList<T> collection, T obj, int? newIndex, Action postChange)
        {
            _collection = collection;
            _obj = obj;
            _oldIndex = _collection.IndexOf(obj);
            if (_oldIndex == -1)
                _oldIndex = null;
            _newIndex = newIndex;
            _postChange = postChange;
        }

        public void Redo()
        {
            if (_oldIndex.HasValue)
                _collection.Remove(_obj);
            if (_newIndex.HasValue)
                _collection.Insert(_newIndex.Value, _obj);
            _postChange?.Invoke();
        }

        public void Undo()
        {
            if (_newIndex.HasValue)
                _collection.Remove(_obj);
            if (_oldIndex.HasValue)
                _collection.Insert(_oldIndex.Value, _obj);
            _postChange?.Invoke();
        }

    }
}
