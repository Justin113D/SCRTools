using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCR.Tools.DynamicDataExpression.Evaluate
{
    public interface IDataAccess<T>
    {
        /// <summary>
        /// Data keys used for evaluation
        /// </summary>
        public ReadOnlyDictionary<string, DataKey<T>> DataKeys { get; }
    }
}
