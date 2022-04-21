using System.Collections.Generic;
using System.Windows;

namespace SCR.Tools.WPF.Styling
{
    /// <summary>
    /// Base class for a themed window frame that sets all necessary events for the window frame to function
    /// </summary>
    public partial class BaseStyle : ResourceDictionary
    {
        /// <summary>
        /// Active theme resource dictionary
        /// </summary>
        private static ResourceDictionary _themeRd;

        private static BaseStyle _themeBaseStyle;

        private static readonly HashSet<Application> _initiatedApps;

        /// <summary>
        /// Updates the window theme accordingly at runtime
        /// </summary>
        public static Theme Theme
        {
            get => Properties.Settings.Default.Theme;
            set
            {
                Properties.Settings.Default.Theme = value;
                _themeRd = value.GetResourceDictionary();
                _themeBaseStyle = new();

                foreach (var app in _initiatedApps)
                {
                    var rd = FindBaseStyle(app.Resources, out BaseStyle? bs)?.MergedDictionaries;
                    if (rd == null)
                    {
                        app.Resources.MergedDictionaries.Add(_themeBaseStyle);
                    }
                    else
                    {
                        int index = rd.IndexOf(bs);
                        rd[index] = _themeBaseStyle;
                    }
                }

                Properties.Settings.Default.Save();
            }
        }

        static BaseStyle()
        {
            _themeRd = Properties.Settings.Default.Theme.GetResourceDictionary();
            _themeBaseStyle = new();
            _initiatedApps = new();
        }

        /// <summary>
        /// Find the merged dictionary that holds a base style instance
        /// </summary>
        /// <param name="resources">Resource dictionary containing the base style</param>
        /// <param name="style">Base style inside the resources</param>
        /// <returns></returns>
        private static ResourceDictionary? FindBaseStyle(ResourceDictionary resources, out BaseStyle? style)
        {
            foreach (ResourceDictionary rd in resources.MergedDictionaries)
            {
                if (rd is BaseStyle baseStyle)
                {
                    style = baseStyle;
                    return resources;
                }

                ResourceDictionary? bs = FindBaseStyle(rd, out style);
                if (bs != null)
                    return bs;
            }

            style = null;
            return null;
        }

        /// <summary>
        /// Initialize base style for an application
        /// </summary>
        /// <param name="app"></param>
        public static void Init(Application app)
        {
            // check if a style was already initiated for the application
            if (_initiatedApps.Contains(app))
                return;

            // Find out if the app was instantiated with a base style already.
            // if none is found, create and add one
            ResourceDictionary? rd = FindBaseStyle(app.Resources, out _);
            if (rd == null)
            {
                rd = app.Resources;
                rd.MergedDictionaries.Add(_themeBaseStyle);
            }

            // add the app to the initiated apps
            _initiatedApps.Add(app);

            // add an event that removes the app from the initiated apps upon exiting
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
}
