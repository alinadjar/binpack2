using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPsolve1.SubsetSum
{
    public class Subset_Sum
    {
        // recursive function --> of exponential time
        public static bool General(int[] set, int n, int sum)
        {
            // Returns true if there is a subset of set[] with sum 
            // equal to given sum 

            // Base Cases 
            if (sum == 0)
            {

                return true;
            }

            if (n == 0 && sum != 0)
                return false;

            // If last element is greater than sum,  
            // then ignore it 
            if (set[n - 1] > sum)
                return General(set, n - 1, sum);

            /* else, check if sum can be obtained  
            by any of the following 
            (a) including the last element 
            (b) excluding the last element */
            return General(set, n - 1, sum) || General(set, n - 1, sum - set[n - 1]);
        }


    }
}
