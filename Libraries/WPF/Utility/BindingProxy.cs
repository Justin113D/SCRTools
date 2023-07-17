﻿using System.Windows;

namespace SCR.Tools.WPF.Utility
{
    /// <summary>
    /// Used for "reusing" a Binding
    /// </summary>
    public class BindingProxy : Freezable
    {
        protected override Freezable CreateInstanceCore()
            => new BindingProxy();

        public object Data
        {
            get => GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty
            = DependencyProperty.Register(
                "Data",
                typeof(object),
                typeof(BindingProxy),
                new UIPropertyMetadata(null));
    }
}
