using LPsolve1.Commons;
using LPsolve1.Excel;
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


            // add the remaining cheqs to Holding
            Bin holding = Bins.Where(b => b.Title == "Holding").SingleOrDefault();
            Cheqs.Where(c => c.ValueCurrent > 0).ToList().ForEach(cheq =>
            {
                holding.Container.Add(cheq);
                holding.Base += cheq.ValueCurrent;
                holding.CurrentBedehi += cheq.ValueCurrent;
            });



            //------------------------------print results:

            foreach (var bin in Bins)
            {
                Console.WriteLine(bin.CodeMarkaz + ":" + bin.Title + "  ----------------------------- " + "Base = " + bin.Base + "  current = " + bin.CurrentBedehi);
                foreach (var i in bin.cheqDetails)
                    Console.WriteLine("         ||| ID Cheq: " + i.Key + " Amount Delivered: " + i.Value);
            }

            Write.Log_2_Excel_4_GroupPack(Bins);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sortCheqASC"></param>
        private static void Run_GroupFit(bool sortCheqASC)
        {
            List<Cheq> sortedCheqs = null;

            if (sortCheqASC)
                sortedCheqs = Cheqs.OrderBy(x => x.ValueBase).ToList();
            else
                sortedCheqs = Cheqs.OrderByDescending(x => x.ValueBase).ToList();


            List<string> list_CodeMarkaz = Bins.Select(i => i.CodeMarkaz).Distinct().ToList();

            list_CodeMarkaz.ForEach(codeMarkaz =>
            {
                foreach (Cheq cheq in sortedCheqs)
                {
                    if (cheq.ValueCurrent != 0)
                        Assign_To_Markaz(cheq, codeMarkaz);
                }
            });
        }

        private static bool Assign_To_Markaz(Cheq cheq, string codeMarkaz)
        {
            try
            {
                List<Bin> filteredByMarkaz = Bins.Where(m => m.CodeMarkaz == codeMarkaz).ToList();

                if (cheq.ValueCurrent <= RemainAvailable(filteredByMarkaz))
                {
                    foreach (Bin bin in filteredByMarkaz.OrderBy(x => x.CurrentBedehi))
                    {
                        if (cheq.ValueCurrent <= bin.CurrentBedehi)
                        {
                            bin.CurrentBedehi -= cheq.ValueCurrent;
                            bin.Container.Add(cheq);
                            bin.cheqDetails.Add(cheq.ID, cheq.ValueCurrent);
                            break;
                        }
                        else
                        {
                            cheq.ValueCurrent -= bin.CurrentBedehi;
                            bin.CurrentBedehi = 0;
                            bin.Container.Add(cheq);
                            bin.cheqDetails.Add(cheq.ID, bin.CurrentBedehi);
                        }
                    }// end for

                    cheq.ValueCurrent = 0;
                    return true;
                }
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
                //throw;
            }
        }

        private static long RemainAvailable(List<Bin> filteredByMarkaz)
        {
            return filteredByMarkaz.Sum(s => s.CurrentBedehi);
        }
    }
}
