using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace SCRCommon.WpfStyles
{
    /// <summary>
    /// Base class for a darkmode window frame that sets all necessary events for the window frame to function
    /// </summary>
    public partial class BaseWindowStyle : ResourceDictionary
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
        /// The current active window theme. Default is dark
        /// </summary>
        private static Theme windowTheme = Theme.Dark;

        /// <summary>
        /// Gets, sets and updates the window theme accordingly
        /// </summary>
        public static Theme WindowTheme
        {
            get
            {
                return windowTheme;
            }
            set
            {
                if (windowTheme == value) return;
                windowTheme = value;


                ResourceDictionary theme = GetTheme(value);

                foreach(Window w in activeWindows)
                {
                    BaseWindowStyle bws = null;
                    // get the corresponding resource dictionary
                    foreach(ResourceDictionary rd in w.Resources.MergedDictionaries)
                    {
                        if(rd is BaseWindowStyle)
                        {
                            bws = (BaseWindowStyle)rd;
                            break;
                        }
                    }

                    //if its not null, replace it
                    if(bws != null)
                    {
                        int index = w.Resources.MergedDictionaries.IndexOf(bws);
                        w.Resources.MergedDictionaries[index] = new BaseWindowStyle(theme);
                    }
                }
            }
        }

        /// <summary>
        /// The windows which use this resource dictionary
        /// </summary>
        private static List<Window> activeWindows;

        /// <summary>
        /// Used to access the maximize button and change the icon when the window changes state
        /// </summary>
        private Button maximizeButton;

        /// <summary>
        /// The window used to access the state
        /// </summary>
        private Window window;

        /// <summary>
        /// Default constructor
        /// </summary>
        public BaseWindowStyle()
        {
            MergedDictionaries.Add(GetTheme(WindowTheme));

            InitializeComponent();
        }

        private BaseWindowStyle(ResourceDictionary themeRD)
        {
            MergedDictionaries.Add(themeRD);
            InitializeComponent();
        }

        /// <summary>
        /// Returns according theme based on the parameter
        /// </summary>
        /// <param name="theme">The theme to get</param>
        /// <returns></returns>
        private static ResourceDictionary GetTheme(Theme theme)
        {
            var asm = System.Reflection.Assembly.Load("SCRCommon");
            string path;
            switch (WindowTheme)
            {
                case Theme.Dark:
                    path = "SCRCommon.WpfStyles.DarkTheme.xaml";
                    break;
                case Theme.Light:
                    path = "SCRCommon.WpfStyles.LightTheme.xaml";
                    break;
                default:
                    path = "";
                    break;
            }
            ResourceDictionary rd = (ResourceDictionary)XamlReader.Load(asm.GetManifestResourceStream(path));
            return rd;
        }

        /// <summary>
        /// When the minimize button is clicked: <br/>
        /// Minimizes the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Minimize(object sender, RoutedEventArgs e)
        {
            window.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// When the maximize button is clicked: <br/>
        /// Maximizes the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Maximize(object sender, RoutedEventArgs e)
        {
            window.WindowState = window.WindowState != WindowState.Maximized ? WindowState.Maximized : WindowState.Normal;
        }

        /// <summary>
        /// When the close button is clicked: <br/>
        /// Closes the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close(object sender, RoutedEventArgs e)
        {
            window.Close();
        }

        /// <summary>
        /// When the window changes states: <br/>
        /// Changes the icon on the maximize button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowStateChanged(object sender, EventArgs e)
        {
            maximizeButton.Content = window.WindowState == WindowState.Maximized ? "◱" : "☐";
        }

        /// <summary>
        /// Removes the window from the windows list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveWindow(object sender, EventArgs e)
        {
            activeWindows.Remove((Window)sender);
        }

        /// <summary>
        /// Gets called once the maximize button is initialized: <br/>
        /// Sets itself into the maximizeButton field and sets the window and "statechanged" event on the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MaximizeButton_Initialized(object sender, EventArgs e)
        {
            maximizeButton = (Button)sender;
            maximizeButton.Dispatcher.BeginInvoke(new ThreadStart(() =>
            {
                window = maximizeButton.Tag as Window;
                window.StateChanged += WindowStateChanged;
                window.Closed += RemoveWindow;

                if (activeWindows == null)
                {
                    activeWindows = new List<Window>();
                }
                activeWindows.Add(window);
            }));
        }
    }
}
