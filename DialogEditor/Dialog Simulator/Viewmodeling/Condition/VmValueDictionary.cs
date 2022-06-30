using SCR.Tools.UndoRedo.Collections;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SCR.Tools.Common;
using SCR.Tools.Viewmodeling;

namespace SCR.Tools.Dialog.Simulator.Viewmodeling.Condition
{
    internal class VmValueDictionary<T> : BaseViewModel
    {
        private readonly TrackDictionary<int, T> _source;

        private readonly TrackList<VmValueDictionaryItem<T>> _values;

        private readonly T _defaultValue;

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

            foreach(KeyValuePair<int, T> pair in source)
            {
                VmValueDictionaryItem<T> viewmodel = new(this, pair.Key);
                internalValues.Add(viewmodel);
            }

            _values = new(internalValues);
            Values = new(internalValues);
        }

        public int ChangeKey(int oldID, int newID)
        {
            int key = _source.FindNextFreeKey(newID);

            T TValue = _source[oldID];

            BeginChangeGroup();

            _source.Remove(oldID);
            _source.Add(key, TValue);

            EndChangeGroup();

            return key;
        }

        public void AddValue(int id)
        {
            BeginChangeGroup();
            int freeKey = _source.FindNextFreeKey(id);

            _source.Add(freeKey, _defaultValue);

            VmValueDictionaryItem<T> vmOption = new(this, freeKey);
            _values.Add(vmOption);

            EndChangeGroup();
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

        private int _id;

        public int ID
        {
            get => _id;
            set
            {
                if (_id == value)
                    return;

                BeginChangeGroup();

                int newKey = _parent.ChangeKey(_id, value);

                TrackValueChange(
                    (v) => _id = v, _id, newKey);

                TrackNotifyProperty(nameof(ID));

                EndChangeGroup();
            }
        }

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
            _id = index;
        }

        public void Remove()
            => _parent.RemoveValue(this);

        public override string ToString()
            => $"{ID}";
    }
}
