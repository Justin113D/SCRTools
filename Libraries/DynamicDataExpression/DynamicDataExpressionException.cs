using System;

namespace SCR.Tools.DynamicDataExpression
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
