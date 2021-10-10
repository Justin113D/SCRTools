using System;
using System.Collections;
using System.Collections.Generic;

namespace SCR.Expression.Internal
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
        public long? ID { get; }

        /// <summary>
        /// Source string index
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Value type
        /// </summary>
        public KeyType Type 
            => ID == null ? Key.NoIDType : Key.IDType;

        public ValueBlock(string value, IDataAccess<T> da, int index)
        {
            Index = index;

            string pre = null;
            if(value[0] is '!' or '-')
            {
                pre = value[0].ToString();
                value = value.Remove(0, 1);
            }

            // Get the key
            Handle = "";
            while(value.Length > 0 && !char.IsDigit(value[0]))
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
                ID = int.Parse(value);
                if(pre != null)
                    ID = -ID;
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

            if(type == KeyType.Number && pre == "!")
                throw new DynamicDataExpressionException($"Invalid value operator \"!\"! Number values only allow \"-\".", index);
            else if(type == KeyType.Boolean && pre == "-")
                throw new DynamicDataExpressionException($"Invalid value operator \"-\"! Boolean values only allow \"!\".", index);
            else if(type == KeyType.NumberList && pre != null)
                throw new DynamicDataExpressionException($"Invalid value operator! Number lists don't support value operators.", index);
        }

        public object Evaluate(T data)
        {
            object value = Key.GetValue(ID, data);

            if(value == null)
            {
                throw new DynamicDataExpressionException($"Value {this} returns null!", Index);
            }

            // check if the value is correct
            string expected = "";
            switch(Type)
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
                    throw new DynamicDataExpressionException($"Value {this} does not return according type!\n Expected: {expected}\n Received: {value.GetType()}", Index);
            }

            return value;
        }

        public override string ToString()
        {
            string result = $"{Handle}{(ID < 0 ? -ID : ID)}";
            if(ID < 0)
            {
                switch(Type)
                {
                    case KeyType.Boolean:
                        result = "!" + result;
                        break;
                    case KeyType.Number:
                        result = "-" + result;
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
                Invert = new(invert == '!' ? CheckOperator.Invert : CheckOperator.Negate, index);
            }
        }

        public override string ToString()
            => $"{Invert?.OP.ToStringExtension() ?? ""}{(Closed ? ")" : "(")}";
    }
}
