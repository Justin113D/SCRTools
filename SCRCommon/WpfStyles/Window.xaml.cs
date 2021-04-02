﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Shell;

namespace SCRCommon.WpfStyles
{
    /// <summary>
    /// Interaction logic for Window.xaml
    /// </summary>
    public class Window : System.Windows.Window
    {
        public static readonly DependencyProperty MinimizeButtonProperty
            = DependencyProperty.Register(
                nameof(MinimizeButton),
                typeof(Visibility),
                typeof(Window));

        public static readonly DependencyProperty MaximizeButtonProperty
            = DependencyProperty.Register(
                nameof(MaximizeButton),
                typeof(Visibility),
                typeof(Window));

        public static readonly DependencyProperty CloseButtonProperty
            = DependencyProperty.Register(
                nameof(CloseButton),
                typeof(Visibility),
                typeof(Window));

        public static readonly DependencyProperty ShadowPaddingProperty
            = DependencyProperty.Register(
                nameof(ShadowPadding),
                typeof(Thickness),
                typeof(Window),
                new PropertyMetadata(
                    new Thickness(7)
                ));

        public Visibility MinimizeButton
        {
            get => (Visibility)GetValue(MinimizeButtonProperty);
            set => SetValue(MinimizeButtonProperty, value);
        }

        public Visibility MaximizeButton
        {
            get => (Visibility)GetValue(MaximizeButtonProperty);
            set => SetValue(MaximizeButtonProperty, value);
        }

        public Visibility CloseButton
        {
            get => (Visibility)GetValue(CloseButtonProperty);
            set => SetValue(CloseButtonProperty, value);
        }

        public Thickness ShadowPadding
        {
            get => (Thickness)GetValue(ShadowPaddingProperty);
            set => SetValue(ShadowPaddingProperty, value);
        }

        /// <summary>
        /// The windows which use this resource dictionary
        /// </summary>
        private static readonly List<Window> _activeWindows;

        private static readonly ControlTemplate _template;

        private Button _maximizeButton;

        static Window()
        {
            _activeWindows = new List<Window>();

            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SCRCommon.WpfStyles.Window.xaml");
            _template = (ControlTemplate)XamlReader.Load(stream);
        }

        public Window()
        {
            BaseStyle.Init();

            StateChanged += OnStateChanged;
            Loaded += OnLoaded;
            Closed += OnClosed;

            Padding = new(4);

            WindowChrome chrome = new()
            {
                CaptionHeight = 40,
                GlassFrameThickness = new(0),
                CornerRadius = new(0),
                ResizeBorderThickness = Padding
            };

            BindingOperations.SetBinding(
                chrome,
                WindowChrome.ResizeBorderThicknessProperty,
                new Binding("ShadowPadding") { Source = this }
            );

            WindowChrome.SetWindowChrome(this, chrome);

            AllowsTransparency = true;
            WindowStyle = WindowStyle.None;

            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;

            Template = _template;
        }

        /// <summary>
        /// When the minimize button is clicked: <br/>
        /// Minimizes the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Minimize(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// When the maximize button is clicked: <br/>
        /// Maximizes the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Maximize(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState != WindowState.Maximized ? WindowState.Maximized : WindowState.Normal;
        }

        /// <summary>
        /// When the close button is clicked: <br/>
        /// Closes the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnStateChanged(object sender, EventArgs e)
        {
            _maximizeButton.Content = WindowState == WindowState.Maximized ? "◱" : "☐";
        }


        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _maximizeButton = (Button)Template.FindName("MaximizeButton", this);

            ((Button)Template.FindName("MinimizeButton", this)).Click += Minimize;
            _maximizeButton.Click += Maximize;
            ((Button)Template.FindName("CloseButton", this)).Click += Close;

            _activeWindows.Add(this);
        }

        private void OnClosed(object sender, EventArgs e)
        {
            _activeWindows.Remove(this);
        }
    }

}