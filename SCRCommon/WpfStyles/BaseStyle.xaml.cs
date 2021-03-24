using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace SCRCommon.WpfStyles
{
    /// <summary>
    /// The different window themes to choose from
    /// </summary>
    public enum Theme
    {
        Dark,
        Light
    }

    /// <summary>
    /// Base class for a darkmode window frame that sets all necessary events for the window frame to function
    /// </summary>
    public partial class BaseStyle : ResourceDictionary
    {
        /// <summary>
        /// The current active window theme. Default is dark
        /// </summary>
        private static Theme _windowTheme = Theme.Dark;

        private static readonly Dictionary<Theme, BaseStyle> _loadedStyles;


        /// <summary>
        /// Gets, sets and updates the window theme accordingly
        /// </summary>
        public static Theme WindowTheme
        {
            get => _windowTheme;
            set
            {
                if(_windowTheme == value)
                    return;
                _windowTheme = value;

                var r = Application.Current.Resources;

                if(r.GetType() == typeof(BaseStyle))
                {
                    Application.Current.Resources = _loadedStyles[value];
                    return;
                }

                for(int i = 0; i < r.MergedDictionaries.Count; i++)
                {
                    var m = r.MergedDictionaries[i];
                    if(m.GetType() == typeof(BaseStyle))
                    {
                        r.MergedDictionaries[i] = _loadedStyles[value];
                        break;
                    }
                }

            }
        }

        static BaseStyle()
        {
            _loadedStyles = new();

            foreach(Theme theme in Enum.GetValues<Theme>())
                _loadedStyles.Add(theme, new BaseStyle(theme));
        }

        public static void Init()
        {
            var r = Application.Current.Resources;

            if(r.GetType() == typeof(BaseStyle))
            {
                Application.Current.Resources = _loadedStyles[_windowTheme];
                return;
            }

            for(int i = 0; i < r.MergedDictionaries.Count; i++)
            {
                var m = r.MergedDictionaries[i];
                if(m.GetType() == typeof(BaseStyle))
                {
                    r.MergedDictionaries[i] = _loadedStyles[_windowTheme];
                    break;
                }
            }

            r.MergedDictionaries.Add(new BaseStyle());
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public BaseStyle()
        {
            MergedDictionaries.Add(_loadedStyles[_windowTheme].MergedDictionaries[0]);
            InitializeComponent();
        }

        private BaseStyle(Theme theme)
        {
            MergedDictionaries.Add(new() { Source = new($"/SCRCommon;component/WpfStyles/{theme}.xaml", UriKind.RelativeOrAbsolute) });
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
