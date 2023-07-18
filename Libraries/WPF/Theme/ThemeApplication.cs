using SCR.Tools.WPF.Theme.Styles;
using System;
using System.Windows;

namespace SCR.Tools.WPF.Theme
{
    public abstract class ThemeApplication : Application
    {
        private Skin _skin;
        private ResourceDictionary? _skinResources;
        private ResourceDictionary? _themeResources;
        private readonly AppResources _appResources;

        public abstract ThemeAppSettings Settings { get; }

        public Skin Skin
        {
            get => _skin;
            set
            {
                if (_skin == value)
                    return;

                _skin = value;
                Settings.Skin = value;

                ReloadSkin();
            }
        }

        public double AppFontSize
        {
            get => _appResources.AppFontSize;
            set => _appResources.AppFontSize = value;
        }

        public ThemeApplication() : base()
        {
            _skin = Settings.Skin;

            ReloadSkin();

            _appResources = new(this);
            Resources.MergedDictionaries.Add(_appResources);
        }

        public void ReloadSkin()
        {
            if (_skinResources != null)
            {
                Resources.MergedDictionaries.Remove(_skinResources);
                _skinResources = null;


            }

            if (_themeResources != null)
            {
                Resources.MergedDictionaries.Remove(_themeResources);
                _themeResources = null;
            }

            _skinResources = _skin.GetResources();
            Resources.MergedDictionaries.Insert(0, _skinResources);

            // to reload the theme, we reload the entire theme resources
            Uri themeUri = new("/SCR.Tools.WPF;component/Theme/Style.xaml", UriKind.RelativeOrAbsolute);
            _themeResources = new() { Source = themeUri };
            Resources.MergedDictionaries.Insert(1, _themeResources);
        }

    }
}
