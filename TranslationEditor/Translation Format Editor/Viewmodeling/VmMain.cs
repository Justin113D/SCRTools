using SCR.Tools.TranslationEditor.Data;
using SCR.Tools.UndoRedo;
using SCR.Tools.Viewmodeling;
using System.IO;

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
        public RelayCommand CmdUndo
            => new(Undo);

        /// <summary>
        /// Redo Command
        /// </summary>
        public RelayCommand CmdRedo
            => new(Redo);

        public RelayCommand CmdDeleteSelected
            => new(DeleteSelected);

        public VmMain()
        {
            FormatTracker = new();
            FormatTracker.Use();
            Format = new(this, new());
        }

        /// <summary>
        /// Sets/Refreshes the message display
        /// </summary>
        /// <param name="message">Message to display</param>
        /// <param name="warning">Whether its a warning</param>
        public void SetMessage(string message, bool warning)
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

        private void DeleteSelected()
        {
            Format.CmdRemoveSelected.Execute(null);
        }

        public void LoadFormat(string content)
        {
            try
            {
                HeaderNode headerNode = JsonFormatHandler.ReadFormat(content);

                FormatTracker.Reset();
                Format = new(this, headerNode);
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
            string result = Format.Header.WriteFormat(Properties.Settings.Default.JsonIndenting);
            SetMessage("Saved Format", false);
            return result;
        }

        public void NewFormat()
        {
            FormatTracker.Reset();
            Format = new(this, new());
            SetMessage("Created new Format", false);
        }

        public void ExportLanguage(string filepath)
        {
            if (Format == null)
            {
                SetMessage("No Format Loaded!", true);
                return;
            }

            (string keys, string values) = Format.Header.ExportLanguageData();

            File.WriteAllText(filepath, values);

            string keyFilePath = Path.ChangeExtension(filepath, "langkey");
            File.WriteAllText(keyFilePath, keys);

            SetMessage("Exported Language Files", false);
        }

    }
}
