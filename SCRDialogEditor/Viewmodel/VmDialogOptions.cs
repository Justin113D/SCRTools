using Microsoft.Win32;
using SCRCommon.Viewmodels;
using SCRDialogEditor.Data;
using System;
using System.Collections.Generic;
using System.Windows;

namespace SCRDialogEditor.Viewmodel
{
    public class VmDialogOptions : FileBaseViewModel
    {
        public RelayCommand Cmd_SetDialogOptionsPath
            => new(SetDialogOptionsPath);

        public DialogOptions Data { get; private set; }

        public VmNodeOptions VMCharacterOptions { get; private set; }

        public VmNodeOptions VMExpressionOptions { get; private set; }

        public VmNodeIcons VMNodeIcons { get; private set; }

        public string DialogOptionsPath
        {
            get => Properties.Settings.Default.DialogOptionsPath;
            set => Properties.Settings.Default.DialogOptionsPath = value;
        }

        public override string FileFilter
            => "Json File |*.json";

        public override string FileTypeName
            => "Dialog Options";

        public override ChangeTracker PinTracker
            => null;

        public VmDialogOptions()
        {
            Data = new DialogOptions();
            if(!string.IsNullOrWhiteSpace(DialogOptionsPath))
            {
                if(Load(DialogOptionsPath))
                    LoadedFilePath = DialogOptionsPath;
            }
            else
            {
                VMCharacterOptions = new VmNodeOptions(Data.CharacterOptions);
                VMExpressionOptions = new VmNodeOptions(Data.ExpressionOptions);
                VMNodeIcons = new VmNodeIcons(Data.NodeIcons, this);
            }
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
                Data = DialogOptions.ReadFromFile(path);
            }
            catch(Exception e)
            {
                MessageBox.Show("An error occured while loading the dialog options: " + e.GetType().Name + "\n " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            VMCharacterOptions = new VmNodeOptions(Data.CharacterOptions);
            VMExpressionOptions = new VmNodeOptions(Data.ExpressionOptions);
            VMNodeIcons = new VmNodeIcons(Data.NodeIcons, this);
            return true;
        }

        public override void Save(string path)
        {
            if(path != LoadedFilePath)
            {
                Dictionary<VmNodeIcon, string> fullPaths = new();
                foreach(VmNodeIcon ni in VMNodeIcons.Icons)
                {
                    if(ni.FullFilePath != null)
                        fullPaths.Add(ni, ni.FullFilePath);
                }

                LoadedFilePath = path;

                foreach(var pair in fullPaths)
                    pair.Key.FullFilePath = pair.Value;
            }

            Data.SaveToFile(path);
        }

        public override bool ResetConfirmation() => true;

        public override void Reset()
        {
            Data = new DialogOptions();
            VMCharacterOptions = new VmNodeOptions(Data.CharacterOptions);
            VMExpressionOptions = new VmNodeOptions(Data.ExpressionOptions);
            VMNodeIcons = new VmNodeIcons(Data.NodeIcons, this);
        }
    }
}
