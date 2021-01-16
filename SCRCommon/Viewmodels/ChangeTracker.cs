using System;
using System.Collections.Generic;
using System.Linq;

namespace SCRCommon.Viewmodels
{
    public interface ITrackable
    {
        void Undo();

        void Redo();
    }



    /// <summary>
    /// Undo-Redo Change system
    /// </summary>
    public class ChangeTracker
    {
        private class TrackGroup : ITrackable
        {
            public readonly List<ITrackable> changes;

            public readonly List<Action> postGroupActions;

            public TrackGroup()
            {
                changes = new List<ITrackable>();
                postGroupActions = new List<Action>();
            }

            public override bool Equals(object obj)
            {
                return obj is TrackGroup group &&
                       EqualityComparer<List<ITrackable>>.Default.Equals(changes, group.changes) &&
                       EqualityComparer<List<Action>>.Default.Equals(postGroupActions, group.postGroupActions);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(changes, postGroupActions);
            }

            public void Redo()
            {
                for(int i = 0; i < changes.Count; i++)
                    changes[i].Redo();
                foreach(Action a in postGroupActions)
                    a.Invoke();
            }

            public void Undo()
            {
                for(int i = changes.Count - 1; i >= 0; i--)
                    changes[i].Undo();
                foreach(Action a in postGroupActions)
                    a.Invoke();
            }
        }

        public struct Pin
        {
            private readonly ChangeTracker tracker;
            private readonly ITrackable tracking;
            private readonly int index;

            public Pin(ChangeTracker tracker, ITrackable tracking, int index)
            {
                this.tracker = tracker;
                this.tracking = tracking;
                this.index = index;
            }

            public bool CheckValid()
            {
                if(index == -1)
                    return tracker._currentChangeIndex == -1;

                return tracker._currentChangeIndex == index
                    && tracker._trackedChanges[tracker._currentChangeIndex] == tracking;
            }
        }

        /// <summary>
        /// Index of the current change
        /// </summary>
        private int _currentChangeIndex;

        private TrackGroup _currentGroup;

        private int _groupings;

        private int _resets;

        public bool ResetOnNextChange { get; set; }

        /// <summary>
        /// The tracked changes that can be undone and redone
        /// </summary>
        private readonly List<ITrackable> _trackedChanges;

        public bool HasChanges => _currentChangeIndex > -1;

        /// <summary>
        /// Creates a new change tracker
        /// </summary>
        public ChangeTracker()
        {
            _trackedChanges = new List<ITrackable>();
            _currentChangeIndex = -1;
        }

        /// <summary>
        /// Resets the tracked changes
        /// </summary>
        public void Reset()
        {
            _trackedChanges.Clear();
            _currentChangeIndex = -1;
            _resets++;
        }

        /// <summary>
        /// Undos the last change
        /// </summary>
        public bool Undo()
        {
            if(_currentGroup != null)
                throw new InvalidOperationException("Cannot perform Redo while grouping is active! Make sure to stop grouping using EndGroup()!");

            if(_currentChangeIndex == -1 || ResetOnNextChange)
                return false;

            _trackedChanges[_currentChangeIndex].Undo();
            _currentChangeIndex--;
            return true;
        }

        /// <summary>
        /// Redos the last undo
        /// </summary>
        public bool Redo()
        {
            if(_currentGroup != null)
                throw new InvalidOperationException("Cannot perform Redo while grouping is active! Make sure to stop grouping using EndGroup()!");

            if(_currentChangeIndex == _trackedChanges.Count - 1 || ResetOnNextChange)
                return false;

            _currentChangeIndex++;
            _trackedChanges[_currentChangeIndex].Redo();
            return true;
        }

        /// <summary>
        /// Groups all incoming changes into one change until <see cref="EndGroup"/> is called
        /// </summary>
        public void BeginGroup()
        {
            _groupings++;
            if(_currentGroup != null)
                return;
            _currentGroup = new TrackGroup();
        }

        public void PostGroupAction(Action action)
        {
            if(_currentGroup == null)
                throw new InvalidOperationException("No grouping active!");
            _currentGroup.postGroupActions.Add(action);
        }

