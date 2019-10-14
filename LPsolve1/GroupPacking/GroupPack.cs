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
        public static List<Cheq> Cheqs = null; // لیست همه چک ها
        public static List<Bin> Bins = null; // لیست همه فاکتورها
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
                //logger.Add(new Logger { CheqID=cheq.ID, Base=cheq.ValueBase, currentValue = cheq.ValueCurrent, factor = "Holding", mandeFactor = -1 });
                holding.Container.Add(cheq);
                holding.Base += cheq.ValueCurrent;
                holding.CurrentBedehi += cheq.ValueCurrent;

            });



            //------------------------------print log results:

            foreach (var bin in Bins)
            {
                Console.WriteLine(bin.CodeMarkaz + ":" + bin.Title + "  ----------------------------- " + "Base = " + bin.Base + "  current = " + bin.CurrentBedehi);
                foreach (var i in bin.CheqDetails)
                    Console.WriteLine("         ||| ID Cheq: " + i.Key + " Amount Delivered: " + i.Value);
            }
            foreach(Cheq j in Bins.Where(b => b.Title == "Holding").SingleOrDefault().Container)
                Console.WriteLine("         ||| ID Cheq: " + j.ID + " Base: " + j.ValueBase + " Current:"+ j.ValueCurrent);

            Write.Log_2_Excel_4_GroupPack(Bins);
            Write.Log_2_Excel_logger_GroupPack(logger);
        }


        /// <summary>
        /// الگوریتم تخصیص چک ها به مراکز
        /// ابتدا مراکز بر حسب مجموع کل بدهی فاکتورهایشان بصورت نزولی مرتب شده
        /// و سپس هر چک ، در صورتی به مرکز تحویل میشود که مبلغ چک کمتر از کل بدهی مرکز باشد
        /// هنگام تحویل چک به مرکز، ابتدا فاکتورهای مرکز بطور صعودی مرتب شده و سپس چک به فاکتورها تخصیص داده میشود
        /// چنانچه مبلغ چک بیشتر از فاکتور باشد، طبیعتا چک ، آن فاکتور را صفر میکند و باقیمانده چک برای سایر فاکتور ها اعمال میگردد
        /// این روال برای هر مرکز تکرار میشود
        /// در پایان، چک های باقیمانده صرفا در هلدینگ قرار میگیرند
        /// </summary>
        /// <param name="sortCheqASC"></param>
        private static void Run_GroupFit(bool sortCheqASC = true)
        {
            List<Cheq> sortedCheqs = null;
            

            List<string> list_CodeMarkaz = Bins.Where(b => b.Title != "Holding").Select(i => i.CodeMarkaz).Distinct().ToList();




            /*
             Dictionary priorityDic ====> مرتب سازی نزولی مراکز بر حسب کل بدهی هر مرکز

              کل مانده بدهی مرکز  | کدمرکز    
            -------------------------------
            مبلغ کل بدهی شهداب  | کد شهداب  
           مبلغ کل بدهی بهاران  | کد بهاران  

            */

            Dictionary<string, long> priorityDic = new Dictionary<string, long>();
            foreach (string codeMarkaz in list_CodeMarkaz)
                priorityDic.Add(codeMarkaz, RemainAvailable(Bins.Where(m => m.CodeMarkaz == codeMarkaz).ToList()));

            list_CodeMarkaz = priorityDic.OrderByDescending(d => d.Value).Select(k => k.Key).ToList();

            // حلقه تخصیص چک ها 
            // هر بار تکرار حلقه، تسویه بدهی های یک مرکز
            list_CodeMarkaz.ForEach(codeMarkaz =>
            {

                if (sortCheqASC)
                    sortedCheqs = Cheqs.Where(c => c.ValueCurrent > 0).OrderBy(x => x.ValueCurrent).ToList();
                else
                    sortedCheqs = Cheqs.Where(c => c.ValueCurrent > 0).OrderByDescending(x => x.ValueCurrent).ToList();


                foreach (Cheq cheq in sortedCheqs)
                {
                    if (cheq.ValueCurrent > 0)
                        Assign_To_Markaz(cheq, codeMarkaz); //  تخصیص چک به مرکز
                }
            });
        }

        private static bool Assign_To_Markaz(Cheq cheq, string codeMarkaz)
        {
            try
            {
                // Bins are factors
                // filtering the factors of the requested codeMarkaz, e.g. 'شهداب'
                List<Bin> filteredByMarkaz = Bins.Where(m => m.CodeMarkaz == codeMarkaz).ToList();

                // مجموع بدهی فاکتورهای مرکز
                long remainBedehiMarkaz = RemainAvailable(filteredByMarkaz);


                if (cheq.ValueCurrent <= remainBedehiMarkaz)
                {
                    // حلقه روی فاکتورهای مرکز معین شده 
                    foreach (Bin bin in filteredByMarkaz.OrderBy(x => x.CurrentBedehi).Where(y => y.CurrentBedehi > 0).ToList())
                    {
                        if (cheq.ValueCurrent > 0 && cheq.ValueCurrent <= bin.CurrentBedehi)
                        {
                            // Logging:
                            Console.WriteLine("============Cheq " + cheq.ID + " value: " + cheq.ValueCurrent + "==> " + bin.Title + " bin.mande = " + bin.CurrentBedehi);
                            logger.Add(new Logger { CheqID = cheq.ID , currentValue = cheq.ValueCurrent , factor = bin.Title , mandeFactor = bin.CurrentBedehi , Base = cheq.ValueBase});
                            
                            //bin.Container.Add(cheq);  // ==> used prevously in BestFit Algorithm


                            bin.AddAmountReceivedByCheq(cheq.ID, cheq.ValueCurrent);
                            // the above line is equivalent to :
                            // cheqDetails.Add(cheq.ID, cheq.ValueCurrent.ToString());


                            bin.CurrentBedehi = bin.CurrentBedehi - cheq.ValueCurrent;                                                      
                            cheq.ValueCurrent = 0;
                            break;
                        }                        
                        else if(cheq.ValueCurrent > 0)
                        {
                            // some Logging:
                            Console.WriteLine("@@#########Cheq "+ cheq.ID+ " value: "+cheq.ValueCurrent +"==> "+bin.Title+ " bin.mande = "+bin.CurrentBedehi);
                            logger.Add(new Logger { CheqID = cheq.ID, currentValue = cheq.ValueCurrent, factor = bin.Title, mandeFactor = bin.CurrentBedehi, Base = cheq.ValueBase });

                            //bin.Container.Add(cheq);  // ==> used prevously in BestFit Algorithm


                            bin.AddAmountReceivedByCheq(cheq.ID, bin.CurrentBedehi);
                            // the above line is equivalent to :
                            //bin.CheqDetails.Add(cheq.ID, bin.CurrentBedehi);

                            cheq.ValueCurrent = cheq.ValueCurrent - bin.CurrentBedehi;
                            bin.CurrentBedehi = 0;                            
                        }
                    }// end for

                    return true;
                }
                else
                    return false; // چون مبلغ چک، بیشتر از کل بدهی مرکز است
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in Assign_To_Markaz Method: " +ex.Message);
                return false;
            }
        }


        /// <summary>
        /// محاسبه مجموع بدهی فاکتورهای ارسال شده بعنوان پارامتر
        /// </summary>
        /// <param name="filteredByMarkaz">لیست فاکتورهای یک مرکز</param>
        /// <returns></returns>
        private static long RemainAvailable(List<Bin> filteredByMarkaz)
        {
            return filteredByMarkaz.Sum(s => s.CurrentBedehi);
        }
    }
}
