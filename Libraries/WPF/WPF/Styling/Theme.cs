using System;
using System.Windows;

namespace SCR.Tools.WPF.Styling
{
    /// <summary>
    /// Available Window Themes
    /// </summary>
    public enum Theme
    {
        Dark,
        Light
    }

    public static class ThemeExentions
    {
        /// <summary>
        /// Resource dictionary for the theme
        /// </summary>
        /// <param name="theme"></param>
        /// <returns></returns>
        public static ResourceDictionary GetResourceDictionary(this Theme theme)
            => new() { Source = theme.GetUri() };

        /// <summary>
        /// URI to the themes resource dictionary
        /// </summary>
        /// <param name="theme"></param>
        /// <returns></returns>
        public static Uri GetUri(this Theme theme)
            => new($"/SCR.Tools.WPF;component/WPF/Styling/Themes/{theme}.xaml", UriKind.RelativeOrAbsolute);
    }
}
