using ColorPickerWPF;
using SCRCommon.Viewmodels;
using SCRDialogEditor.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

namespace SCRDialogEditor.Viewmodel
{
    public class VmNodeOptions : BaseViewModel
    {
        private readonly List<NodeOption> _rawOptions;

        public ObservableCollection<VmNodeOption> Options { get; }

        public ObservableCollection<VmNodeOption> OptionsCombobox { get; }

        public VmNodeOptions(List<NodeOption> options)
        {
            _rawOptions = options;
            Options = new();
            foreach(var o in _rawOptions)
                Options.Add(new(this, o));
            OptionsCombobox = new(Options.ToArray());
            Options.Add(new(this, null));
        }

        public void QualifyNode(NodeOption option)
        {
            OptionsCombobox.Add(Options[^1]);
            _rawOptions.Add(option);
            Options.Add(new(this, null));
            OnPropertyChanged(nameof(_rawOptions));
        }

        public VmNodeOption GetOption(string name)
            => Options.First(x => x.Name == name);
    }

    public class VmNodeOption : BaseViewModel
    {
        public RelayCommand Cmd_ChangeColor
            => new(ChangeColor);

        private readonly VmNodeOptions _parent;

        private NodeOption _data;

        public string Name
        {
            get => _data?.Name ?? "";
            set
            {
                if(string.IsNullOrWhiteSpace(value))
                    return;
                if(_data == null)
                    Qualify();
                _data.Name = value;
            }
        }


        public SolidColorBrush Color => new(_data?.Color ?? Colors.Transparent);

        public VmNodeOption(VmNodeOptions options, NodeOption nodeOption)
        {
            _parent = options;
            _data = nodeOption;
        }

        /// <summary>
        /// When the null node gets edited, it will be added to the list of nodes
        /// </summary>
        private void Qualify()
        {
            _data = new();
            _parent.QualifyNode(_data);
            OnPropertyChanged(nameof(Color));
        }

        private void ChangeColor()
        {
            if(_data == null)
                return;

            if(ColorPickerWindow.ShowDialog(out Color col, ColorPickerWPF.Code.ColorPickerDialogOptions.SimpleView))
            {
                _data.Color = col;
                OnPropertyChanged(nameof(Color));
            }
        }

        public override string ToString()
            => Name;
    }


}