        /// <summary>
        /// Finishes the grouping
        /// </summary>
        public void EndGroup()
        {
            _groupings--;
            if(_groupings > 0)
                return;


            if(_currentGroup.changes.Count == 0)
                return;

            ClearRedos();

            if(_currentGroup.changes.Count == 1 && _currentGroup.postGroupActions.Count == 0)
                _trackedChanges.Add(_currentGroup.changes[0]);
            else
                _trackedChanges.Add(_currentGroup);

            _currentGroup = null;
        }

        /// <summary>
        /// Adds a change to undo/redo
        /// </summary>
        /// <param name="change">The change</param>
        public void TrackChange(ITrackable change)
        {
            if(_currentGroup != null)
            {
                _currentGroup.changes.Add(change);
                return;
            }

            ClearRedos();
            _trackedChanges.Add(change);
        }

        /// <summary>
        /// Adds a <see cref="Change"/> to the tracker
        /// </summary>
        /// <param name="change"></param>
        public void TrackChange(Action<bool> change)
            => TrackChange(new Change(change));

        private void ClearRedos()
        {
            if(ResetOnNextChange)
            {
                _currentChangeIndex = 0;
                ResetOnNextChange = false;
            }
            else
                _currentChangeIndex++;

            if(_currentChangeIndex < _trackedChanges.Count - 1)
                _trackedChanges.RemoveRange(_currentChangeIndex, _trackedChanges.Count - _currentChangeIndex);
        }

        public Pin PinCurrent()
            => new Pin(this, _currentChangeIndex == -1 ? null : _trackedChanges[_currentChangeIndex], _currentChangeIndex);

    }

    /// <summary>
    /// Calls an action upon undo/redo. Passes "false" for undo and "true" for redo
    /// </summary>
    public struct Change : ITrackable
    {
        private readonly Action<bool> _modifyCallback;

        public Change(Action<bool> modifyCallback)
        {
            _modifyCallback = modifyCallback ?? throw new ArgumentNullException("Modifycallback cant be null!");
        }

        public void Redo()
        {
            _modifyCallback.Invoke(true);
        }

        public void Undo()
        {
            _modifyCallback.Invoke(false);
        }

    }

    public struct ChangedValue<T> : ITrackable
    {
        private readonly Action<T> _modifyCallback;
        private readonly T _oldValue;
        private readonly T _newValue;

        public ChangedValue(Action<T> modifyCallback, T oldValue, T newValue)
        {
            _modifyCallback = modifyCallback ?? throw new ArgumentNullException("Modifycallback cant be null!");
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public void Redo()
        {
            _modifyCallback.Invoke(_newValue);
        }

        public void Undo()
        {
            _modifyCallback.Invoke(_oldValue);
        }

    }

    public struct ChangedList<T> : ITrackable
    {

        private readonly ICollection<T> _collection;

        private readonly Action _postChange;

        private readonly T[] _oldContents;

        private readonly T[] _newContents;

        public ChangedList(ICollection<T> collection, T[] old, Action postChange)
        {
            _collection = collection;
            _postChange = postChange;
            _oldContents = old;
            _newContents = collection.ToArray();
        }

        public void Undo()
        {
            _collection.Clear();
            foreach(var v in _oldContents)
                _collection.Add(v);
            _postChange?.Invoke();
        }

        public void Redo()
        {
            _collection.Clear();
            foreach(var v in _newContents)
                _collection.Add(v);
            _postChange?.Invoke();
        }

    }

    public struct ChangedListSingleEntry<T> : ITrackable
    {
        private readonly IList<T> _collection;

        private readonly T obj;

        private readonly int? oldIndex;

        private readonly int? newIndex;

        private readonly Action _postChange;

        public ChangedListSingleEntry(IList<T> collection, T obj, int? newIndex, Action postChange)
        {
            _collection = collection;
            this.obj = obj;
            oldIndex = _collection.IndexOf(obj);
            if(oldIndex == -1)
                oldIndex = null;
            this.newIndex = newIndex;
            _postChange = postChange;
        }

        public void Redo()
        {
            if(oldIndex.HasValue)
                _collection.Remove(obj);
            if(newIndex.HasValue)
                _collection.Insert(newIndex.Value, obj);
            _postChange?.Invoke();
        }

        public void Undo()
        {
            if(newIndex.HasValue)
                _collection.Remove(obj);
            if(oldIndex.HasValue)
                _collection.Insert(oldIndex.Value, obj);
            _postChange?.Invoke();
        }

    }
}
