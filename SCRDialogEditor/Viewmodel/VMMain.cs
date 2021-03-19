using SCRCommon.Viewmodels;
using SCRDialogEditor.Data;
using SCRDialogEditor.XAML;

namespace SCRDialogEditor.Viewmodel
{
    public class VmMain : FileBaseViewModel
    {
        #region Commands

        /// <summary>
        /// Command for the "settings" button
        /// </summary>
        public RelayCommand Cmd_Settings
            => new(OpenSettings);

        /// <summary>
        /// Command for the "dialog options" button
        /// </summary>
        public RelayCommand Cmd_DialogOptions
            => new(OpenDialogOptions);

        #endregion

        public VmGrid Grid { get; private set; }

        /// <summary>
        /// Dialog options
        /// </summary>
        public VmDialogOptions DialogOptions { get; }

        /// <summary>
        /// Settings
        /// </summary>
        public VmSettings Settings { get; }

        public override string FileFilter
            => "Json File (*.json)|*.json";

        public override string FileTypeName
            => "Json";

        public VmMain()
        {
            Settings = new(this);
            DialogOptions = new();
            Grid = new(this, new());
        }


        /// <summary>
        /// Creates a settings dialog
        /// </summary>
        private void OpenSettings()
            => new WndSettings(Settings).ShowDialog();

        /// <summary>
        /// Opens the dialog options dialog
        /// </summary>
        private void OpenDialogOptions()
            => new WndDialogOptions(DialogOptions).ShowDialog();


        public override bool Load(string path)
        {
            Dialog data;
            try
            {
                data = Dialog.LoadFromFile(path);
            }
            catch
            {
                return false;
            }

            Grid = new VmGrid(this, data);
            return true;
        }

        public override void Save(string path)
        {
            Grid.Data.SaveToFile(path);
        }

        public override bool ResetConfirmation()
            => true;

        public override void Reset()
        {
            Grid = new(this, new());
        }
    }
}
