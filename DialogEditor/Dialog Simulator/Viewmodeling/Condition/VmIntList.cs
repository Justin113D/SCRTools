using SCR.Tools.UndoRedo.Collections;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SCR.Tools.WPF.Viewmodeling;

namespace SCR.Tools.Dialog.Simulator.Viewmodeling.Condition
{
    internal class VmIntList : BaseViewModel
    {
        private readonly IList<int> _source;

        private readonly TrackList<VmIntListItem> _items;

        public ReadOnlyObservableCollection<VmIntListItem> Items { get; }

        public bool Expanded { get; set; }

        public VmIntList(IList<int> source)
        {
            _source = source;

            ObservableCollection<VmIntListItem> internalValues = new();

            for (int i = 0; i < source.Count; i++)
            {
                internalValues.Add(new(this));
            }

            _items = new(internalValues);
            Items = new(internalValues);
        }

        private int GetIndex(VmIntListItem item)
        {
            int i = 0;
            foreach (VmIntListItem listItem in _items)
            {
                if (listItem == item)
                {
                    return i;
                }
                i++;
            }

            throw new InvalidOperationException("Item not in list");
        }

        public int GetValue(VmIntListItem item)
        {
            return _source[GetIndex(item)];
        }

        public void ChangeValue(VmIntListItem item, int value)
        {
            _source[GetIndex(item)] = value;
        }

        public void AddValue(int value)
        {
            BeginChangeGroup();

            _source.Add(value);
            _items.Add(new(this));

            EndChangeGroup();
        }

        public void Remove(VmIntListItem item)
        {
            BeginChangeGroup();

            int index = GetIndex(item);
            _source.RemoveAt(index);
            _items.RemoveAt(index);

            EndChangeGroup();
        }
    }

    internal class VmIntListItem : BaseViewModel
    {
        private readonly VmIntList _parent;

        public int Value
        {
            get => _parent.GetValue(this);
            set
            {
                if (Value == value)
                    return;

                BeginChangeGroup();

                _parent.ChangeValue(this, value);
                TrackNotifyProperty(nameof(Value));

                EndChangeGroup();
            }
        }

        public RelayCommand CmdRemove
            => new(Remove);

        public VmIntListItem(VmIntList parent)
        {
            _parent = parent;
        }
    
        private void Remove()
        {
            _parent.Remove(this);
        }
    }
}
