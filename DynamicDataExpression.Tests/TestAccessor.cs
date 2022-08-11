using SCR.Tools.DynamicDataExpression;
using System.Collections.ObjectModel;

namespace DynamicDataExpression.Tests
{
    public class TestData : IDataAccess
    {
        public ReadOnlyDictionary<string, DataKey> DataAccessKeys { get; }

        public TestData()
        {
            #pragma warning disable IDE0055
            Dictionary<string, DataKey> dataKeys = new()
            {
                { "N", new("Number", KeyType.Number, KeyType.None) },
                { "B", new("Boolean", KeyType.Boolean, KeyType.None) },
                { "L", new("List", KeyType.NumberList, KeyType.None) }
            };
            #pragma warning restore IDE0055


            DataAccessKeys = new(dataKeys);
        }

        public object GetValue(string key, long? id)
        {
            if(id == null)
            {
                return new InvalidOperationException();
            }

            switch (key)
            {
                case "N":
                    return id;
                case "B":
                    return id > 0;
                case "L":
                    int length = (int)id;
                    int[] result = new int[length];
                    for (int i = 0; i < length; i++)
                    {
                        result[i] = i;
                    }
                    return result;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
