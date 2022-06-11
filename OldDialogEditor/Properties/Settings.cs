using System.Configuration;
using System.Diagnostics;

namespace SCR.Tools.DialogEditor.Properties
{
    internal sealed partial class Settings : ApplicationSettingsBase
    {

        private static Settings defaultInstance = (Settings)Synchronized(new Settings());

        public static Settings Default => defaultInstance;

        

        [UserScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("")]
        public string DialogOptionsPath
        {
            get => (string)this[nameof(DialogOptionsPath)];
            set => this[nameof(DialogOptionsPath)] = value;
        }

        [UserScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("15")]
        public double Fontsize
        {
            get => ((double)(this[nameof(Fontsize)]));
            set => this[nameof(Fontsize)] = value;
        }

        [UserScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("true")]
        public bool HighlightPortraitAvailable
        {
            get => ((bool)(this[nameof(HighlightPortraitAvailable)]));
            set => this[nameof(HighlightPortraitAvailable)] = value;
        }
    }
}
