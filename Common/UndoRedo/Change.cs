using System;

namespace SCR.Tools.UndoRedo
{
    /// <summary>
    /// Calls an action upon undo/redo. Passes "false" for undo and "true" for redo
    /// </summary>
    public struct Change : ITrackable
    {
        private readonly Action _undoCallback;

        private readonly Action _redoCallback;

        /// <summary>
        /// Create a change with a redo and undo callback
        /// </summary>
        /// <param name="redoCallback">Action called upon redoing</param>
        /// <param name="undoCallback">Action called upon undoing</param>
        /// <exception cref="ArgumentNullException"></exception>
        public Change(Action undoCallback, Action redoCallback)
        {
            _undoCallback = undoCallback ?? throw new ArgumentNullException(nameof(undoCallback));
            _redoCallback = redoCallback ?? throw new ArgumentNullException(nameof(redoCallback));
        }

        public void Undo() 
            => _undoCallback();

        public void Redo() 
            => _redoCallback.Invoke();
    }
}
