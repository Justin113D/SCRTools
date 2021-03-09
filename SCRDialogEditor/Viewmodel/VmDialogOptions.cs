using Microsoft.Win32;
using SCRCommon.Viewmodels;
using SCRDialogEditor.Data;
using System;
using System.Windows;

namespace SCRDialogEditor.Viewmodel
{
    public class VmDialogOptions : FileBaseViewModel
    {
        public RelayCommand Cmd_SetDialogOptionsPath 
            => new(SetDialogOptionsPath);

        public DialogOptions DialogOptions { get; private set; }

        public VmNodeOptions VMCharacterOptions { get; private set; }

        public VmNodeOptions VMExpressionOptions { get; private set; }

        public string DialogOptionsPath
        {
            get => Properties.Settings.Default.DialogOptionsPath;
            set => Properties.Settings.Default.DialogOptionsPath = value;
        }

        public override string FileFilter 
            => "Json File |*.json";

        public override string FileTypeName 
            => "Dialog Options";

        public VmDialogOptions()
        {
            DialogOptions = new DialogOptions();
            if(!string.IsNullOrWhiteSpace(DialogOptionsPath))
                if(Load(DialogOptionsPath))
                    LoadedFilePath = DialogOptionsPath;

            VMCharacterOptions = new VmNodeOptions(DialogOptions.CharacterOptions);
            VMExpressionOptions = new VmNodeOptions(DialogOptions.ExpressionOptions);
        }

        private void SetDialogOptionsPath()
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Title = "Select dialog options file",
                Filter = "json file (*.json)|*.json"
            };
            if(ofd.ShowDialog() == true)
            {
                DialogOptionsPath = ofd.FileName;
                Properties.Settings.Default.Save();
            }
        }

        public override bool Load(string path)
        {
            try
            {
                DialogOptions = DialogOptions.ReadFromFile(path);
            }
            catch(Exception e)
            {
                MessageBox.Show("An error occured while loading the dialog options: " + e.GetType().Name + "\n " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            VMCharacterOptions = new VmNodeOptions(DialogOptions.CharacterOptions);
            VMExpressionOptions = new VmNodeOptions(DialogOptions.ExpressionOptions);
            return true;
        }

        public override void Save(string path)
        {
            DialogOptions.SaveToFile(path);
        }

        public override bool ResetConfirmation() => true;

        public override void Reset()
        {
            DialogOptions = new DialogOptions();
            VMCharacterOptions = new VmNodeOptions(DialogOptions.CharacterOptions);
            VMExpressionOptions = new VmNodeOptions(DialogOptions.ExpressionOptions);
        }
    }
}
