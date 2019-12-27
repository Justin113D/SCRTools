using Microsoft.Win32;
using SCRCommon.Viewmodels;
using SCRCommon.WpfStyles;
using SCRLanguageEditor.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCRLanguageEditor.Viewmodel
{
    /// <summary>
    /// Viewmodel for the settings
    /// </summary>
    public class VM_Settings : BaseViewModel
    {
        /// <summary>
        /// The settings object, which the viewmodel accesses and modifies
        /// </summary>
        private readonly Settings settings;

        /// <summary>
        /// Gets a list of themes to select from
        /// </summary>
        public List<Theme> Themes
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
            get
            {
                return settings.DefaultFormatPath;
            }
            set
            {
                settings.DefaultFormatPath = value;
            }
        }

        /// <summary>
        /// Used for reverting the default format path when canceling
        /// </summary>
        private string oldDefaultFormatPath;

        /// <summary>
        /// Redirects to the window theme
        /// </summary>
        public Theme WindowTheme
        {
            get
            {
                return settings.WindowTheme;
            }
            set
            {
                settings.WindowTheme = value;
            }
        }

        /// <summary>
        /// Used for reverting the window theme when cancelling
        /// </summary>
        private Theme oldWindowTheme;

        /// <summary>
        /// When the window is being closed via Ok or Cancel button
        /// </summary>
        private bool closing;

        /// <summary>
        /// The command to select a default format path via dialog
        /// </summary>
        public RelayCommand Cmd_SetDefaultPath { get; private set; }

        /// <summary>
        /// The Save command
        /// </summary>
        public RelayCommand<SettingsWindow> Cmd_Save { get; private set; }

        /// <summary>
        /// The cancel command
        /// </summary>
        public RelayCommand<SettingsWindow> Cmd_Cancel { get; private set; }

        /// <summary>
        /// Base constructor; Sets up the viewmodel
        /// </summary>
        /// <param name="settings">The settings object for the viewmodel</param>
        public VM_Settings(Settings settings)
        {
            this.settings = settings;
            Cmd_SetDefaultPath = new RelayCommand(SelectFormatPath);
            Cmd_Save = new RelayCommand<SettingsWindow>(Save);
            Cmd_Cancel = new RelayCommand<SettingsWindow>(Cancel);
            oldDefaultFormatPath = DefaultFormatPath;
            oldWindowTheme = WindowTheme;
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
            settings.Save();
            oldDefaultFormatPath = settings.DefaultFormatPath;
            oldWindowTheme = settings.WindowTheme;
            closing = true;
            w.Close();
        }

        /// <summary>
        /// Reverts the settings
        /// </summary>
        /// <param name="w">The window that needs to be closed upon closing</param>
        private void Cancel(SettingsWindow w)
        {
            DefaultFormatPath = oldDefaultFormatPath;
            WindowTheme = oldWindowTheme;
            closing = true;
            w.Close();
        }

        /// <summary>
        /// Reverts options when closing the window
        /// </summary>
        public void Close()
        {
            if(!closing)
            {
                DefaultFormatPath = oldDefaultFormatPath;
                WindowTheme = oldWindowTheme;
            }
            closing = false;
        }
    }
}
