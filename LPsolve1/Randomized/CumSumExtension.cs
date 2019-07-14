using LPsolve1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPsolve1.Randomized
{
    public static class CumSumExtension
    {
        public static List<Cheq> filterCumSum(this List<Cheq> list, long upto)
        {
            long sum = 0;
            List<Cheq> result = new List<Cheq>();
            foreach(Cheq cheq in list.OrderBy(x => x.ValueBase).Where(x => x.ValueBase <= 4*upto))
            {
                sum += cheq.ValueBase;
                if (sum < upto)
                    result.Add(cheq);
                else break;
            }

            return result;
        }
    }
}
