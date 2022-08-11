using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCR.Tools.DynamicDataExpression;
using SCR.Tools.DynamicDataExpression.Evaluate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicDataExpression.Tests
{
    [TestClass]
    public class ExpressionTests
    {
        private static readonly TestAccessor Accessor = new();

        private static readonly TestData Data = new();

        private static void IntTestExpression(string expression, bool result)
        {
            Assert.AreEqual(DataExpression<TestData>
                .ParseExpression(expression, Accessor, KeyType.Boolean)
                .EvaluateBoolean(Data),
                result);
        }

        private static void IntTestExpression(string expression, int result)
        {
            Assert.AreEqual(DataExpression<TestData>
                            .ParseExpression(expression, Accessor, KeyType.Number)
                            .EvaluateNumber(Data),
                            result);
        }


        [TestMethod]
        public void IntEquals()
        {
            IntTestExpression("0 == -1", false);
            IntTestExpression("0 == 0", true);
            IntTestExpression("0 == 1", false);
        }

        [TestMethod]
        public void IntUnequals()
        {
            IntTestExpression("0 != -1", true);
            IntTestExpression("0 != 0", false);
            IntTestExpression("0 != 1", true);
        }

        [TestMethod]
        public void IntSmaller()
        {
            IntTestExpression("0 < -1", false);
            IntTestExpression("0 < 0", false);
            IntTestExpression("0 < 1", true);
        }

        [TestMethod]
        public void IntLarger()
        {
            IntTestExpression("0 > -1", true);
            IntTestExpression("0 > 0", false);
            IntTestExpression("0 > 1", false);
        }

        [TestMethod]
        public void IntSmallerEquals()
        {
            IntTestExpression("0 <= -1", false);
            IntTestExpression("0 <= 0", true);
            IntTestExpression("0 < 1", true);
        }

        [TestMethod]
        public void IntLargetEquals()
        {
            IntTestExpression("0 >= -1", true);
            IntTestExpression("0 >= 0", true);
            IntTestExpression("0 >= 1", false);
        }

        [TestMethod]
        public void IntAdd()
        {
            IntTestExpression("1 + 1", 2);
            IntTestExpression("1 + -1", 0);
            IntTestExpression("1 + 1 + 1", 3);
        }

        [TestMethod]
        public void IntSubtract()
        {
            IntTestExpression("-(-(-(-1)))", 1);
            IntTestExpression("1 - 1", 0);
            IntTestExpression("1 - -1", 2);
            IntTestExpression("1 - 1 - 1", -1);
        }

        [TestMethod]
        public void IntMultiply()
        {
            IntTestExpression("1 * 1", 1);
            IntTestExpression("1 * -1", -1);
            IntTestExpression("-1 * -1", 1);
            IntTestExpression("2 * 4", 8);
            IntTestExpression("2 * 2 * 2", 8);
        }
    }
}
