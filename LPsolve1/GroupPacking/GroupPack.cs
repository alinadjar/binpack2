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


        public static List<Logger> logger = new List<Logger>();

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
                logger.Add(new Logger { CheqID=cheq.ID, Base=cheq.ValueBase, currentValue = cheq.ValueCurrent, factor = "Holding", mandeFactor = -1 });
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
            foreach(Cheq j in Bins.Where(b => b.Title == "Holding").SingleOrDefault().Container)
                Console.WriteLine("         ||| ID Cheq: " + j.ID + " Base: " + j.ValueBase + " Current:"+ j.ValueCurrent);

            Write.Log_2_Excel_4_GroupPack(Bins);
            Write.Log_2_Excel_logger_GroupPack(logger);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sortCheqASC"></param>
        private static void Run_GroupFit(bool sortCheqASC)
        {
            List<Cheq> sortedCheqs = null;
            

            List<string> list_CodeMarkaz = Bins.Where(b => b.Title != "Holding").Select(i => i.CodeMarkaz).Distinct().ToList();

            Dictionary<string, long> priorutyDic = new Dictionary<string, long>();
            foreach (string codeMarkaz in list_CodeMarkaz)
                priorutyDic.Add(codeMarkaz, RemainAvailable(Bins.Where(m => m.CodeMarkaz == codeMarkaz).ToList()));

            list_CodeMarkaz = priorutyDic.OrderByDescending(d => d.Value).Select(k => k.Key).ToList();
            list_CodeMarkaz.ForEach(codeMarkaz =>
            {

                if (sortCheqASC)
                    sortedCheqs = Cheqs.OrderBy(x => x.ValueCurrent).ToList();
                else
                    sortedCheqs = Cheqs.OrderByDescending(x => x.ValueCurrent).ToList();


                foreach (Cheq cheq in sortedCheqs)
                {
                    if (cheq.ValueCurrent > 0)
                        Assign_To_Markaz(cheq, codeMarkaz);
                }
            });
        }

        private static bool Assign_To_Markaz(Cheq cheq, string codeMarkaz)
        {
            try
            {
                List<Bin> filteredByMarkaz = Bins.Where(m => m.CodeMarkaz == codeMarkaz).ToList();

                long remainBedehiMarkaz = RemainAvailable(filteredByMarkaz);
                if (cheq.ValueCurrent <= remainBedehiMarkaz)
                {
                    foreach (Bin bin in filteredByMarkaz.OrderBy(x => x.CurrentBedehi).Where(y => y.CurrentBedehi > 0).ToList())
                    {
                        if (cheq.ValueCurrent > 0 && cheq.ValueCurrent <= bin.CurrentBedehi)
                        {
                            Console.WriteLine("============Cheq " + cheq.ID + " value: " + cheq.ValueCurrent + "==> " + bin.Title + " bin.mande = " + bin.CurrentBedehi);
                            logger.Add(new Logger { CheqID = cheq.ID , currentValue = cheq.ValueCurrent , factor = bin.Title , mandeFactor = bin.CurrentBedehi , Base = cheq.ValueBase});
                            //bin.Container.Add(cheq);
                            bin.cheqDetails.Add(cheq.ID, cheq.ValueCurrent.ToString());
                            bin.CurrentBedehi = bin.CurrentBedehi - cheq.ValueCurrent;                                                      
                            cheq.ValueCurrent = 0;
                            break;
                        }                        
                        else if(cheq.ValueCurrent > 0)
                        {
                            Console.WriteLine("@@#########Cheq "+ cheq.ID+ " value: "+cheq.ValueCurrent +"==> "+bin.Title+ " bin.mande = "+bin.CurrentBedehi);
                            logger.Add(new Logger { CheqID = cheq.ID, currentValue = cheq.ValueCurrent, factor = bin.Title, mandeFactor = bin.CurrentBedehi, Base = cheq.ValueBase });

                            //bin.Container.Add(cheq);
                            bin.cheqDetails.Add(cheq.ID, bin.CurrentBedehi.ToString());
                            cheq.ValueCurrent = cheq.ValueCurrent - bin.CurrentBedehi;
                            bin.CurrentBedehi = 0;                            
                        }
                    }// end for

                    //cheq.ValueCurrent = 0;
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
