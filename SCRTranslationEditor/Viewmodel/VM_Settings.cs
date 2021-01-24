using Microsoft.Win32;
using SCRCommon.Viewmodels;
using SCRCommon.WpfStyles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SCRTranslationEditor.Viewmodel
{
    /// <summary>
    /// Viewmodel for the settings
    /// </summary>
    public class VM_Settings : BaseViewModel
    {
        private readonly VM_Main _mainViewModel;

        /// <summary>
        /// Gets a list of themes to select from
        /// </summary>
        public static List<Theme> Themes
        {
            get
            {
                return Enum.GetValues(typeof(Theme)).Cast<Theme>().ToList();
            }
        }

        /// <summary>
        /// Redirects to the default format path
        /// </summary>
        public string DefaultFormatPath
        {
            get => Properties.Settings.Default.DefaultFormatPath;
            set
            {
                if(string.IsNullOrWhiteSpace(value))
                {
                    Properties.Settings.Default.DefaultFormatPath = "";
                    return;
                }

                if(!Path.IsPathFullyQualified(value))
                    return;

                Properties.Settings.Default.DefaultFormatPath = Path.ChangeExtension(value, "json");
            }
        }

        public bool DisplayOriginal
        {
            get => Properties.Settings.Default.DisplayOriginal;
            set
            {
                Properties.Settings.Default.DisplayOriginal = value;
                OnPropertyChanged(nameof(DisplayOriginalReal));
            }
        }

        public bool DisplayOriginalReal => DisplayOriginal && !DevMode;

        /// <summary>
        /// Redirects to the window theme
        /// </summary>
        public Theme WindowTheme
        {
            get
            {
                return Properties.Settings.Default.WindowTheme;
            }
            set
            {
                Properties.Settings.Default.WindowTheme = value;
                BaseWindowStyle.WindowTheme = value;
            }
        }


        /// <summary>
        /// Whether the application currently is in developer mode (allows to edit the format)
        /// </summary>
        public bool DevMode
        {
            get => Properties.Settings.Default.DevMode;
            set => _mainViewModel.Format.ChangedMode(value);
        }

        public bool JsonIndenting
        {
            get => Properties.Settings.Default.JsonIndenting;
            set => Properties.Settings.Default.JsonIndenting = value;
        }

        /// <summary>
        /// The command to select a default format path via dialog
        /// </summary>
        public RelayCommand Cmd_SetDefaultPath { get; private set; }

        /// <summary>
        /// The Save command
        /// </summary>
        public RelayCommand<SettingsWindow> Cmd_Save { get; private set; }

        /// <summary>
        /// Base constructor; Sets up the viewmodel
        /// </summary>
        /// <param name="settings">The settings object for the viewmodel</param>
        public VM_Settings(VM_Main mainViewModel)
        {
            _mainViewModel = mainViewModel;
            Cmd_SetDefaultPath = new RelayCommand(SelectFormatPath);
            Cmd_Save = new RelayCommand<SettingsWindow>(Save);
            BaseWindowStyle.WindowTheme = WindowTheme;
        }

        private void SelectFormatPath()
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Title = "Select format file",
                Filter = "Xml file (*.xml)|*.xml"
            };
            if(ofd.ShowDialog() == true)
            {
                DefaultFormatPath = ofd.FileName;
            }
        }

        /// <summary>
        /// Saves the settings
        /// </summary>
        /// /// <param name="w">The window that needs to be closed upon saving</param>
        private void Save(SettingsWindow w)
        {
            Properties.Settings.Default.Save();
            w.Close();
        }

    }
}
