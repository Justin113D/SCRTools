using SCRLanguageEditor.Data;
using SCRCommon.Viewmodels;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace SCRLanguageEditor.Viewmodel
{
    public class VM_Main : BaseViewModel
    {
        private HeaderNode HeaderNode;

        public ObservableCollection<VM_Node> Nodes { get; private set; }

        public RelayCommand LoadFile { get; private set; }
        public RelayCommand CreateNewFile { get; private set; }

        public VM_Main()
        {
            LoadFile = new RelayCommand(() => OpenFile());
            CreateNewFile = new RelayCommand(() => NewFile());

            Nodes = new ObservableCollection<VM_Node>()
            {
                null
            };
        }
        private void OpenFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "XML Files (*.xml)|*.xml";
            if (ofd.ShowDialog() == true)
            {
                HeaderNode = FileLoader.LoadXMLFile(ofd.FileName);
            }
            List<VM_Node> nodes = new List<VM_Node>();
            foreach(Node n in HeaderNode?.ChildNodes)
            {
                switch (n.Type)
                {
                    case Node.NodeType.ParentNode:
                        nodes.Add(new VM_ParentNode((ParentNode)n));
                        break;
                    case Node.NodeType.StringNode:
                        nodes.Add(new VM_StringNode((StringNode)n));
                        break;
                    default:
                        nodes.Add(null);
                        break;
                }
            }
            if(nodes.Count == 0)
            {
                nodes.Add(null);
            }

            Nodes = new ObservableCollection<VM_Node>(nodes);
        }

        private void NewFile()
        {
            HeaderNode = new HeaderNode("English", "0");
        }
    }
}
