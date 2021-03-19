using SCRCommon.Viewmodels;
using SCRDialogEditor.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SCRDialogEditor.Viewmodel
{
    public class VmNodeOptions : BaseViewModel
    {
        public List<NodeOption> RawOptions { get; }

        public ObservableCollection<VmNodeOption> Options { get; }

        public VmNodeOptions(List<NodeOption> options)
        {
            RawOptions = options;
            Options = new();
            foreach(var o in RawOptions)
                Options.Add(new(this, o));
            Options.Add(new(this, null));
        }

        public void QualifyNode(NodeOption option)
        {
            RawOptions.Add(option);
            Options.Add(new(this, null));
            OnPropertyChanged(nameof(RawOptions));
        }

        public NodeOption GetOption(string name)
            => RawOptions.Find(x => x.Name == name);
    }
}
