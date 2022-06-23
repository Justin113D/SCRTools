using SCR.Tools.UndoRedo.Trackable;
using System;
using System.Collections.Generic;

namespace SCR.Tools.UndoRedo
{
    /// <summary>
    /// Undo-Redo Change system
    /// </summary>
    public class ChangeTracker
    {
        #region Private

        /// <summary>
        /// Index of the current change
        /// </summary>
        private int _currentChangeIndex;

        /// <summary>
        /// Current tracking group
        /// </summary>
        private TrackGroup? _currentGroup;

        /// <summary>
        /// Amount of grouping
        /// </summary>
        private int _groupings;

        /// <summary>
        /// The tracked changes that can be undone and redone
        /// </summary>
        private readonly List<ITrackable> _trackedChanges;

        #endregion

        /// <summary>
        /// Used for checking if a change tracker is on the same change-state
        /// </summary>
        public struct Pin
        {
            private readonly ChangeTracker tracker;
            private readonly ITrackable? tracking;
            private readonly int index;

            public Pin(ChangeTracker tracker, ITrackable? tracking, int index)
            {
                this.tracker = tracker;
                this.tracking = tracking;
                this.index = index;
            }

            public bool CheckValid()
            {
                if (index == -1)
                    return tracker._currentChangeIndex == -1;

                return tracker._currentChangeIndex == index
                    && tracker._trackedChanges[tracker._currentChangeIndex] == tracking;
            }
        }

        /// <summary>
        /// Whether there are any active changes
        /// </summary>
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
        /// Places the instance into <see cref="Global"/>
        /// </summary>
        public void Use()
            => GlobalChangeTrackerC.GlobalChangeTracker = this;

        /// <summary>
        /// Resets the tracked changes
        /// </summary>
        public void Reset()
        {
            _trackedChanges.Clear();
            _currentChangeIndex = -1;
        }

        /// <summary>
        /// Undos the last change
        /// </summary>
        public bool Undo()
        {
            if (_currentGroup != null)
                throw new InvalidOperationException("Cannot perform Redo while grouping is active! Make sure to stop grouping using EndGroup()!");

            if (_currentChangeIndex == -1)
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
            if (_currentGroup != null)
                throw new InvalidOperationException("Cannot perform Redo while grouping is active! Make sure to stop grouping using EndGroup()!");

            if (_currentChangeIndex == _trackedChanges.Count - 1)
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
            if (_currentGroup != null)
                return;
            _currentGroup = new TrackGroup();
        }

        /// <summary>
        /// Adds an actio to perform after a group undo/redo <br/>
        /// Immidiately invokes, as a way to enact the change
        /// </summary>
        /// <param name="action"></param>
        public void PostGroupAction(Action action)
        {
            if (_currentGroup == null)
                throw new InvalidOperationException("No grouping active!");
            _currentGroup.PostGroupActions.Add(action);
            action.Invoke();
        }

        public void GroupNotifyPropertyChanged(Action<string> action, string name)
        {
            if (_currentGroup == null)
                throw new InvalidOperationException("No grouping active!");

            _currentGroup.NotifyProperties.Add((action, name));
            action.Invoke(name);
        }

        /// <summary>
        /// Finishes the grouping 
        /// </summary>
        /// <param name="discard">Undoes any changes and does not add the final grouping. Also resets <see cref="ResetOnNextChange"/></param>
        public void EndGroup(bool discard = false)
        {
            _groupings--;
            if (_currentGroup == null || _groupings > 0)
                return;

            if (_currentGroup.Changes.Count == 0)
            {
                _currentGroup = null;
                return;
            }

            if (discard)
            {
                _currentGroup.Undo();
                _currentGroup = null;
                return;
            }

            ClearRedos();

            if (_currentGroup.Changes.Count == 1
                && _currentGroup.PostGroupActions.Count == 0
                && _currentGroup.NotifyProperties.Count == 0)
                _trackedChanges.Add(_currentGroup.Changes[0]);
            else
                _trackedChanges.Add(_currentGroup);

            _currentGroup = null;
        }

        private void TrackChange(ITrackable trackable)
        {
            trackable.Redo();
            if (_currentGroup != null)
            {
                _currentGroup.Changes.Add(trackable);
                return;
            }

            ClearRedos();
            _trackedChanges.Add(trackable);
        }

        /// <summary>
        /// Adds a change to undo/redo <br/>
        /// Performs "redo" after tracking, performing the change
        /// </summary>
        /// <param name="change">The change</param>
        public void TrackChange(Action redo, Action undo)
            => TrackChange(new Change(redo, undo));

        /// <summary>
        /// Creates a value change
        /// </summary>
        /// <param name="modifyCallback">Callback which executes the value change</param>
        /// <param name="oldValue">Old value (passed on undo)</param>
        /// <param name="newValue">New value (passed on redo)</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void TrackValueChange<T>(Action<T> modifyCallback, T oldValue, T newValue)
            => TrackChange(new ValueChange<T>(modifyCallback, oldValue, newValue));

        /// <summary>
        /// Creates a change that does nothing
        /// </summary>
        public void BlankChange()
            => TrackChange(new BlankChange());

        public void BlankValueChange<T>(Action<T> modifyCallback, T oldValue, T newValue) where T : IEquatable<T>
        {
            if (oldValue.Equals(newValue))
            {
                BlankChange();
            }
            else
            {
                TrackValueChange(modifyCallback, oldValue, newValue);
            }
        }

        private void ClearRedos()
        {
            _currentChangeIndex++;
            int removeCount = _trackedChanges.Count - _currentChangeIndex;
            if (removeCount > 0)
                _trackedChanges.RemoveRange(_currentChangeIndex, removeCount);
        }

        public Pin PinCurrent()
            => new(this, _currentChangeIndex == -1 ? null : _trackedChanges[_currentChangeIndex], _currentChangeIndex);

    }
}
