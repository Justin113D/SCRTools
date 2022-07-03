using SCR.Tools.UndoRedo.Collections;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SCR.Tools.Common;
using SCR.Tools.Viewmodeling;
using System;

namespace SCR.Tools.Dialog.Simulator.Viewmodeling.Condition
{
    internal class VmSlotDictionary<T, vmT> 
        where vmT : VmlotDictionaryItem<T, vmT> 
        where T : ICloneable
    {
        private readonly TrackDictionary<int, T> _source;

        private readonly TrackList<vmT> _slots;

        private readonly T _defaultSlot;

        private readonly Func<VmSlotDictionary<T, vmT>, int, vmT> _createSlot;

        public bool Expanded { get; set; }

        public ReadOnlyObservableCollection<vmT> Slots { get; }

        public T this[int index]
        {
            get => _source[index];
            set => _source[index] = value;
        }

        public VmSlotDictionary(TrackDictionary<int, T> source, T defaultSlot, Func<VmSlotDictionary<T, vmT>, int, vmT> createSlot)
        {
            _source = source;
            _defaultSlot = defaultSlot;
            _createSlot = createSlot;

            ObservableCollection<vmT> internalSlots = new();

            foreach (KeyValuePair<int, T> pair in source)
            {
                vmT viewmodel = _createSlot(this, pair.Key);
                internalSlots.Add(viewmodel);
            }

            _slots = new(internalSlots);
            Slots = new(internalSlots);
        }

        public bool ChangeID(int oldID, int newID)
        {
            if(_source.ContainsKey(newID))
            {
                return false;
            }

            BeginChangeGroup();

            T item = _source[oldID];
            _source.Remove(oldID);
            _source.Add(newID, item);

            EndChangeGroup();

            return true;
        }

        public bool AddSlot(int id)
        {
            if (_source.ContainsKey(id))
            {
                return false;
            }

            BeginChangeGroup();

            _source.Add(id, (T)_defaultSlot.Clone());

            vmT vmOption = _createSlot(this, id);
            _slots.Add(vmOption);

            EndChangeGroup();

            return true;
        }

        public void RemoveSlot(vmT slot)
        {
            BeginChangeGroup();

            _source.Remove(slot.ID);
            _slots.Remove(slot);

            EndChangeGroup();
        }
    }

    internal abstract class VmlotDictionaryItem<T, vmT> : BaseViewModel 
        where vmT : VmlotDictionaryItem<T, vmT> 
        where T : ICloneable
    {
        private readonly VmSlotDictionary<T, vmT> _parent;

        private int _id;

        public int ID
        {
            get => _id;
            set
            {
                if (_id == value)
                    return;

                BeginChangeGroup();

                if(_parent.ChangeID(_id, value))
                {
                    TrackValueChange(
                        (v) => _id = v, _id, value);

                    TrackNotifyProperty(nameof(ID));
                }

                EndChangeGroup();
            }
        }

        protected T Slot
            => _parent[ID];

        public RelayCommand CmdRemove
            => new(Remove);

        public VmlotDictionaryItem(VmSlotDictionary<T, vmT> parent, int id)
        {
            _parent = parent;
            _id = id;
        }

        public void Remove()
            => _parent.RemoveSlot((vmT)this);

        public override string ToString()
            => $"{ID}";
    }
}
