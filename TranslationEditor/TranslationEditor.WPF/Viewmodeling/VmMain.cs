using SCR.Tools.TranslationEditor.Data;
using SCR.Tools.UndoRedo;
using SCR.Tools.Viewmodeling;

namespace SCR.Tools.TranslationEditor.WPF.Viewmodeling
{
    public class VmMain : BaseViewModel
    {
        /// <summary>
        /// Viewmodel specific Change Tracker for undoing redoing
        /// </summary>
        public ChangeTracker ProjectTracker { get; }

        /// <summary>
        /// The loaded format
        /// </summary>
        public VmProject? Format { get; private set; }

        /// <summary>
        /// Whether a format is loaded
        /// </summary>
        public bool FormatLoaded 
            => Format != null;


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
            ProjectTracker = new();
            ProjectTracker.Use();
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
            ProjectTracker.Undo();

            SetMessage("Performed Undo", false);
        }

        /// <summary>
        /// Performs redo on the Change Tracker and displays the action to the user
        /// </summary>
        private void Redo()
        {
            ProjectTracker.Redo();

            SetMessage("Performed Redo", false);
        }


        public void ExpandAll()
            => Format?.ExpandAll();

        public void CollapseAll()
            => Format?.CollapseAll();


        /// <summary>
        /// Loads a format from a Json string object
        /// </summary>
        /// <param name="format">The format Json string</param>
        public void LoadFormat(string format)
        {
            try
            {
                ProjectTracker.ResetOnNextChange = true;
                HeaderNode headerNode = JsonFormatHandler.ReadFormat(format);

                Format = new(headerNode, ProjectTracker);
                OnPropertyChanged(nameof(FormatLoaded));

                SetMessage("Loaded Format", false);
            }
            catch
            {
                SetMessage("Error loading Format", true);
                throw;
            }
        }

        /// <summary>
        /// Loads project data
        /// </summary>
        /// <param name="data">Project data to be loaded</param>
        public void LoadProject(string data)
        {
            if (Format == null)
                return;

            try
            {
                Format.LoadProject(data);
                SetMessage("Loaded Project", false);
            }
            catch
            {
                SetMessage("Failed to load Project", false);
                throw;
            }
        }


        public string WriteProject()
        {
            if (Format == null)
                return "";

            try
            {
                string data = Format.CompileProject();
                SetMessage("Saved Project", false);
                return data;
            }
            catch
            {
                SetMessage("Failed to save Project", false);
                throw;
            }
        }

        public void NewProject()
        {
            if (Format == null)
                return;

            Format.ResetProject();
            SetMessage("Reset Project", false);
        }


        public void ExportLanguage(string filepath)
        {
            if (Format == null)
            {
                SetMessage("No Format Loaded!", true);
                return;
            }

            Format.ExportLanguage(filepath);
            SetMessage("Exported Language Files", false);
        }

        public void ImportLanguage(string filepath)
        {
            if(Format == null)
            {
                SetMessage("No Format Loaded!", true);
                return;
            }

            Format.ImportLanguage(filepath);
            SetMessage("Imported Language Files", false);
        }

    }
}
