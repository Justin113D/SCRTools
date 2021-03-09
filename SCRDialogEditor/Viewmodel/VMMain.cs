using SCRCommon.Viewmodels;
using SCRDialogEditor.Data;
using SCRDialogEditor.XAML;

namespace SCRDialogEditor.Viewmodel
{
    public class VmMain : FileBaseViewModel
    {
        /// <summary>
        /// Command for the "settings" button
        /// </summary>
        public RelayCommand Cmd_Settings 
            => new(OpenSettings);

        public RelayCommand Cmd_DialogOptions 
            => new(OpenDialogOptions);

        public VmGrid Grid { get; }

        public VmDialogOptions DialogOptions { get; private set; }

        public VmSettings Settings { get; }

        public override string FileFilter 
            => "Json File (*.json)|*.json";

        public override string FileTypeName 
            => "Json";

        public VmMain()
        {
            Settings = new VmSettings(this);
            DialogOptions = new VmDialogOptions();
            Grid = new VmGrid(this);
        }

        /// <summary>
        /// Creates a settings dialog
        /// </summary>
        private void OpenSettings() => new WndSettings(Settings).ShowDialog();

        private void OpenDialogOptions() => new WndDialogOptions(DialogOptions).ShowDialog();

      
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

            Grid.SetDialog(data);
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
            Grid.SetDialog(new Dialog());
        }
    }
}
