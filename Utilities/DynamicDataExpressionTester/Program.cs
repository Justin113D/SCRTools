using SCR.Tools.DynamicDataExpression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCR.Tools.DynamicDataExpressionTester
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            //var checker = DataExpression<Data.Data>.ParseExpression("", Data.DataAccessor.DA);
            //var checker1 = DataExpression<Data.Data>.ParseExpression("3 * ((4", Data.DataAccessor.DA);
            //var checker2 = DataExpression<Data.Data>.ParseExpression("3 + 4 * 5 <= I40 & 1 - 5 = I20 | !F10", Data.DataAccessor.DA);
            //var checker3 = DataExpression<Data.Data>.ParseExpression("(((((3 + ((4 * 5))) <= I40) & !((1 - 5) = I20)) | !F10", Data.DataAccessor.DA);
            //var checker4 = DataExpression<Data.MockSCRData>.ParseExpression("1^2^3 > 0", Data.DataAccessor.DA);
            //var checker5 = DataExpression<Data.Data>.ParseExpression("1^(2^(3^(4^5))) > 0", Data.DataAccessor.DA);
            //var checker6 = DataExpression<Data.Data>.ParseExpression("(((1^2)^3)^4)^5 > 0", Data.DataAccessor.DA);
            //var checker7 = DataExpression<Data.Data>.ParseExpression("20 = 30 = 40", Data.DataAccessor.DA);

            //checker4.Evaluate(null);

            App.Main();
        }
    }
}
