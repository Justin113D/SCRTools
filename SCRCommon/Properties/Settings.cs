using System.Diagnostics;
using System.Configuration;

namespace SCRCommon.Properties
{
    internal sealed partial class Settings : ApplicationSettingsBase
    {

        private static Settings defaultInstance = (Settings)Synchronized(new Settings());

        public static Settings Default => defaultInstance;

        [UserScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("Dark")]
        public Wpf.Theme Theme
        {
            get => (Wpf.Theme)this[nameof(Theme)];
            set
            {
                if(value == Theme)
                    return;
                this[nameof(Theme)] = value;
                Wpf.BaseStyle.Theme = value;
            }
        }
    }
}
