using SCR.Tools.WPF.Theme;
using System.Configuration;
using System.Diagnostics;

namespace SCR.Tools.WPF.Properties
{
    internal sealed class Settings : ApplicationSettingsBase
    {

        private static readonly Settings defaultInstance
            = (Settings)Synchronized(new Settings());

        public static Settings Default => defaultInstance;

        [UserScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("Dark")]
        public Skin Skin
        {
            get => (Skin)this[nameof(Skin)];
            set
            {
                if (value == Skin)
                    return;
                this[nameof(Skin)] = value;
            }
        }
    }
}
