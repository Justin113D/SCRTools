using SCR.Tools.DynamicDataExpression.Internal;
using System;
using System.Collections.Generic;

namespace SCR.Tools.DynamicDataExpression.Evaluate
{
    public static class DataExpression
    {
        /// <summary>
        /// Checks if the expression is valid. <br/>
        /// Throws a <see cref="DynamicDataExpressionException"/> if not
        /// </summary>
        /// <param name="expression"></param>
        public static void ValidateExpression(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                throw new ArgumentException("Expression cannot be empty!", nameof(expression));

            StateMachine sm = new();

            for (int i = 0; i < expression.Length; i++)
            {
                char c = expression[i];

                if (c == ' ')
                    continue;

                sm.NextState(c, i);
            }

            if (!sm.IsExitState())
            {
                throw new DynamicDataExpressionException($"Expression ends invalid! {sm.GetCurrentErrorMessage()}", expression.Length);
            }
        }
    }

    public sealed class DataExpression<T>
    {
        #region Regular Expressions

        private const string _legalChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789. -!#|&<>=+-*/%^()";

        private static readonly HashSet<char> _legalCharsMap;

        static DataExpression()
        {
            _legalCharsMap = new HashSet<char>();
            for (int i = 0; i < _legalChars.Length; i++)
                _legalCharsMap.Add(_legalChars[i]);
        }

        #endregion

        private readonly IDataAccess<T> _dataAccessor;

        private readonly IStackBlock<T>[] _calcStack;

        private readonly int _valueStackSize;

        private DataExpression(IStackBlock<T>[] calcStack, int valueStackSize, IDataAccess<T> dataAccessor)
        {
            _calcStack = calcStack;
            _valueStackSize = valueStackSize;
            _dataAccessor = dataAccessor;
        }

        public bool Evaluate(T data)
        {
            object[] stack = new object[_valueStackSize];
            int stackPointer = 0;

            for (int i = 0; i < _calcStack.Length; i++)
                stackPointer = _calcStack[i].Evaluate(data, _dataAccessor, stack, stackPointer);

            return (bool)stack[0];
        }

        public static DataExpression<T> ParseExpression(string expression, IDataAccess<T> accessor)
        {
            if (string.IsNullOrWhiteSpace(expression))
                throw new ArgumentException("Expression cannot be empty!", nameof(expression));

            if (accessor == null)
                throw new ArgumentNullException(nameof(accessor), "Accessor cannot be null!");

            for (int i = 0; i < expression.Length; i++)
            {
                if (!_legalCharsMap.Contains(expression[i]))
                {
                    throw new DynamicDataExpressionException($"Illegal character \"{expression[i]}\"", i);
                }
            }

            List<ISplitBlock> blocks = new();

            SplitExpression(expression, blocks, accessor);

            blocks = ConvertToPrefix(blocks);

            int valueStackSize = Validate(blocks);

            IStackBlock<T>[] result = GetCalcStack(blocks);

            return new(result, valueStackSize, accessor);
        }

        /// <summary>
        /// Splits the expression into its components
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="blocks"></param>
        /// <param name="accessor"></param>
        private static void SplitExpression(string expression, List<ISplitBlock> blocks, IDataAccess<T> accessor)
        {
            StateMachine sm = new();

            int currentIndex = 0;
            string current = "";

            int bracketStackCount = 0;

            for (int i = 0; i < expression.Length; i++)
            {
                char c = expression[i];

                if (c == ' ')
                    continue;

                bool? blockstate = sm.NextState(c, i);

                if (blockstate.HasValue && current.Length > 0)
                {
                    if (blockstate.Value) // is operation
                    {
                        blocks.Add(new OperatorBlock(current, currentIndex));
                    }
                    else // is value expression
                    {
                        blocks.Add(new ValueBlock<T>(current, accessor, currentIndex));
                    }
                    current = "";
                }

                if (c == '(')
                {
                    bracketStackCount++;
                    blocks.Add(new BracketBlock(false, current.Length > 0 ? current[0] : default, i));
                    current = "";
                }
                else if (c == ')')
                {
                    bracketStackCount--;
                    if (bracketStackCount < 0)
                    {
                        throw new DynamicDataExpressionException($"Unexpected closed bracket", i);
                    }
                    blocks.Add(new BracketBlock(true, default, i));
                    current = "";
                }
                else
                {
                    current += c;
                    if (current.Length == 1)
                        currentIndex = i;
                }
            }

            if (!sm.IsExitState())
                throw new DynamicDataExpressionException($"Expression ends invalid! {sm.GetCurrentErrorMessage()}", expression.Length);

            if (current.Length > 0)
            {
                blocks.Add(new ValueBlock<T>(current, accessor, currentIndex));
            }

            while (bracketStackCount > 0)
            {
                blocks.Add(new BracketBlock(true, default, expression.Length));
                bracketStackCount--;
            }
        }

