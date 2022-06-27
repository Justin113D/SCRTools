using SCR.Tools.UndoRedo.Collections;
using System.Drawing;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;

namespace SCR.Tools.Dialog.Data
{
    /// <summary>
    /// Dialog options
    /// </summary>
    public class DialogOptions
    {
        private string? _portraitsPath;

        /// <summary>
        /// All available character options
        /// </summary>
        public TrackDictionary<string, Color> CharacterOptions { get; }

        /// <summary>
        /// All available emotion options
        /// </summary>
        public TrackDictionary<string, Color> ExpressionOptions { get; }

        /// <summary>
        /// available node icons
        /// </summary>
        public TrackDictionary<string, string> NodeIcons { get; }

        public string? PortraitsPath
        {
            get => _portraitsPath;
            set
            {
                TrackValueChange(
                    (v) => _portraitsPath = v, _portraitsPath, value);
            }
        }

        public DialogOptions()
        {
            CharacterOptions = new();
            ExpressionOptions = new();
            NodeIcons = new();
        }
    }
}
