using SCRLanguageEditor.Data;
using SCRCommon.Viewmodels;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Markup;
using System;

namespace SCRLanguageEditor.Viewmodel
{
    public class VM_Main : BaseViewModel
    {
        private ResourceDictionary windowTheme;
        public HeaderNode HeaderNode { get; private set; }

        public RelayCommand LoadFile { get; private set; }

        public VM_Main()
        {
            LoadFile = new RelayCommand(() => OpenFile());
        }

        public void SetDictionaries(MainWindow window)
        {
            var asm = System.Reflection.Assembly.Load("SCRCommon");
            var stream = asm.GetManifestResourceStream("SCRCommon.WpfStyles.WindowStyle.xaml");
            windowTheme = (ResourceDictionary)XamlReader.Load(stream);

            window.Resources.MergedDictionaries.Add(windowTheme);
        }

        private void OpenFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "XML Files (*.xml)|*.xml";
            if (ofd.ShowDialog() == true)
            {
                HeaderNode = FileLoader.LoadXMLFile(ofd.FileName);
            }

        }
    }
}
