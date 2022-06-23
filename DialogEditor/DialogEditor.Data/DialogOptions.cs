using SCR.Tools.UndoRedo.Collections;
using System.Drawing;

namespace SCR.Tools.DialogEditor.Data
{
    /// <summary>
    /// Dialog options
    /// </summary>
    public class DialogOptions
    {
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

        public DialogOptions()
        {
            CharacterOptions = new();
            ExpressionOptions = new();
            NodeIcons = new();
        }
    }
}
