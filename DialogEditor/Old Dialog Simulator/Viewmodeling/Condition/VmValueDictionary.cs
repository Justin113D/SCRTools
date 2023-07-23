using SCR.Tools.UndoRedo.Collections;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SCR.Tools.Common;
using System.Linq;
using SCR.Tools.WPF.Viewmodeling;

namespace SCR.Tools.Dialog.Simulator.Viewmodeling.Condition
{
    internal class VmValueDictionary<T> : BaseViewModel
    {
        private readonly TrackDictionary<int, T> _source;

        private readonly TrackList<VmValueDictionaryItem<T>> _values;

        private readonly T _defaultValue;

        public bool Expanded { get; set; }

        public ReadOnlyObservableCollection<VmValueDictionaryItem<T>> Values { get; }

        public T this[int index]
        {
            get => _source[index];
            set => _source[index] = value;
        }

        public VmValueDictionary(TrackDictionary<int, T> source, T defaultValue)
        {
            _source = source;
            _defaultValue = defaultValue;

            ObservableCollection<VmValueDictionaryItem<T>> internalValues = new();

            foreach(int id in source.Keys.OrderBy(x => x))
            {
                internalValues.Add(new(this, id));
            }

            _values = new(internalValues);
            Values = new(internalValues);
        }

        public bool AddValue(int id)
        {
            if(_source.ContainsKey(id))
            {
                return false;
            }

            BeginChangeGroup();

            _source.Add(id, _defaultValue);

            VmValueDictionaryItem<T> vmOption = new(this, id);

            int i = 0;
            for(; i < _values.Count; i++)
            {
                if(id < _values[i].ID)
                {
                    break;
                }
            }

            _values.Insert(i, vmOption);

            EndChangeGroup();

            return true;
        }

        public void RemoveValue(VmValueDictionaryItem<T> value)
        {
            BeginChangeGroup();

            _source.Remove(value.ID);
            _values.Remove(value);

            EndChangeGroup();
        }
    }

    internal class VmValueDictionaryItem<T> : BaseViewModel
    {
        private readonly VmValueDictionary<T> _parent;

        public int ID { get; }

        public T Value
        {
            get => _parent[ID];
            set
            {
                BeginChangeGroup();

                _parent[ID] = value;
                TrackNotifyProperty(nameof(Value));

                EndChangeGroup();
            }
        }

        public RelayCommand CmdRemove
            => new(Remove);

        public VmValueDictionaryItem(VmValueDictionary<T> parent, int index)
        {
            _parent = parent;
            ID = index;
        }

        public void Remove()
            => _parent.RemoveValue(this);

        public override string ToString()
            => $"{ID}";
    }
}
