using System.Configuration;
using System.Diagnostics;

namespace SCR.Tools.TranslationEditor.ProjectEditor.Properties
{
    internal class Settings : ApplicationSettingsBase
    {
        private static readonly Settings _defaultInstance = (Settings)Synchronized(new Settings());

        public static Settings Default => _defaultInstance;

        [UserScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("15")]
        public double Fontsize
        {
            get => (double)this[nameof(Fontsize)];
            set => this[nameof(Fontsize)] = value;
        }

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
