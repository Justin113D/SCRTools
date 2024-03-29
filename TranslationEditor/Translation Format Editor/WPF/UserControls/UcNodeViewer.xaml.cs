﻿using SCR.Tools.TranslationEditor.FormatEditor.Viewmodeling;
using System.Windows;
using System.Windows.Controls;

namespace SCR.Tools.TranslationEditor.FormatEditor.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for UcNodeViewer.xaml
    /// </summary>
    public partial class UcNodeViewer : UserControl
    {
        private VmFormat ViewModel => (VmFormat)DataContext;

        public static readonly DependencyProperty DraggingProperty =
            DependencyProperty.Register(
                nameof(Dragging),
                typeof(bool),
                typeof(UcNodeViewer));

        public bool Dragging
        {
            get => (bool)GetValue(DraggingProperty);
            set => SetValue(DraggingProperty, value);
        }


        public UcNodeViewer()
        {
            InitializeComponent();
        }

        private void TreeView_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ViewModel.DeselectAll();
        }

        private void Event_DeselectAll(object sender, System.Windows.RoutedEventArgs e)
        {
            ViewModel.DeselectAll();
        }
    }
}
