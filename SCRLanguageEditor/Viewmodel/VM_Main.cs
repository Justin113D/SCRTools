using SCRLanguageEditor.Data;
using SCRCommon.Viewmodels;
using Microsoft.Win32;

namespace SCRLanguageEditor.Viewmodel
{
    public class VM_Main : BaseViewModel
    {
        public HeaderNode HeaderNode { get; private set; }

        public RelayCommand LoadFile { get; private set; }
        public RelayCommand CreateNewFile { get; private set; }

        public VM_Main()
        {
            LoadFile = new RelayCommand(() => OpenFile());
            CreateNewFile = new RelayCommand(() => NewFile());
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

        private void NewFile()
        {
            HeaderNode = new HeaderNode("English", "0");
        }
    }
}
