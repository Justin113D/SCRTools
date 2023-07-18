using SCR.Tools.WPF.Viewmodeling;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;

namespace SCR.Tools.Dialog.Editor.Viewmodeling
{
    public class VmInstruction : BaseViewModel
    {
        private readonly VmNodeOutput _parent;

        private int Index => _parent.Instructions.IndexOf(this);

        public string Instruction
        {
            get => _parent.Data.Instructions[Index];
            set 
            {
                BeginChangeGroup();
                _parent.Data.Instructions[Index] = value;
                TrackNotifyProperty(nameof(Instruction));
                EndChangeGroup();
            }
        }

        public RelayCommand CmdRemove
            => new(Remove);

        public VmInstruction(VmNodeOutput parent)
        {
            _parent = parent;
        }

        private void Remove()
        {
            _parent.RemoveInstruction(Index);
        }
    }
}
