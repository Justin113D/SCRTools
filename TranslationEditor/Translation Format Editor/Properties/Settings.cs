using SCR.Tools.WPF.Theme;
using System.Configuration;
using System.Diagnostics;

namespace SCR.Tools.TranslationEditor.FormatEditor.Properties
{
    internal class Settings : ThemeAppSettings
    {
        private static readonly Settings _defaultInstance = (Settings)Synchronized(new Settings());

        public static Settings Default => _defaultInstance;

        [UserScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("True")]
        public bool JsonIndenting
        {
            get => (bool)this[nameof(JsonIndenting)];
            set => this[nameof(JsonIndenting)] = value;
        }
    }
}
