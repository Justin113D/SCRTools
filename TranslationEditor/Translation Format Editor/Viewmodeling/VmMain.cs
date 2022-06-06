using SCR.Tools.TranslationEditor.Data;
using SCR.Tools.UndoRedo;
using SCR.Tools.Viewmodeling;

namespace SCR.Tools.TranslationEditor.FormatEditor.Viewmodeling
{
    public class VmMain : BaseViewModel
    {
        /// <summary>
        /// Viewmodel specific Change Tracker for undoing redoing
        /// </summary>
        public ChangeTracker FormatTracker { get; }

        /// <summary>
        /// The loaded format
        /// </summary>
        public VmFormat Format { get; private set; }

        /// <summary>
        /// Error/warning message that is displayed in the main window
        /// </summary>
        public string? Message { get; private set; }

        /// <summary>
        /// Used to display the message <br/>
        /// 0 = None <br/>
        /// 1 = Green <br/>
        /// 2 = Red
        /// </summary>
        public int DisplayMessage { get; private set; }


        /// <summary>
        /// Undo Command 
        /// </summary>
        public RelayCommand Cmd_Undo
            => new(Undo);

        /// <summary>
        /// Redo Command
        /// </summary>
        public RelayCommand Cmd_Redo
            => new(Redo);

        public VmMain()
        {
            FormatTracker = new();
            FormatTracker.Use();
            Format = new(new(), FormatTracker);
        }

        /// <summary>
        /// Sets/Refreshes the message display
        /// </summary>
        /// <param name="message">Message to display</param>
        /// <param name="warning">Whether its a warning</param>
        private void SetMessage(string message, bool warning)
        {
            if (DisplayMessage > 0)
            {
                DisplayMessage = 0;
                Message = "";
            }

            Message = message;
            DisplayMessage = warning ? 2 : 1;
        }

        /// <summary>
        /// Performs undo on the Change Tracker and displays the action to the user
        /// </summary>
        private void Undo()
        {
            if (FormatTracker.Undo())
            {
                SetMessage("Performed Undo", false);
            }
        }

        /// <summary>
        /// Performs redo on the Change Tracker and displays the action to the user
        /// </summary>
        private void Redo()
        {
            if (FormatTracker.Redo())
            {
                SetMessage("Performed Redo", false);
            }
        }

        public void ExpandAll()
            => Format?.ExpandAll();

        public void CollapseAll()
            => Format?.CollapseAll();


        public void LoadFormat(string path)
        {
            try
            {
                HeaderNode headerNode = JsonFormatHandler.ReadFormat(path);

                FormatTracker.Reset();
                Format = new(headerNode, FormatTracker);
                SetMessage("Loaded Format", false);
            }
            catch
            {
                SetMessage("Error loading Format", true);
                throw;
            }
        }

        public string WriteFormat()
        {
            string result = Format.WriteFormat();
            SetMessage("Saved Format", false);
            return result;
        }

        public void NewFormat()
        {
            FormatTracker.Reset();
            Format = new(new(), FormatTracker);
            SetMessage("Created new Format", false);
        }
    }
}
