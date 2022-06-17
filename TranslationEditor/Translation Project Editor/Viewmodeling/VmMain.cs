using SCR.Tools.TranslationEditor.Data;
using SCR.Tools.UndoRedo;
using SCR.Tools.Viewmodeling;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;
using System.IO;

namespace SCR.Tools.TranslationEditor.ProjectEditor.Viewmodeling
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
        public RelayCommand CmdUndo
            => new(Undo);

        /// <summary>
        /// Redo Command
        /// </summary>
        public RelayCommand CmdRedo
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
            if(UndoChange())
            {
                SetMessage("Performed Undo", false);
            }
        }

        /// <summary>
        /// Performs redo on the Change Tracker and displays the action to the user
        /// </summary>
        private void Redo()
        {
            if(RedoChange())
            {
                SetMessage("Performed Redo", false);
            }
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
                HeaderNode headerNode = JsonFormatHandler.ReadFormat(format);

                Format = new(headerNode);
                ProjectTracker.Reset();
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

            BeginChangeGroup();

            try
            {
                Format.Header.LoadProject(data);
            }
            catch
            {
                EndChangeGroup(true);
                SetMessage("Failed to load Project", true);
                throw;
            }

            SetMessage("Loaded Project", false);
            EndChangeGroup();
            ResetTracker();
            Format.RefreshNodeValues();
        }


        public string WriteProject()
        {
            if (Format == null)
                return "";

            try
            {
                string data = Format.Header.CompileProject();
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

            Format.Header.ResetAllStrings();
            Format.RefreshNodeValues();
            ResetTracker();
            SetMessage("Reset Project", false);
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

        public void ImportLanguage(string filepath)
        {
            if (Format == null)
            {
                SetMessage("No Format Loaded!", true);
                return;
            }

            string values = File.ReadAllText(filepath);

            string keyFilePath = Path.ChangeExtension(filepath, "langkey");
            string keys = File.ReadAllText(keyFilePath);

            BeginChangeGroup();

            try
            {
                Format.Header.ImportLanguageData(keys, values);
            }
            catch
            {
                EndChangeGroup(true);
                throw;
            }

            EndChangeGroup();
            ResetTracker();
            Format.RefreshNodeValues();

            SetMessage("Imported Language Files", false);
        }

    }
}
