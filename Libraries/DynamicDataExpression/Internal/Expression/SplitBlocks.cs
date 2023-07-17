using System.Collections.Generic;

namespace SCR.Tools.DynamicDataExpression.Internal.Expression
{
    internal interface ISplitBlock
    {
        public int Index { get; }
    }

    /// <summary>
    /// A value expression block
    /// </summary>
    internal struct ValueBlock : ISplitBlock
    {
        public string Handle { get; }

        /// <summary>
        /// Key of the value
        /// </summary>
        public DataKey? Key { get; }

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
        {
            get
            {
                if (Key == null)
                {
                    return KeyType.Number;
                }

                return ID == null ? Key.Value.NoIDType : Key.Value.IDType;
            }
        }

        public KeyType RealType
            => Type == KeyType.NumberList && Invert ? KeyType.Number : Type;

        internal ValueBlock(string handle, DataKey? key, double? iD, bool invert, int index)
        {
            Handle = handle;
            Key = key;
            ID = iD;
            Invert = invert;
            Index = index;
        }

        public static ValueBlock Parse(string value, IReadOnlyDictionary<string, DataKey> accessorKeys, int index)
        {
            char pre = default;
            if (value[0] is '!' or '-' or '#')
            {
                pre = value[0];
                value = value.Remove(0, 1);
            }
            bool invert = pre != default;

            // Get the key
            string handle = "";
            while (value.Length > 0 && char.IsLetter(value[0]))
            {
                handle += value[0];
                value = value.Remove(0, 1);
            }

            DataKey? key;
            if (string.IsNullOrWhiteSpace(handle))
            {
                // No key is equal to a regular number, which needs no special key
                key = null;
            }
            else
            {
                if (!accessorKeys.TryGetValue(handle, out DataKey dKey))
                {
                    throw new DynamicDataExpressionException($"Key \"{handle}\"does not exist in data accessor!", index);
                }
                key = dKey;
            }

            // get the ID value
            KeyType type;
            double? id;
            if (key == null)
            {
                id = double.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
                type = KeyType.Number;
            }
            else if (value.Length > 0)
            {
                id = double.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
                type = key.Value.IDType;
                if (type == KeyType.None)
                    throw new DynamicDataExpressionException($"Key \"{handle}\"does not support usage with Index!", index);
            }
            else
            {
                id = null;
                type = key.Value.NoIDType;
                if (type == KeyType.None)
                    throw new DynamicDataExpressionException($"Key \"{handle}\" does not support usage without Index!", index);
            }

            if (invert)
            {
                if (type == KeyType.Number && pre != '-')
                    throw new DynamicDataExpressionException($"Invalid value operator {pre}! Number values only allow \"-\".", index);
                else if (type == KeyType.Boolean && pre != '!')
                    throw new DynamicDataExpressionException($"Invalid value operator {pre}! Boolean values only allow \"!\".", index);
                else if (type == KeyType.NumberList && pre != '#')
                    throw new DynamicDataExpressionException($"Invalid value operator {pre}! Number lists only allow \"#\".", index);
            }

            return new(handle, key, id, invert, index);
        }

        public override string ToString()
        {
            string result = $"{Handle}{ID}";
            if (Invert)
            {
                switch (Type)
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
            => OP.ToStringExtension();
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
            if (invert == default)
                Invert = null;
            else
            {
                if (invert == '#')
                    throw new DynamicDataExpressionException("Count operator can only directly be used with Number List Keys", index);
                Invert = new(invert == '!' ? CheckOperator.Invert : CheckOperator.Negate, index);
            }
        }

        public override string ToString()
            => $"{Invert?.OP.ToStringExtension() ?? ""}{(Closed ? ")" : "(")}";
    }
}
