using SCRLanguageEditor.Data;
using SCRCommon.Viewmodels;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Xml;
using System.Collections.Generic;

namespace SCRLanguageEditor.Viewmodel
{
    public class VM_Main : BaseViewModel
    {
        private HeaderNode format;

        public ObservableCollection<VM_Node> Nodes { get; private set; }

        public RelayCommand LoadFile { get; private set; }
        public RelayCommand CreateNewFile { get; private set; }

        public VM_Main()
        {
            LoadFile = new RelayCommand(() => OpenFile());
            CreateNewFile = new RelayCommand(() => NewFile());
            LoadTemplate();
        }

        private void LoadTemplate()
        {
            format = FileLoader.LoadXMLFile("LanguageFiles/Format.xml");
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
            ofd.Filter = "XML Files (*.xml)|*.xml";
            if (ofd.ShowDialog() == true)
            {

            }
        }

        private void NewFile()
        {

        }
    }
}
