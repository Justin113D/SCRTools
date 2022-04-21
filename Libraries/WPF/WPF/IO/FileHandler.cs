using SCR.Tools.UndoRedo;
using System;

namespace SCR.Tools.WPF.IO
{
    /// <summary>
    /// File handler with custom handling
    /// </summary>
    public class FileHandler : BaseFileHandler
    {
        private readonly Action<string>? _toFile;
        private readonly Action<string>? _fromFile;
        private readonly Action? _reset;

        /// <param name="fileFilter">Dialog File Filter</param>
        /// <param name="fileTypeName">How the file type is called</param>
        /// <param name="pinTracker">Tracker for checking whether the user should save</param>
        /// <param name="toFile">Called upon saving to a file. <br/>Parameter is file path</param>
        /// <param name="fromFile">Called upon loading file. <br/>Parameter is file path</param>
        /// <param name="reset">Resets the data to a "new file"</param>
        public FileHandler(
            string fileFilter, 
            string fileTypeName, 
            ChangeTracker? pinTracker, 
            Action<string>? toFile, 
            Action<string>? fromFile, 
            Action? reset) 
            : 
            base(fileFilter, fileTypeName, pinTracker)
        {
            _toFile = toFile;
            _fromFile = fromFile;
            _reset = reset;
        }

        protected override void InternalReset()
        {
            if (_reset == null)
                throw new InvalidOperationException("Reset action not specified");
            _reset();
        }

        protected override void InternalSave(string filePath)
        {
            if (_toFile == null)
                throw new InvalidOperationException("Save action not specified");
            _toFile(filePath);
        }

        protected override void InternalLoad(string filePath)
        {
            if (_fromFile == null)
                throw new InvalidOperationException("Load action not specified");
            _fromFile(filePath);
        }
    }
}
