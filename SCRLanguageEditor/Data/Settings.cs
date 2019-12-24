using SCRCommon.WpfStyles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SCRLanguageEditor.Data
{
    public class Settings
    {
        private string defaultFormatPath = "LanguageFiles/Format.xml";

        public string DefaultFormatPath
        {
            get
            {
                return defaultFormatPath;
            }
            set
            {
                if (value == defaultFormatPath) return;
                if(File.Exists(value))
                {
                    defaultFormatPath = value;
                }
            }
        }

        public Theme WindowTheme
        {
            get
            {
                return BaseWindowStyle.WindowTheme;
            }
            set
            {
                BaseWindowStyle.WindowTheme = value;
            }
        }

        public Settings()
        {
            Load();
        }

        public void Load()
        {
            if(File.Exists("Settings.settings"))
            {
                string[] lines = File.ReadAllLines("Settings.settings");
                foreach(string l in lines)
                {
                    // Getting the default format path
                    if(l.StartsWith("DefaultFormatPath="))
                    {
                        DefaultFormatPath = l.Substring(19);
                    }
                    // Getting the theme
                    else if(l.StartsWith("Theme="))
                    {
                        if(Enum.TryParse(l.Substring(6), out Theme theme))
                        {
                            WindowTheme = theme;
                        }
                    }
                    // Not a valid setting
                    else
                    {
                        
                    }
                }
            }
        }

        public void Save()
        {
            string[] lines =
            {
                "DefaultFormatPath=" + defaultFormatPath,
                "Theme=" + WindowTheme
            };

            File.WriteAllLines("Settings.settings", lines);
        }
    }
}
