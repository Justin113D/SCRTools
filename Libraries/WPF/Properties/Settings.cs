using System.Diagnostics;
using System.Configuration;

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
        public Styling.Theme Theme
        {
            get => (Styling.Theme)this[nameof(Theme)];
            set
            {
                if(value == Theme)
                    return;
                this[nameof(Theme)] = value;
                Styling.BaseStyle.Theme = value;
            }
        }
    }
}
