using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCR.Tools.DynamicDataExpression.Evaluate
{
    public sealed class DynamicDataExpressionException : Exception
    {
        /// <summary>
        /// Index in the source string
        /// </summary>
        public int Index { get; }

        public DynamicDataExpressionException(string message, int index) : base(message)
        {
            Index = index;
        }
    }
}
