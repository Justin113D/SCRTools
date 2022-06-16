using SCR.Tools.Viewmodeling;
using SCR.Tools.Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SCR.Tools.DialogEditor.Viewmodeling
{
    public class VmNodeIcons : BaseViewModel
    {
        private readonly Dictionary<string, string> _rawIcons;

        public ObservableCollection<VmNodeIcon> Icons { get; }

        public VmNodeIcons(Dictionary<string, string> icons)
        {
            _rawIcons = icons;
            Icons = new();
            foreach (string o in _rawIcons.Keys)
                Icons.Add(new(this, o));
        }

        public string this[string name]
        {
            get => _rawIcons[name];
            set => _rawIcons[name] = value;
        }

        public string RenameOption(string oldName, string newName)
        {
            string name = _rawIcons.FindNextFreeKey(newName, true);

            string iconPath = _rawIcons[oldName];
            _rawIcons.Remove(oldName);
            _rawIcons.Add(name, iconPath);

            return name;
        }

        public void AddOption(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            string freeName = _rawIcons.FindNextFreeKey(name, true);
            _rawIcons.Add(freeName, "");

            VmNodeIcon vmIcon = new(this, freeName);
            Icons.Add(vmIcon);
        }
    }

    public class VmNodeIcon : BaseViewModel
    {
        private readonly VmNodeIcons _parent;

        private string _name;

        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value) || _name == value)
                    return;

                _name = _parent.RenameOption(_name, value);
            }
        }

        public string IconPath
        {
            get => _parent[Name];
            set => _parent[Name] = value;
        }

        public VmNodeIcon(VmNodeIcons parent, string name)
        {
            _parent = parent;
            _name = name;
        }

        public override string ToString()
            => Name;
    }
}
