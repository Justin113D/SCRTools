using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCR.Expression
{
    /// <summary>
    /// Key return type
    /// </summary>
    public enum KeyType
    {
        /// <summary>
        /// None, Key doesnt support this type
        /// </summary>
        None,

        /// <summary>
        /// Returns a boolean
        /// </summary>
        Boolean,

        /// <summary>
        /// Return a number
        /// </summary>
        Number,

        /// <summary>
        /// Returns a number array
        /// </summary>
        NumberList
    }

    /// <summary>
    /// Contains data to determine how an expression key should be 
    /// </summary>
    public struct DataKey<T>
    {
        public static readonly DataKey<T> NumberDataKey = new("Number", KeyType.Number, KeyType.None, (id, _) => id);
            
        /// <summary>
        /// Name of the Key
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Returned datatype when using an ID
        /// </summary>
        public KeyType IDType { get; }

        /// <summary>
        /// Returned datatype when not using an ID
        /// </summary>
        public KeyType NoIDType { get; }

        /// <summary>
        /// Takes an ID and source object and returns the according value
        /// </summary>
        public Func<long?, T, object> GetValue { get; }

        public DataKey(string name, KeyType iDType, KeyType noIDType, Func<long?, T, object> getValue)
        {
            Name = name;
            IDType = iDType;
            NoIDType = noIDType;
            GetValue = getValue;
        }
    }
}
