using SCR.Tools.DialogEditor.Data;
using SCR.Tools.UndoRedo;
using SCR.Tools.Viewmodeling;

namespace SCR.Tools.DialogEditor.Viewmodeling
{
    public class VmMain : BaseViewModel
    {
        /// <summary>
        /// Viewmodel specific Change Tracker for undoing redoing
        /// </summary>
        public ChangeTracker DialogTracker { get; }

        public VmDialog Dialog { get; private set; }

        public VmDialogOptions DialogOptions { get; }


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
        public RelayCommand CmdUndo
            => new(Undo);

        /// <summary>
        /// Redo Command
        /// </summary>
        public RelayCommand CmdRedo
            => new(Redo);


        public VmMain()
        {
            DialogTracker = new();
            DialogTracker.Use();

            Dialog = new(this, new());
            DialogOptions = new();
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
            if (DialogTracker.Undo())
            {
                SetMessage("Performed Undo", false);
            }
        }

        /// <summary>
        /// Performs redo on the Change Tracker and displays the action to the user
        /// </summary>
        private void Redo()
        {
            if (DialogTracker.Redo())
            {
                SetMessage("Performed Redo", false);
            }
        }


        public void LoadDialog(string data)
        {
            try
            {
                Dialog dialog = JsonFormatHandler.ReadDialog(data);
                Dialog = new VmDialog(this, dialog);
                DialogTracker.Reset();
                SetMessage("Loaded Dialog", false);
            }
            catch
            {
                SetMessage("Error loading Dialog", true);
                throw;
            }
        }

        public string WriteDialog()
        {
            string result = Dialog.Data.WriteDialog(true);
            SetMessage("Saved Dialog", false);
            return result;
        }

        public void NewDialog()
        {
            Dialog = new(this, new());
            DialogTracker.Reset();
            SetMessage("Created new Format", false);
        }

    }
}
