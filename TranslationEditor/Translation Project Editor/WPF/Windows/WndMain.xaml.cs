﻿using SCR.Tools.TranslationEditor.ProjectEditor.Viewmodeling;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using Window = SCR.Tools.WPF.Styling.Window;

namespace SCR.Tools.TranslationEditor.ProjectEditor.WPF.Windows
{
    /// <summary>
    /// Interaction logic for WndMain.xaml
    /// </summary>
    public partial class WndMain : Window
    {
        public WndMain()
        {
            InitDataContext();
            InitializeComponent();
        }

        protected override void Close(object sender, RoutedEventArgs e)
        {
            if(MenuBar.CloseConfirmation())
            {
                base.Close(sender, e);
            }
        }

        private void InitDataContext()
        {
            VmMain vm = new();
            DataContext = vm;

            string formatPath = Properties.Settings.Default.DefaultFormatPath;
            if (string.IsNullOrWhiteSpace(formatPath))
                return;

            if (!File.Exists(formatPath))
            {
                MessageBox.Show("Default format path does not resolve to a file", "Default format not found", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                string data = File.ReadAllText(formatPath);
                vm.LoadFormat(data);
            }
            catch (Exception e)
            {
                MessageBox.Show("Default format is not valid!\n " + e.Message, "Error loading default format", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void IB_New(object sender, object e)
        {
            MenuBar.NewProject(sender, new());
        }

        private void IB_Open(object sender, object e)
        {
            MenuBar.OpenProject(sender, new());
        }

        private void IB_OpenFormat(object sender, object e)
        {
            MenuBar.LoadFormat(sender, new());
        }

        private void IB_Save(object sender, object e)
        {
            MenuBar.SaveProject(sender, new());
        }

        private void IB_SaveAs(object sender, object e)
        {
            MenuBar.SaveProjectAs(sender, new());
        }

        private void IB_Export(object sender, object e)
        {
            MenuBar.ExportLanguageFiles(sender, new());
        }

        private void IB_Import(object sender, object e)
        {
            MenuBar.ImportLanguageFiles(sender, new());
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            MenuBar.Close();
            base.OnClosing(e);
        }
    }
}
