using SCR.Tools.UndoRedo.Collections;
using SCR.Tools.Viewmodeling;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;

namespace SCR.Tools.Dialog.Simulator.Viewmodeling.Condition
{
    internal class VmIntSet : BaseViewModel
    {
        private readonly ISet<int> _source;

        private readonly TrackList<int> _values;

        public ReadOnlyObservableCollection<int> Values { get; }

        public bool Expanded { get; set; }

        public RelayCommand<int> CmdRemove
            => new(RemoveValue);

        public VmIntSet(ISet<int> source)
        {
            _source = source;

            ObservableCollection<int> internalValues = new(source.OrderBy(x => x));
            
            _values = new(internalValues);
            Values = new(internalValues);
        }

        public void AddValue(int value)
        {
            BeginChangeGroup();

            _source.Add(value);

            for(int i = 0; i < _values.Count; i++)
            {
                if (_values[i] > value)
                {
                    _values.Insert(i, value);
                    EndChangeGroup();
                    return;
                }
            }

            _values.Add(value);

            EndChangeGroup();
        }

        public void RemoveValue(int value)
        {
            BeginChangeGroup();

            _source.Remove(value);
            _values.Remove(value);

            EndChangeGroup();
        }
    }
}
