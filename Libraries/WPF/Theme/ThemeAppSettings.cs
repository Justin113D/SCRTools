using System.Configuration;
using System.Diagnostics;

namespace SCR.Tools.WPF.Theme
{
    public abstract class ThemeAppSettings : ApplicationSettingsBase
    {
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

        [UserScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("15")]
        public double Fontsize
        {
            get => (double)this[nameof(Fontsize)];
            set => this[nameof(Fontsize)] = value;
        }

    }
}
