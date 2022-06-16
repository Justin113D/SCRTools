using SCR.Tools.Viewmodeling;
using SCR.Tools.Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

namespace SCR.Tools.DialogEditor.Viewmodeling
{
    public class VmNodeOptions : BaseViewModel
    {
        private readonly Dictionary<string, Color> _rawOptions;

        public ObservableCollection<VmNodeOption> Options { get; }

        public VmNodeOptions(Dictionary<string, Color> options)
        {
            _rawOptions = options;
            Options = new();
            foreach(string o in _rawOptions.Keys)
                Options.Add(new(this, o));
        }

        public Color this[string name]
        {
            get => _rawOptions[name];
            set => _rawOptions[name] = value;
        }

        public string RenameOption(string oldName, string newName)
        {
            string name = _rawOptions.FindNextFreeKey(newName, true);

            Color colorValue = _rawOptions[oldName];
            _rawOptions.Remove(oldName);
            _rawOptions.Add(name, colorValue);

            return name;
        }

        public void AddOption(string name)
        {
            if(string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            string freeName = _rawOptions.FindNextFreeKey(name, true);
            _rawOptions.Add(freeName, Color.Red);

            VmNodeOption vmOption = new(this, freeName);
            Options.Add(vmOption);
        }
    }

    public class VmNodeOption : BaseViewModel
    {
        private readonly VmNodeOptions _parent;

        private string _name;

        public string Name
        {
            get => _name;
            set
            {
                if(string.IsNullOrWhiteSpace(value) || _name == value)
                    return;

                _name = _parent.RenameOption(_name, value);
            }
        }

        public Color Color
        {
            get => _parent[Name];
            set => _parent[Name] = value;
        }

        public VmNodeOption(VmNodeOptions parent, string name)
        {
            _parent = parent;
            _name = name;
        }

        public override string ToString()
            => Name;
    }
}
