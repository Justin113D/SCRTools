using System;
using System.Collections;
using System.Collections.Generic;

namespace SCR.Expression.Internal
{
    internal interface IStackBlock<T>
    {
        public int Evaluate(T data, IDataAccess<T> accessor, object[] stack, int stackPointer);
    }

    internal struct StackValueBlock<T> : IStackBlock<T>
    {
        private readonly string key;

        private readonly long? id;

        public StackValueBlock(string key, long? id)
        {
            this.key = key;
            this.id = id;
        }

        public int Evaluate(T data, IDataAccess<T> accessor, object[] stack, int stackPointer)
        {
            DataKey<T> dk = string.IsNullOrWhiteSpace(key) ? DataKey<T>.NumberDataKey : accessor.DataKeys[key];
            KeyType type = id == null ? dk.NoIDType : dk.IDType;
            object value = dk.GetValue(id, data);

            // check if the value is correct
            string expected = "";
            switch(type)
            {
                case KeyType.Boolean:
                    if(value is not bool)
                    {
                        expected = "boolean";
                        goto default;
                    }
                    break;
                case KeyType.Number:
                    try
                    {
                        value = Convert.ToInt64(value);
                    }
                    catch
                    {
                        expected = "number";
                        goto default;
                    }
                    break;
                case KeyType.NumberList:
                    if(value.GetType() == typeof(long[]))
                        break;

                    expected = "number array/enumerable";
                    if(value is ICollection c)
                    {
                        long[] result = new long[c.Count];

                        int i = 0;
                        try
                        {
                            foreach(object o in c)
                            {
                                result[i] = Convert.ToInt64(o);
                                i++;
                            }
                        }
                        catch
                        {
                            goto default;
                        }

                        value = result;
                    }
                    else if(value is IEnumerable e)
                    {
                        List<long> result = new();
                        try
                        {
                            foreach(object o in e)
                                result.Add(Convert.ToInt64(o));
                        }
                        catch
                        {
                            goto default;
                        }

                        value = result.ToArray();
                    }
                    else
                        goto default;

                    break;
                default:
                    throw new InvalidOperationException($"{key}{id}: Expected {expected}, got {value.GetType()}");
            }

            stack[stackPointer] = value;
            stackPointer++;
            return stackPointer;
        }

        public override string ToString() 
            => $"{(id < 0 ? "~" : "")}{key}{(id < 0 ? -id : id)}";
    }

    internal struct StackOperatorBlock<T> : IStackBlock<T>
    {
        private readonly CheckOperator op;

        public StackOperatorBlock(CheckOperator op) 
            => this.op = op;

        public int Evaluate(T data, IDataAccess<T> accessor, object[] stack, int stackPointer)
        {
            if(op.Needs2Operands())
            {
                stackPointer--;
                object left = stack[stackPointer];
                object right = stack[stackPointer-1];
                object result = null;

                if(left is bool bl && right is bool br)
                {
                    switch(op)
                    {
                        case CheckOperator.Or:
                            result = bl || br;
                            break;
                        case CheckOperator.And:
                            result = bl && br;
                            break;
                        case CheckOperator.Equals:
                            result = bl == br;
                            break;
                        case CheckOperator.Unequals:
                            result = bl != br;
                            break;
                    }
                }
                else if(left is long nl && right is long nr)
                {
                    switch(op)
                    {
                        case CheckOperator.Equals:
                            result = nl == nr;
                            break;
                        case CheckOperator.Unequals:
                            result = nl != nr;
                            break;
                        case CheckOperator.Greater:
                            result = nl > nr;
                            break;
                        case CheckOperator.GreaterEquals:
                            result = nl >= nr;
                            break;
                        case CheckOperator.Smaller:
                            result = nl < nr;
                            break;
                        case CheckOperator.SmallerEquals:
                            result = nl <= nr;
                            break;
                        case CheckOperator.Add:
                            result = nl + nr;
                            break;
                        case CheckOperator.Subtract:
                            result = nl - nr;
                            break;
                        case CheckOperator.Multiply:
                            result = nl * nr;
                            break;
                        case CheckOperator.Divide:
                            result = nl / nr;
                            break;
                        case CheckOperator.Modulo:
                            result = nl % nr;
                            break;
                        case CheckOperator.Exponent:
                            result = (long)Math.Pow(nl, nr);
                            break;
                    }
                }
                else if(left is long[] a && right is long n)
                {

                }

                stack[stackPointer - 1] = result;
            }
            else
            {
                int outp = stackPointer - 1;
                object value = stack[outp];

                if(op == CheckOperator.Invert)
                {
                    stack[outp] = !(bool)value;
                }
                else // must be negate
                {
                    stack[outp] = -(long)value;
                }
            }

            return stackPointer;
        }

        public override string ToString()
            => op.ToStringExtension();
    }
}
