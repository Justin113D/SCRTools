using SCR.Tools.DynamicDataExpression.Evaluate;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SCR.Tools.DynamicDataExpression.Internal
{
    internal interface ISplitBlock
    {
        public int Index { get; }
    }

    /// <summary>
    /// A value expression block
    /// </summary>
    internal struct ValueBlock<T> : ISplitBlock
    {
        public string Handle { get; }

        /// <summary>
        /// Key of the value
        /// </summary>
        public DataKey<T> Key { get; }

        /// <summary>
        /// ID of the key
        /// </summary>
        public double? ID { get; }

        /// <summary>
        /// Whether the result should be inverted/negated/summed up
        /// </summary>
        public bool Invert { get; }

        /// <summary>
        /// Source string index
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Value type
        /// </summary>
        public KeyType Type 
            => ID == null ? Key.NoIDType : Key.IDType;

        public KeyType RealType 
            => Type == KeyType.NumberList && Invert ? KeyType.Number : Type;

        public ValueBlock(string value, IDataAccess<T> da, int index)
        {
            Index = index;

            char pre = default;
            if(value[0] is '!' or '-' or '#')
            {
                pre = value[0];
                value = value.Remove(0, 1);
            }
            Invert = pre != default;

            // Get the key
            Handle = "";
            while(value.Length > 0 && char.IsLetter(value[0]))
            {
                Handle += value[0];
                value = value.Remove(0, 1);
            }

            if(string.IsNullOrWhiteSpace(Handle))
            {
                // No key is equal to a regular number, which needs no special key
                Key = DataKey<T>.NumberDataKey;
            }
            else
            {
                if(!da.DataKeys.TryGetValue(Handle, out DataKey<T> dKey))
                {
                    throw new DynamicDataExpressionException($"Key \"{Handle}\"does not exist in data accessor!", index);
                }
                Key = dKey;
            }

            // get the ID value
            KeyType type;
            if(value.Length > 0)
            {
                ID = double.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
                type = Key.IDType;
                if(type == KeyType.None)
                    throw new DynamicDataExpressionException($"Key \"{Handle}\"does not support usage without Index!", index);
            }
            else
            {
                ID = null;
                type = Key.NoIDType;
                if(type == KeyType.None)
                    throw new DynamicDataExpressionException($"Key \"{Handle}\" does not support usage with Index!", index);
            }

            if(Invert)
            {
                if(type == KeyType.Number && pre != '-')
                    throw new DynamicDataExpressionException($"Invalid value operator {pre}! Number values only allow \"-\".", index);
                else if(type == KeyType.Boolean && pre != '!')
                    throw new DynamicDataExpressionException($"Invalid value operator {pre}! Boolean values only allow \"!\".", index);
                else if(type == KeyType.NumberList && pre != '#')
                    throw new DynamicDataExpressionException($"Invalid value operator {pre}! Number lists only allow \"#\".", index);
            }

        }
        
        public override string ToString()
        {
            string result = $"{Handle}{ID}";
            if(Invert)
            {
                switch(Type)
                {
                    case KeyType.Boolean:
                        result = "!" + result;
                        break;
                    case KeyType.Number:
                        result = "-" + result;
                        break;
                    case KeyType.NumberList:
                        result = "#" + result;
                        break;
                }
            }

            return result;
        }
    }

    internal struct OperatorBlock : ISplitBlock
    {
        /// <summary>
        /// Operator of the block
        /// </summary>
        public CheckOperator OP { get; }

        /// <summary>
        /// Source string index
        /// </summary>
        public int Index { get; }

        public OperatorBlock(string op, int index)
        {
            OP = EnumExtensions.ToOperator(op);
            Index = index;
        }

        public OperatorBlock(CheckOperator op, int index)
        {
            OP = op;
            Index = index;
        }

        public override string ToString()
            => EnumExtensions.ToStringExtension(OP);
    }

    internal struct BracketBlock : ISplitBlock
    {
        /// <summary>
        /// Whether the bracket is closed
        /// </summary>
        public bool Closed { get; }

        /// <summary>
        /// Whether its got a "!" or "-" before it
        /// </summary>
        public OperatorBlock? Invert { get; }

        /// <summary>
        /// Source string index
        /// </summary>
        public int Index { get; }

        public BracketBlock(bool closed, char invert, int index)
        {
            Closed = closed;
            Index = index;
            if(invert == default)
                Invert = null;
            else
            {
                if(invert == '#')
                    throw new DynamicDataExpressionException("Count operator can only directly be used with Number List Keys", index);
                Invert = new(invert == '!' ? CheckOperator.Invert : CheckOperator.Negate, index);
            }
        }

        public override string ToString()
            => $"{Invert?.OP.ToStringExtension() ?? ""}{(Closed ? ")" : "(")}";
    }
}
