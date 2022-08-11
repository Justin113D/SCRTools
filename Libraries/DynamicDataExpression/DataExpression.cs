using SCR.Tools.DynamicDataExpression.Internal.Expression;
using System;
using System.Collections.Generic;

namespace SCR.Tools.DynamicDataExpression
{
    public interface IDataAccess
    {
        public object GetValue(string key, long? id);
    }

    public sealed class DataExpression
    {
        private readonly IStackBlock[] _calcStack;

        private readonly int _valueStackSize;

        private readonly KeyType _outputType;

        internal DataExpression(IStackBlock[] calcStack, int valueStackSize, KeyType outputType)
        {
            _calcStack = calcStack;
            _valueStackSize = valueStackSize;
            _outputType = outputType;
        }

        public bool EvaluateBoolean(IDataAccess data)
        {
            if (_outputType != KeyType.Boolean)
            {
                throw new InvalidOperationException($"Output type of expression is {_outputType}, not boolean!");
            }

            return (bool)Evaluate(data);
        }

        public double EvaluateNumber(IDataAccess data)
        {
            if (_outputType != KeyType.Number)
            {
                throw new InvalidOperationException($"Output type of expression is {_outputType}, not number!");
            }

            return (double)Evaluate(data);
        }

        public double[] EvaluateNumberList(IDataAccess data)
        {
            if (_outputType != KeyType.NumberList)
            {
                throw new InvalidOperationException($"Output type of expression is {_outputType}, not number list!");
            }

            return (double[])Evaluate(data);
        }

        internal object Evaluate(IDataAccess data)
        {
            object[] stack = new object[_valueStackSize];
            int stackPointer = 0;

            for (int i = 0; i < _calcStack.Length; i++)
                stackPointer = _calcStack[i].Evaluate(data, stack, stackPointer);

            return stack[0];
        }

        public static DataExpression ParseExpression(string expression, IReadOnlyDictionary<string, DataKey> accessorKeys, KeyType expectedOutput)
            => ExpressionParser.ParseExpression(expression, accessorKeys, expectedOutput, true);

        /// <summary>
        /// Checks if the expression is valid. <br/>
        /// Throws a <see cref="DynamicDataExpressionException"/> if not
        /// </summary>
        /// <param name="expression"></param>
        public static void ValidateExpression(string expression)
            => ExpressionParser.Verify(expression, true);
    }
}