using System;
using System.Collections.Generic;
using System.Windows;

namespace SCR.Tools.WPF.Theme
{
    /// <summary>
    /// Available Window Skins
    /// </summary>
    public enum Skin
    {
        Dark,
        Light
    }

    internal static class SkinExtensions
    {
        private static readonly Dictionary<Skin, ResourceDictionary> _resources;

        static SkinExtensions()
        {
            _resources = new();

            foreach (Skin skin in Enum.GetValues<Skin>())
            {
                Uri uri = new($"/SCR.Tools.WPF;component/Theme/Skins/{skin}.xaml", UriKind.RelativeOrAbsolute);
                ResourceDictionary resources = new() { Source = uri };
                _resources.Add(skin, resources);
            }
        }

        public static ResourceDictionary GetResources(this Skin skin)
            => _resources[skin];
    }
}