        /// <summary>
        /// Converts the infix list to a prefix list
        /// </summary>
        /// <param name="infix"></param>
        /// <returns></returns>
        private static List<ISplitBlock> ConvertToPrefix(List<ISplitBlock> infix)
        {
            List<ISplitBlock> result = new();
            Stack<ISplitBlock> operatorStack = new();

            for (int i = infix.Count - 1; i >= 0; i--)
            {
                ISplitBlock current = infix[i];
                if (current is ValueBlock<T> v)
                {
                    result.Add(v);
                    continue;
                }
                else if (current is BracketBlock b)
                {
                    if (b.Closed)
                    {
                        operatorStack.Push(b);
                        continue;
                    }

                    ISplitBlock ob = operatorStack.Pop();
                    while (ob is not BracketBlock)
                    {
                        result.Add(ob);
                        ob = operatorStack.Pop();
                    }

                    if (b.Invert.HasValue)
                        operatorStack.Push(b.Invert.Value);
                }
                else if (current is OperatorBlock o)
                {
                    while (operatorStack.Count > 0
                        && operatorStack.Peek() is OperatorBlock ob
                        && o.OP.HasLowerPrecedence(ob.OP))
                    {
                        result.Add(operatorStack.Pop());
                    }

                    operatorStack.Push(o);
                }
            }

            while (operatorStack.Count > 0)
            {
                ISplitBlock block = operatorStack.Pop();
                if (block is OperatorBlock)
                    result.Add(block);
            }

            return result;
        }

        /// <summary>
        /// Throws an error if any component of the calculation doesnt work out and returns the required stack size
        /// </summary>
        /// <param name="prefix"></param>
        private static int Validate(List<ISplitBlock> prefix)
        {
            Stack<KeyType> valueStack = new();
            int stackMax = 0;

            foreach (var b in prefix)
            {
                if (b is ValueBlock<T> v)
                {
                    valueStack.Push(v.RealType);
                    if (valueStack.Count > stackMax)
                        stackMax = valueStack.Count;
                }
                else if (b is OperatorBlock o)
                {
                    if (o.OP.Needs2Operands())
                    {
                        KeyType left = valueStack.Pop();
                        KeyType right = valueStack.Pop();
                        KeyType r = o.OP.EvalType(left, right);

                        if (r == KeyType.None)
                        {
                            throw new DynamicDataExpressionException($"Operator \"{o.OP}\" cannot be applied to types {left} and {right}", o.Index);
                        }

                        valueStack.Push(r);
                    }
                    else
                    {
                        KeyType keytype = valueStack.Pop();
                        KeyType r = o.OP.EvalType(keytype);

                        if (r == KeyType.None)
                        {
                            throw new DynamicDataExpressionException($"Operator \"{o.OP}\" cannot be applied to the type {keytype}", o.Index);
                        }

                        valueStack.Push(r);
                    }
                }
            }

            KeyType result = valueStack.Pop();

            if (result != KeyType.Boolean)
            {
                throw new DynamicDataExpressionException($"Resulting type of the expression is {result}, expecting boolean!", -1);
            }

            return stackMax;
        }

        private static IStackBlock<T>[] GetCalcStack(List<ISplitBlock> prefix)
        {
            IStackBlock<T>[] result = new IStackBlock<T>[prefix.Count];

            for (int i = 0; i < prefix.Count; i++)
            {
                ISplitBlock block = prefix[i];
                if (block is OperatorBlock o)
                {
                    result[i] = new StackOperatorBlock<T>(o.OP);
                }
                else if (block is ValueBlock<T> v)
                {
                    result[i] = new StackValueBlock<T>(v.Handle, v.ID, v.Invert);
                }
            }

            return result;
        }
    }
}