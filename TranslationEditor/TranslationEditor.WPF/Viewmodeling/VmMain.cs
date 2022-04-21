using SCR.Tools.TranslationEditor.Data;
using SCR.Tools.UndoRedo;
using SCR.Tools.Viewmodeling;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SCR.Tools.TranslationEditor.WPF.Viewmodeling
{
    public class VmMain : BaseViewModel
    {
        public ChangeTracker ProjectTracker { get; }

        public VmProject? Format { get; private set; }

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


        public RelayCommand Cmd_Undo
            => new(Undo);

        public RelayCommand Cmd_Redo
            => new(Redo);


        public VmMain()
        {
            ProjectTracker = new();
            ProjectTracker.Use();
        }

        private void SetMessage(string value, bool warning)
        {
            if (DisplayMessage > 0)
            {
                DisplayMessage = 0;
                Message = "";
            }

            Message = value;
            DisplayMessage = warning ? 2 : 1;
        }

        private void Undo()
        {
            ProjectTracker.Undo();

            SetMessage("Performed Undo", false);
        }

        private void Redo()
        {
            ProjectTracker.Redo();

            SetMessage("Performed Redo", false);
        }

        public void LoadFormat(string format)
        {
            ProjectTracker.ResetOnNextChange = true;
            HeaderNode headerNode = JsonFormatHandler.ReadFormat(format);
            Format = new(headerNode, ProjectTracker);

            OnPropertyChanged(nameof(FormatLoaded));

            SetMessage("Loaded Format", false);
        }

        public void LoadProject(string data)
        {
            if (Format == null)
                return;
            Format.LoadProject(data);
            SetMessage("Loaded Project", false);
        }

        public string WriteProject()
        {
            if (Format == null)
                return "";
            string data = Format.WriteProject();
            SetMessage("Saved Project", false);
            return data;
        }

        public void NewProject()
        {
            if (Format == null)
                return;
            Format.NewProject();
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
