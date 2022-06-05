namespace SCR.Tools.UndoRedo
{
    /// <summary>
    /// Trackable for changing a single value
    /// </summary>
    /// <typeparam name="T">Type of the changed value</typeparam>
    public struct ChangedValue<T> : ITrackable
    {
        private readonly Action<T> _modifyCallback;
        private readonly T _oldValue;
        private readonly T _newValue;

        /// <summary>
        /// Creates a value change
        /// </summary>
        /// <param name="modifyCallback">Callback which executes the value change</param>
        /// <param name="oldValue">Old value (passed on undo)</param>
        /// <param name="newValue">New value (passed on redo)</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ChangedValue(Action<T> modifyCallback, T oldValue, T newValue)
        {
            _modifyCallback = modifyCallback ?? throw new ArgumentNullException(nameof(modifyCallback));
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public void Undo() 
            => _modifyCallback.Invoke(_oldValue);

        public void Redo() 
            => _modifyCallback.Invoke(_newValue);
    }
}
