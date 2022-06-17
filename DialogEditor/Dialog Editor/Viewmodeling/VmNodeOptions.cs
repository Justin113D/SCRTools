
using SCR.Tools.Viewmodeling;
using SCR.Tools.Common;
using SCR.Tools.UndoRedo;
using SCR.Tools.UndoRedo.Collections;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;
using System.Collections.ObjectModel;

namespace SCR.Tools.DialogEditor.Viewmodeling
{
    public class VmNodeOptions<T> : BaseViewModel
    {
        private readonly TrackDictionary<string, T> _rawOptions;

        private readonly TrackList<VmNodeOption<T>> _options;

        public ReadOnlyObservableCollection<VmNodeOption<T>> Options { get; }

        private readonly T _defaultValue;

        public VmNodeOptions(TrackDictionary<string, T> options, T defaultValue)
        {
            _rawOptions = options;
            _defaultValue = defaultValue;

            ObservableCollection<VmNodeOption<T>> internalOptions = new();
            foreach(string o in _rawOptions.Keys)
                internalOptions.Add(new(this, o));

            _options = new(internalOptions);
            Options = new(internalOptions);
        }

        public T this[string name]
        {
            get => _rawOptions[name];
            set => _rawOptions[name] = value;
        }

        public string RenameOption(string oldName, string newName)
        {
            string name = _rawOptions.FindNextFreeKey(newName, true);

            T TValue = _rawOptions[oldName];

            BeginChangeGroup();

            _rawOptions.Remove(oldName);
            _rawOptions.Add(name, TValue);

            EndChangeGroup();

            return name;
        }

        public void AddOption(string name)
        {
            if(string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            BeginChangeGroup();

            string freeName = _rawOptions.FindNextFreeKey(name, true);
            _rawOptions.Add(freeName, _defaultValue);

            VmNodeOption<T> vmOption = new(this, freeName);
            _options.Add(vmOption);

            EndChangeGroup();
        }
    }

    public class VmNodeOption<T> : BaseViewModel
    {
        private readonly VmNodeOptions<T> _parent;

        private string _name;

        public string Name
        {
            get => _name;
            set
            {
                if(string.IsNullOrWhiteSpace(value) || _name == value)
                    return;

                BeginChangeGroup();

                string newName = _parent.RenameOption(_name, value);

                TrackValueChange(
                    (v) => _name = v, _name, newName);

                TrackNotifyProperty(nameof(Name));

                EndChangeGroup();
            }
        }

        public T Value
        {
            get => _parent[Name];
            set
            {
                BeginChangeGroup();
                
                _parent[Name] = value;
                TrackNotifyProperty(nameof(Value));

                EndChangeGroup();
            }
        }

        public VmNodeOption(VmNodeOptions<T> parent, string name)
        {
            _parent = parent;
            _name = name;
        }

        public override string ToString()
            => Name;
    }
}
