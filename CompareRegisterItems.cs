using System.Collections.Generic;
using System.Linq;
namespace Gelber.ShoppingLine
{
    public class CompareRegisterItems : IComparer<Register>
    {
        /// <summary>
        /// ShortTestLine Customer
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public int Compare(Register r1, Register r2)
        {
            int sizeR1 = r1.GetCustomers().Count();
            int sizeR2 = r2.GetCustomers().Count();
            return sizeR1.CompareTo(sizeR2);
        }
    }
}
