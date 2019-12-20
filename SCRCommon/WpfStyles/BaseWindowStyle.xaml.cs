using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace SCRCommon.WpfStyles
{
    /// <summary>
    /// Base class for a darkmode window frame that sets all necessary events for the window frame to function
    /// </summary>
    public partial class BaseWindowStyle : ResourceDictionary
    {
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
            InitializeComponent();
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
            }));
        }
    }
}
