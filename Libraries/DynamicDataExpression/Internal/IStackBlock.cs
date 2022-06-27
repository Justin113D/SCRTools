﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace SCR.Tools.DynamicDataExpression.Internal
{
    internal interface IStackBlock<T>
    {
        public int Evaluate(T data, IDataAccess<T> accessor, object[] stack, int stackPointer);
    }

    internal struct StackValueBlock<T> : IStackBlock<T>
    {
        private readonly string key;

        private readonly double? id;

        private readonly bool inverted;

        public StackValueBlock(string key, double? id, bool inverted)
        {
            this.key = key;
            this.id = id;
            this.inverted = inverted;
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
                    if(value is double)
                        break;

                    try
                    {
                        value = Convert.ToDouble(value);
                    }
                    catch
                    {
                        expected = "number";
                        goto default;
                    }
                    break;
                case KeyType.NumberList:
                    if(value.GetType() == typeof(double[]))
                        break;

                    expected = "number array/enumerable";
                    if(value is ICollection c)
                    {
                        double[] result = new double[c.Count];

                        int i = 0;
                        try
                        {
                            foreach(object o in c)
                            {
                                result[i] = Convert.ToDouble(o);
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
                        List<double> result = new();
                        try
                        {
                            foreach(object o in e)
                                result.Add(Convert.ToDouble(o));
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

            if(inverted)
            {
                switch(type)
                {
                    case KeyType.Boolean:
                        value = !(bool)value;
                        break;
                    case KeyType.Number:
                        value = -(double)value;
                        break;
                    case KeyType.NumberList:
                        value = ((double[])value).Length;
                        break;
                }
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
                object right = stack[stackPointer - 1];
                object result = null;

                if(left is bool bl && right is bool br)
                {
                    result = op switch
                    {
                        CheckOperator.Or        => bl || br,
                        CheckOperator.And       => bl && br,
                        CheckOperator.Equals    => bl == br,
                        CheckOperator.Unequals  => bl != br,
                        _ => throw new DynamicDataExpressionException("How did you even come across this? Contact the dev asap", -2),
                    };
                }
                else if(left is double nl && right is double nr)
                {
                    result = op switch
                    {
                        CheckOperator.Equals        => nl == nr,
                        CheckOperator.Unequals      => nl != nr,
                        CheckOperator.Greater       => nl > nr,
                        CheckOperator.GreaterEquals => nl >= nr,
                        CheckOperator.Smaller       => nl < nr,
                        CheckOperator.SmallerEquals => nl <= nr,
                        CheckOperator.Add           => nl + nr,
                        CheckOperator.Subtract      => nl - nr,
                        CheckOperator.Multiply      => nl * nr,
                        CheckOperator.Divide        => nl / nr,
                        CheckOperator.Modulo        => nl % nr,
                        CheckOperator.Exponent      => Math.Pow(nl, nr),
                        _ => throw new DynamicDataExpressionException("How did you even come across this? Contact the dev asap", -2),
                    };
                }
                else if(left is double[] a && right is double n)
                {
                    Func<double, double, bool> check = op switch
                    {
                        CheckOperator.Equals        => (l, r) => l == r,
                        CheckOperator.Unequals      => (l, r) => l == r,
                        CheckOperator.Greater       => (l, r) => l > r,
                        CheckOperator.GreaterEquals => (l, r) => l >= r,
                        CheckOperator.Smaller       => (l, r) => l < r,
                        CheckOperator.SmallerEquals => (l, r) => l <= r,
                        _ => throw new DynamicDataExpressionException("How did you even come across this? Contact the dev asap", -2),
                    };

                    bool contains = false;
                    for(int i = 0; i < a.Length; i++)
                    {
                        if(check(a[i], n))
                        {
                            contains = true;
                            break;
                        }
                    }

                    result = op == CheckOperator.Unequals ? !contains : contains;
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
                    stack[outp] = -(double)value;
                }
            }

            return stackPointer;
        }

        public override string ToString()
            => op.ToStringExtension();
    }
}
