using SCR.Tools.DynamicDataExpression.Internal.Expression;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SCR.Tools.DynamicDataExpression
{
    public interface IDataAccess
    {
        public object GetValue(string key, long? id);
    }

    public sealed class DataExpression
    {
        private static readonly Regex sWhitespace = new Regex(@"\s+");

        public string Expression { get; }

        private readonly IStackBlock[] _calcStack;

        private readonly int _valueStackSize;

        private readonly KeyType _outputType;

        internal DataExpression(string expression, IStackBlock[] calcStack, int valueStackSize, KeyType outputType)
        {
            Expression = expression;
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

        public object Evaluate(IDataAccess data)
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
        public static void Verify(string expression)
            => ExpressionParser.Verify(expression, true);

        public static string FormatExpression(string expression)
        {
            string result = sWhitespace.Replace(expression, "");
            return result;
        }

        public override string ToString()
            => Expression;
    }
}