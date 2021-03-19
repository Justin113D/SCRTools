using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
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
    public partial class BaseWindowStyle : ResourceDictionary
    {
        /// <summary>
        /// The current active window theme. Default is dark
        /// </summary>
        private static Theme windowTheme = Theme.Dark;

        public static ResourceDictionary currentTheme { get; private set; }

        /// <summary>
        /// Gets, sets and updates the window theme accordingly
        /// </summary>
        public static Theme WindowTheme
        {
            get => windowTheme;
            set
            {
                if(windowTheme == value)
                    return;
                windowTheme = value;

                currentTheme = GetTheme(value);

                if(activeWindows == null)
                {
                    return;
                }

                foreach(Window w in activeWindows)
                {
                    // get the corresponding resource dictionary
                    foreach(ResourceDictionary rd in w.Resources.MergedDictionaries)
                    {
                        if(rd is BaseWindowStyle style)
                        {
                            style.MergedDictionaries[0] = currentTheme;
                            break;
                        }
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

        static BaseWindowStyle()
        {
            currentTheme = GetTheme(WindowTheme);
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public BaseWindowStyle()
        {
            MergedDictionaries.Add(currentTheme);
            InitializeComponent();
        }

        /// <summary>
        /// Returns according theme based on the parameter
        /// </summary>
        /// <param name="theme">The theme to get</param>
        /// <returns></returns>
        private static ResourceDictionary GetTheme(Theme theme)
        {
            //var asm = Assembly.Load("SCRCommon")
            ResourceDictionary rd = new ResourceDictionary();
            string path = theme switch
            {
                Theme.Light => "LightTheme.xaml",
                Theme.Dark => "DarkTheme.xaml",
                _ => "DarkTheme.xaml",
            };
            rd.Source = new Uri("/SCRCommon;component/WpfStyles/" + path, UriKind.RelativeOrAbsolute);
            return rd;

            //System.IO.Stream v = asm.GetManifestResourceStream(path);
            //ResourceDictionary rd = (ResourceDictionary)XamlReader.Load(v);
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
            if(maximizeButton == null)
                return;
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
        /// Sets itself into the maximizeButton field
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MaximizeButton_Initialized(object sender, EventArgs e)
        {
            maximizeButton = (Button)sender;
        }

        /// <summary>
        /// Initializes the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Initialized(object sender, EventArgs e)
        {
            ((Border)sender).Dispatcher.BeginInvoke(new ThreadStart(() =>
            {
                window = ((Border)sender).Tag as Window;
                window.StateChanged += WindowStateChanged;
                window.Closed += RemoveWindow;

                Style windowStyle = (bool)typeof(Window).GetField("_showingAsDialog", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(window) ? (Style)this["DialogWindowStyle"] : (Style)this["DefaultWindowStyle"];
                window.Style = windowStyle;

                if(activeWindows == null)
                {
                    activeWindows = new List<Window>();
                }
                activeWindows.Add(window);
            }));

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
