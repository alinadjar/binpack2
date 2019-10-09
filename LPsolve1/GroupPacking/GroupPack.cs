using LPsolve1.Commons;
using LPsolve1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPsolve1.GroupPacking
{
    public class GroupPack
    {
        public static List<Cheq> Cheqs = null;
        public static List<Bin> Bins = null;
        public static bool Is_TotalBedehi_more_than_sum_Cheq_values = false;

        public static void GroupFit()
        {
            Cheqs = new List<Cheq>();
            Bins = new List<Bin>();


            Is_TotalBedehi_more_than_sum_Cheq_values = ImportData.Prepare_Before_Run(Bins, Cheqs);

            bool sort_Cheqs_Ascending = true;
            Run_GroupFit(sort_Cheqs_Ascending);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sortCheqASC"></param>
        private static void Run_GroupFit(bool sortCheqASC)
        {
            List<Cheq> sortedCheqs = null;

            if(sortCheqASC)
                sortedCheqs = Cheqs.OrderBy(x => x.ValueBase).ToList();
            else
                sortedCheqs = Cheqs.OrderByDescending(x => x.ValueBase).ToList();



            foreach(Cheq cheq in sortedCheqs)
            {

            }
        }
    }
}
