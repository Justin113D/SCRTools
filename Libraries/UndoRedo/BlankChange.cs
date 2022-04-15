using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCR.Tools.UndoRedo
{
    public struct BlankChange : ITrackable
    {
        public void Redo() { }

        public void Undo() { }
    }
}
