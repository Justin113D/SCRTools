using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCR.Tools.TranslationEditor.WPF.Properties
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
