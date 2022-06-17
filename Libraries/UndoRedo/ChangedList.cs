using System;
using System.Collections.Generic;
using System.Linq;

namespace SCR.Tools.UndoRedo
{
    [Obsolete("Please use a track collection/list")]
    public struct ChangedList<T> : ITrackable
    {

        private readonly ICollection<T> _collection;

        private readonly Action? _postChange;

        private readonly T[] _oldContents;

        private readonly T[] _newContents;

        public ChangedList(ICollection<T> collection, T[] newContents, Action? postChange)
        {
            _collection = collection;
            _postChange = postChange;
            _oldContents = collection.ToArray();
            _newContents = newContents;
        }

        public void Undo()
        {
            _collection.Clear();
            foreach (var v in _oldContents)
                _collection.Add(v);
            _postChange?.Invoke();
        }

        public void Redo()
        {
            _collection.Clear();
            foreach (var v in _newContents)
                _collection.Add(v);
            _postChange?.Invoke();
        }

    }
}
