using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Linq;

namespace SCRCommon.Wpf
{
    /// <summary>
    /// The different window themes to choose from
    /// </summary>
    public enum Theme
    {
        Dark,
        Light
    }
    public static class ThemeExentions
    {
        public static ResourceDictionary GetResourceDictionary(this Theme theme)
            => new() { Source = theme.GetUri() };

        public static Uri GetUri(this Theme theme)
            => new($"/SCRCommon;component/WpfStyles/{theme}.xaml", UriKind.RelativeOrAbsolute);
    }

    /// <summary>
    /// Base class for a darkmode window frame that sets all necessary events for the window frame to function
    /// </summary>
    public partial class BaseStyle : ResourceDictionary
    {
        private static ResourceDictionary _themeRd;

        private static BaseStyle _themeBaseStyle;

        private static readonly HashSet<Application> _initiatedApps;

        /// <summary>
        /// Gets, sets and updates the window theme accordingly
        /// </summary>
        public static Theme Theme
        {
            get => Properties.Settings.Default.Theme;
            set
            {
                Properties.Settings.Default.Theme = value;
                _themeRd = value.GetResourceDictionary();
                _themeBaseStyle = new();

                foreach(var app in _initiatedApps)
                {
                    var rd = FindBaseStyle(app.Resources).MergedDictionaries;
                    if(rd == null)
                    {
                        app.Resources.MergedDictionaries.Add(_themeBaseStyle);
                    }
                    else
                    {
                        int index = rd.IndexOf(
                            rd.First(x => x.GetType() == typeof(BaseStyle))
                        );
                        rd[index] = _themeBaseStyle;    
                    }
                }

                Properties.Settings.Default.Save();
            }
        }

        static BaseStyle()
        {
            _initiatedApps = new();
            _themeRd = Properties.Settings.Default.Theme.GetResourceDictionary();
            _themeBaseStyle = new();
        }

        private static ResourceDictionary FindBaseStyle(ResourceDictionary resources)
        {
            foreach(ResourceDictionary rd in resources.MergedDictionaries)
            {
                if(rd.GetType() == typeof(BaseStyle))
                    return resources;
                
                ResourceDictionary bs = FindBaseStyle(rd);
                if(bs != null)
                    return bs;
            }
            return null;
        }

        public static void Init(Application app)
        {
            if(_initiatedApps.Contains(app))
                return;

            ResourceDictionary rd = FindBaseStyle(app.Resources);
            if(rd == null)
            {
                rd = app.Resources;
                rd.MergedDictionaries.Add(_themeBaseStyle);
            }

            _initiatedApps.Add(app);
            app.Exit += (o, e) => _initiatedApps.Remove(app);
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public BaseStyle()
        {
            MergedDictionaries.Add(_themeRd);
            InitializeComponent();
        }
    }

    public static class WPFExtensions
    {
        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            //get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            //we've reached the end of the tree
            if(parentObject == null)
                return null;

            //check if the parent matches the type we're looking for
            if(parentObject is T parent)
                return parent;
            else
                return FindParent<T>(parentObject);
        }
    }
}
