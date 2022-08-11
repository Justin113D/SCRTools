using SCR.Tools.DynamicDataExpression;
using SCR.Tools.DynamicDataExpression.Evaluate;
using System.Collections.ObjectModel;

namespace DynamicDataExpression.Tests
{
    public class TestData
    {

    }

    public class TestAccessor : IDataAccess<TestData>
    {
        public ReadOnlyDictionary<string, DataAccessKey<TestData>> DataAccessKeys { get; }

        public TestAccessor()
        {
            #pragma warning disable IDE0055
            Dictionary<string, DataAccessKey<TestData>> dataKeys = new()
            {
                { "N", new("Number", KeyType.Number, KeyType.None, GetNumber) },
                { "B", new("Boolean", KeyType.Boolean, KeyType.None, GetBoolean) },
                { "L", new("List", KeyType.NumberList, KeyType.None, GetList) }
            };
            #pragma warning restore IDE0055


            DataAccessKeys = new(dataKeys);
        }

        private object GetNumber(double? id, TestData data)
        {
            if(id == null)
            {
                return new InvalidOperationException();
            }

            return id;
        }

        private object GetBoolean(double? id, TestData data)
        {
            if (id == null)
            {
                return new InvalidOperationException();
            }

            return id > 0;
        }

        private object GetList(double? id, TestData data)
        {
            if (id == null)
            {
                return new InvalidOperationException();
            }

            int length = (int)id;
            int[] result = new int[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = i;
            }
            return result;
        }
    }
}
