using DryIoc;
using SCRLanguageEditor.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCRLanguageEditor.Viewmodel
{
    public class VM_Main
    {
        private IContainer container;

        public ObservableCollection<Node> Nodes { get; private set; }

        public VM_Main(IContainer container)
        {
            this.container = container;
            Nodes = new ObservableCollection<Node>();
        }
    }
}
