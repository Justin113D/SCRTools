using SCRLanguageEditor.Data;
using SCRCommon.Viewmodels;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Xml;
using System.Collections.Generic;
using SCRCommon.WpfStyles;

namespace SCRLanguageEditor.Viewmodel
{
    public class VM_Main : BaseViewModel
    {
        private HeaderNode format;

        public ObservableCollection<VM_Node> Nodes { get; private set; }

        public RelayCommand Cmd_LoadFormat { get; private set; }
        public RelayCommand Cmd_NewFile { get; private set; }
        public RelayCommand Cmd_Open { get; private set; }
        public RelayCommand Cmd_Save { get; private set; }
        public RelayCommand Cmd_SaveAs { get; private set; }
        public RelayCommand Cmd_Settings { get; private set; }

        private readonly VM_Settings settings;

        public VM_Main(VM_Settings settings)
        {
            this.settings = settings;
            Cmd_Open = new RelayCommand(() => OpenFile());
            Cmd_NewFile = new RelayCommand(() => NewFile());
            Cmd_Settings = new RelayCommand(() => OpenSettings());
            LoadTemplate(settings.DefaultFormatPath);
        }

        private void LoadTemplate(string path)
        {
            format = FileLoader.LoadXMLFile(path);
            Nodes = new ObservableCollection<VM_Node>();
            foreach (Node n in format?.ChildNodes)
            {
                switch (n.Type)
                {
                    case Node.NodeType.ParentNode:
                        Nodes.Add(new VM_ParentNode((ParentNode)n));
                        break;
                    case Node.NodeType.StringNode:
                        Nodes.Add(new VM_StringNode((StringNode)n));
                        break;
                    default:
                        Nodes.Add(null);
                        break;
                }
            }
        }

        private void OpenFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Lang Files (*.lang)|*.lang";
            if (ofd.ShowDialog() == true)
            {

            }
        }

        private void NewFile()
        {
            BaseWindowStyle.WindowTheme = BaseWindowStyle.WindowTheme == Theme.Dark ? Theme.Light : Theme.Dark;
        }

        private void OpenSettings()
        {
            new SettingsWindow(settings).ShowDialog();
        }
    }
}
