using System;
using System.Windows;

namespace SCR.Tools.WPF.Theme
{
    public class ThemeApplication : Application
    {
        //private readonly static ResourceDictionary _themeResources;

        private Skin _skin;

        /// <summary>
        /// Active theme resource dictionary
        /// </summary>
        private ResourceDictionary? _skinResources;
        private ResourceDictionary? _themeResources;

        public Skin Skin
        {
            get => _skin;
            set
            {
                if (_skin == value)
                    return;

                _skin = value;

                var settings = WPF.Properties.Settings.Default;
                settings.Skin = value;
                settings.Save();

                ReloadSkin();
            }
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

        public ThemeApplication() : base()
        {
            _skin = WPF.Properties.Settings.Default.Skin;
            ReloadSkin();
        }
    }
}
