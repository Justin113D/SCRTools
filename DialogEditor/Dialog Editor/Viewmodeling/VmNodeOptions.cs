
using SCR.Tools.Viewmodeling;
using SCR.Tools.Common;
using SCR.Tools.UndoRedo.Collections;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;

namespace SCR.Tools.DialogEditor.Viewmodeling
{
    public class VmNodeOptions<T> : BaseViewModel
    {
        private readonly TrackDictionary<string, T> _rawOptions;

        private readonly TrackDictionary<string, VmNodeOption<T>> _lut;

        private readonly TrackList<VmNodeOption<T>> _options;

        public ReadOnlyObservableCollection<VmNodeOption<T>> Options { get; }

        private readonly T _defaultValue;

        public VmNodeOptions(TrackDictionary<string, T> options, T defaultValue)
        {
            _rawOptions = options;
            _defaultValue = defaultValue;

            Dictionary<string, VmNodeOption<T>> internalLut = new();
            ObservableCollection<VmNodeOption<T>> internalOptions = new();

            foreach(KeyValuePair<string, T> option in _rawOptions)
            {
                VmNodeOption<T> vmOption = new(this, option.Key);

                internalOptions.Add(vmOption);
                internalLut.Add(vmOption.Name, vmOption);
            }

            _lut = new(internalLut);
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
            VmNodeOption<T> vmValue = _lut[oldName];

            BeginChangeGroup();

            _rawOptions.Remove(oldName);
            _rawOptions.Add(name, TValue);

            _lut.Remove(oldName);
            _lut.Add(name, vmValue);

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
            _lut.Add(freeName, vmOption);
            _options.Add(vmOption);

            EndChangeGroup();
        }
        
        public VmNodeOption<T>? GetOption(string name)
        {
            if(!_lut.TryGetValue(name, out VmNodeOption<T>? result))
            {
                return null;
            }

            return result;
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
