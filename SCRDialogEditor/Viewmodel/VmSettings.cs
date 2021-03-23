using SCRCommon.Viewmodels;
using SCRCommon.WpfStyles;
using System;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable CS1822

namespace SCRDialogEditor.Viewmodel
{
    /// <summary>
    /// Viewmodel for the settings
    /// </summary>
    public class VmSettings : BaseViewModel
    {
        private readonly VmMain _mainViewModel;

        /// <summary>
        /// Gets a list of themes to select from
        /// </summary>
        public static List<Theme> Themes => Enum.GetValues(typeof(Theme)).Cast<Theme>().ToList();

        /// <summary>
        /// Redirects to the window theme
        /// </summary>
        public Theme WindowTheme
        {
            get => Properties.Settings.Default.WindowTheme;
            set
            {
                Properties.Settings.Default.WindowTheme = value;
                BaseStyle.WindowTheme = value;
            }
        }

        /// <summary>
        /// The Save command
        /// </summary>
        public RelayCommand<WndSettings> Cmd_Save { get; private set; }

        /// <summary>
        /// Base constructor; Sets up the viewmodel
        /// </summary>
        /// <param name="settings">The settings object for the viewmodel</param>
        public VmSettings(VmMain mainViewModel)
        {
            _mainViewModel = mainViewModel;
            Cmd_Save = new RelayCommand<WndSettings>(Save);
            BaseStyle.WindowTheme = WindowTheme;
        }

        /// <summary>
        /// Saves the settings
        /// </summary>
        /// /// <param name="w">The window that needs to be closed upon saving</param>
        private void Save(WndSettings w)
        {
            Properties.Settings.Default.Save();
            w.Close();
        }

    }
}
