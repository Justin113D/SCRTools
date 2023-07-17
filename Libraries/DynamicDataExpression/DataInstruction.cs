﻿using SCR.Tools.DynamicDataExpression.Internal;
using SCR.Tools.DynamicDataExpression.Internal.Instruction;
using System;
using System.Collections.Generic;

namespace SCR.Tools.DynamicDataExpression
{
    public enum InstructionType
    {
        set,        // =  ( x = y )
        add,        // += ( x = x + y )
        subtract,   // -= ( x = x - y )
        multiply,   // *= ( x = x * y )
        divide,     // /= ( x = x / y )
        modulo,     // %= ( x = x % y )
        exponent,   // ^= ( x = x ^ y )
        and,        // &= ( x = x & y )
        or,         // |= ( x = x | y )
    }

    public interface IDataSetter : IDataAccess
    {
        public void SetValue(string key, long? id, object value);
    }

    public class DataInstruction
    {
        public string Key { get; }

        public long? ID { get; }

        public InstructionType Type { get; }

        public DataExpression DataExpression { get; }

        internal DataInstruction(string key, long? iD, InstructionType type, DataExpression dataExpression)
        {
            Key = key;
            ID = iD;
            Type = type;
            DataExpression = dataExpression;
        }

        public void Execute(IDataSetter data)
        {
            object value = DataExpression.Evaluate(data);

            if (Type != InstructionType.set)
            {
                object prevValue = GetValue(data);

                if (Type is InstructionType.and or InstructionType.or)
                {
                    bool bValue = (bool)value;
                    bool prevBValue = (bool)prevValue;

                    value = Type == InstructionType.and
                        ? prevBValue && bValue
                        : prevBValue || bValue;
                }
                else
                {
                    double dValue = (double)value;
                    double prevdValue = (double)prevValue;

                    value = Type switch
                    {
                        InstructionType.add => prevdValue + dValue,
                        InstructionType.subtract => prevdValue - dValue,
                        InstructionType.multiply => prevdValue * dValue,
                        InstructionType.divide => prevdValue / dValue,
                        InstructionType.modulo => prevdValue % dValue,
                        InstructionType.exponent => (object)Math.Pow(prevdValue, dValue),
                        _ => throw new InvalidOperationException("Type is not valid for doubles"),
                    };
                }
            }

            try
            {
                data.SetValue(Key, ID, value);
            }
            catch(Exception e)
            {
                throw new InvalidOperationException($"Error setting {Key} {ID} to {value}: {e.GetType().Name}\n{e.Message}");
            }
        }

        public object GetValue(IDataSetter data)
        {
            try
            {
                return data.GetValue(Key, ID);
            }
            catch(Exception e)
            {
                throw new InvalidOperationException($"Error getting {Key} {ID}: {e.GetType().Name}\n{e.Message}");
            }
        }

        public string GetInstructionString()
        {
            string result = Key;
            if(ID != null)
            {
                result += ID;
            }

            result += Type switch
            {
                InstructionType.add => " +=",
                InstructionType.subtract => " -=",
                InstructionType.multiply => " *=",
                InstructionType.divide => " /=",
                InstructionType.modulo => " %=",
                InstructionType.exponent => " ^=",
                InstructionType.and => " &=",
                InstructionType.or => " |=",
                _ => " =",
            };

            return result;
        }

        public static DataInstruction ParseInstruction(
            string instruction,
            IReadOnlyDictionary<string, DataKey> setterKeys,
            IReadOnlyDictionary<string, DataKey> accessorKeys)
            => InstructionParser.ParseInstruction(instruction, setterKeys, accessorKeys);

        /// <summary>
        /// Checks if the expression is valid. <br/>
        /// Throws a <see cref="DynamicDataExpressionException"/> if not
        /// </summary>
        /// <param name="expression"></param>
        public static void Verify(string instruction)
            => InstructionParser.Verify(instruction);

        public override string ToString()
            => GetInstructionString() + " " + DataExpression.Expression;
    }
}
