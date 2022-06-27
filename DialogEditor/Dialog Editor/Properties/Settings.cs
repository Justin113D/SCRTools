using System.Configuration;
using System.Diagnostics;

namespace SCR.Tools.Dialog.Editor.Properties
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
        [DefaultSettingValue("True")]
        public bool JsonIndenting
        {
            get => (bool)this[nameof(JsonIndenting)];
            set => this[nameof(JsonIndenting)] = value;
        }

        [UserScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("")]
        public string DefaultDialogOptionsPath
        {
            get => (string)this[nameof(DefaultDialogOptionsPath)];
            set => this[nameof(DefaultDialogOptionsPath)] = value;
        }
    }
}
