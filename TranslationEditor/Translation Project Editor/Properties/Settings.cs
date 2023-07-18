using SCR.Tools.WPF.Theme;
using System.Configuration;
using System.Diagnostics;

namespace SCR.Tools.TranslationEditor.ProjectEditor.Properties
{
    internal class Settings : ThemeAppSettings
    {
        private static readonly Settings _defaultInstance = (Settings)Synchronized(new Settings());

        public static Settings Default => _defaultInstance;

        [UserScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("")]
        public string DefaultFormatPath
        {
            get => (string)this[nameof(DefaultFormatPath)];
            set => this[nameof(DefaultFormatPath)] = value;
        }
    }
}
