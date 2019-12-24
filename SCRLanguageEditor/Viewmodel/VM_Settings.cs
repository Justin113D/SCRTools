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
    public class VM_Settings : BaseViewModel
    {

        private readonly Settings settings;

        public List<Theme> Themes
        {
            get
            {
                return Enum.GetValues(typeof(Theme)).Cast<Theme>().ToList();
            }
        }
        
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

        private string oldDefaultFormatPath;

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

        private Theme oldWindowTheme;

        public RelayCommand Cmd_Save { get; private set; }
        public RelayCommand Cmd_Cancel { get; private set; }

        public VM_Settings(Settings settings)
        {
            this.settings = settings;
            Cmd_Save = new RelayCommand(Save);
            Cmd_Cancel = new RelayCommand(Cancel);
            oldDefaultFormatPath = DefaultFormatPath;
            oldWindowTheme = WindowTheme;
        }

        private void Save()
        {
            settings.Save();
            oldDefaultFormatPath = settings.DefaultFormatPath;
            oldWindowTheme = settings.WindowTheme;
        }

        private void Cancel()
        {
            DefaultFormatPath = oldDefaultFormatPath;
            WindowTheme = oldWindowTheme;
        }
    }
}
