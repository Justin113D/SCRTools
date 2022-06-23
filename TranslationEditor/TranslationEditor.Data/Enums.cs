using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCR.Tools.TranslationEditor.Data
{
    /// <summary>
    /// Change state of the node
    /// </summary>
    public enum NodeState
    {
        None,
        Translated,
        Outdated,
        Untranslated,
    }
}
