using System;

namespace SCR.Tools.UndoRedo.Trackable
{
    /// <summary>
    /// Calls an action upon undo/redo. Passes "false" for undo and "true" for redo
    /// </summary>
    internal struct Change : ITrackable
    {
        private readonly Action _redoCallback;

        private readonly Action _undoCallback;

        /// <summary>
        /// Create a change with a redo and undo callback
        /// </summary>
        /// <param name="redoCallback">Action called upon redoing</param>
        /// <param name="undoCallback">Action called upon undoing</param>
        /// <exception cref="ArgumentNullException"></exception>
        public Change(Action redoCallback, Action undoCallback)
        {
            _redoCallback = redoCallback ?? throw new ArgumentNullException(nameof(redoCallback));
            _undoCallback = undoCallback ?? throw new ArgumentNullException(nameof(undoCallback));
        }

        public void Redo()
            => _redoCallback.Invoke();

        public void Undo()
            => _undoCallback();

    }
}
