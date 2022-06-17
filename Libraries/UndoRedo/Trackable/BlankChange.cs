using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCR.Tools.UndoRedo.Trackable
{
    internal struct BlankChange : ITrackable
    {
        public void Redo() { }

        public void Undo() { }
    }
}
