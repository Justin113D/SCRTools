using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCR.Tools.UndoRedo
{
    /// <summary>
    /// Undo-Redo Change system
    /// </summary>
    public class ChangeTracker
    {
        public static ChangeTracker Global { get; private set; }

        #region Private
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
                for (int i = 0; i < changes.Count; i++)
                    changes[i].Redo();
                foreach (Action a in postGroupActions)
                    a.Invoke();
            }

            public void Undo()
            {
                for (int i = changes.Count - 1; i >= 0; i--)
                    changes[i].Undo();
                foreach (Action a in postGroupActions)
                    a.Invoke();
            }
        }

        /// <summary>
        /// Index of the current change
        /// </summary>
        private int _currentChangeIndex;

        /// <summary>
        /// Current tracking group
        /// </summary>
        private TrackGroup _currentGroup;

        /// <summary>
        /// Amount of grouping
        /// </summary>
        private int _groupings;

        /// <summary>
        /// The tracked changes that can be undone and redone
        /// </summary>
        private readonly List<ITrackable> _trackedChanges;

        #endregion

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
                if (index == -1)
                    return tracker._currentChangeIndex == -1;

                return tracker._currentChangeIndex == index
                    && tracker._trackedChanges[tracker._currentChangeIndex] == tracking;
            }
        }

        public bool ResetOnNextChange { get; set; }

        public bool HasChanges => _currentChangeIndex > -1;

        static ChangeTracker()
        {
            Global = new ChangeTracker();
        }

        /// <summary>
        /// Creates a new change tracker
        /// </summary>
        public ChangeTracker()
        {
            _trackedChanges = new List<ITrackable>();
            _currentChangeIndex = -1;
        }

        public void Use()
            => Global = this;

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

            if (_currentChangeIndex == -1 || ResetOnNextChange)
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

            if (_currentChangeIndex == _trackedChanges.Count - 1 || ResetOnNextChange)
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
            _currentGroup.postGroupActions.Add(action);
            action.Invoke();
        }

        /// <summary>
        /// Finishes the grouping
        /// </summary>
        public void EndGroup(bool discard = false)
        {
            _groupings--;
            if (_groupings > 0 || _currentGroup.changes.Count == 0)
                return;

            if (discard)
            {
                _currentGroup.Undo();
                _currentGroup = null;
                return;
            }

            ClearRedos();

            if (_currentGroup.changes.Count == 1 && _currentGroup.postGroupActions.Count == 0)
                _trackedChanges.Add(_currentGroup.changes[0]);
            else
                _trackedChanges.Add(_currentGroup);

            _currentGroup = null;
        }

        /// <summary>
        /// Adds a change to undo/redo <br/>
        /// Performs "redo" after tracking, as a way to enact the change
        /// </summary>
        /// <param name="change">The change</param>
        public void TrackChange(ITrackable change)
        {
            change.Redo();
            if (_currentGroup != null)
            {
                _currentGroup.changes.Add(change);
                return;
            }

            ClearRedos();
            _trackedChanges.Add(change);
        }

        private void ClearRedos()
        {
            if (ResetOnNextChange)
            {
                _currentChangeIndex = 0;
                ResetOnNextChange = false;
            }
            else
                _currentChangeIndex++;

            if (_currentChangeIndex < _trackedChanges.Count - 1)
                _trackedChanges.RemoveRange(_currentChangeIndex, _trackedChanges.Count - _currentChangeIndex);
        }

        public Pin PinCurrent()
            => new(this, _currentChangeIndex == -1 ? null : _trackedChanges[_currentChangeIndex], _currentChangeIndex);

    }
}
