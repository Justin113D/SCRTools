namespace SCR.Tools.UndoRedo
{
    /// <summary>
    /// Change tracker base interface
    /// </summary>
    public interface ITrackable
    {
        /// <summary>
        /// Undoes the tracked change
        /// </summary>
        void Undo();

        /// <summary>
        /// Redoes the tracked change
        /// </summary>
        void Redo();
    }
}
