using LPsolve1.Models;
using LPsolve1.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LPsolve1.Commons;

namespace LPsolve1.BinPacking.BestFit
{
    public class BinPack
    {

        public static List<Cheq> Cheqs = null;
        public static List<Bin> Bins = null;
        public static bool Is_TotalBedehi_more_than_sum_Cheq_values = false;


        public static void BestFit()
        {
            Cheqs = new List<Cheq>();
            Bins = new List<Bin>();


            Is_TotalBedehi_more_than_sum_Cheq_values = ImportData.Prepare_Before_Run(Bins, Cheqs);

            List<Bin> BinsPrority1 = Bins.Where(z => z.BargeType == 0 && z.Title != "Holding").ToList();
            List<Bin> BinsPrority2 = Bins.Where(z => z.BargeType != 0 && z.Title != "Holding").ToList();
            Bin holding = Bins.Where(z => z.Title == "Holding").SingleOrDefault();



            if (Is_TotalBedehi_more_than_sum_Cheq_values)
            {
                // order Bedehi by nearest deadlines first 
                Bins = BinsPrority1.OrderBy(x => x.Deadline).ThenBy(r => r.CurrentBedehi).ToList();
            }
            else
            {
                Bins = BinsPrority1.OrderBy(r => r.CurrentBedehi).ToList();
                //Bins = Bins.OrderByDescending(r => r.Current).ToList();
            }

            BinsPrority2.ForEach(rec => {
                Bins.Add(rec);
            });
            Bins.Add(holding);

            //if( Cheqs.Count <= 20)
            //    Run_IdealFit();
            //else
            Run_BestFit();


            

           

            //------------------------------print results:

            foreach (var bin in Bins)
            {
                Console.WriteLine( bin.CodeMarkaz +":"+bin.Title +"  ----------------------------- "+ "Base = "+ bin.Base + "  current = " + bin.CurrentBedehi);
                foreach (var i in bin.Container)
                    Console.WriteLine("         ||| ID Cheq: "+ i.ID +" Left: "+i.ValueCurrent + " form : "+ i.ValueBase);
            }

            Write.Log_2_Excel(Bins);

        }



        private static void Run_BestFit()
        {
            
            List<Cheq> sortedCheqs = Cheqs.OrderBy(x => x.ValueBase).ToList();

            foreach (var cheq in sortedCheqs)
            {
                bool assignFlag = false;
                //foreach (var bin in Bins.Where(p => p.Title != "Holding").OrderBy(x => x.CurrentBedehi))
                foreach (var bin in Bins.Where(p => p.Title != "Holding"))
                    if (Assign(cheq, bin) == true)
                    {
                        assignFlag = true;
                        break;
                    }


                if (assignFlag == false)
                    Assign_2_Holding(cheq);
            }
        }

        private static bool Assign(Cheq chq, Bin bin)
        {
            if (chq.ValueCurrent <= bin.CurrentBedehi)
            {
                bin.CurrentBedehi -= chq.ValueCurrent;
                chq.ValueCurrent = 0;
                bin.Container.Add(chq);

                return true;
            }
            return false;
        }


        private static bool Assign_2_Holding(Cheq cheq)
        {
            try
            {
                Bin binHolding = Bins.Where(p => p.Title == "Holding").Single();

                binHolding.Container.Add(cheq);

                binHolding.Base += cheq.ValueCurrent;
                binHolding.CurrentBedehi += cheq.ValueCurrent;


                //List<Bin> listBins =  Is_TotalBedehi_more_than_sum_Cheq_values == true ? 
                //    Bins.Where(p => p.Title != "Holding").OrderBy(x => x.Deadline).ThenByDescending(n => n.CurrentBedehi).ToList() :                 
                //    Bins.Where(p => p.Title != "Holding").OrderByDescending(n => n.CurrentBedehi).ToList();

                //foreach (var bin in listBins)
                foreach (var bin in Bins)
                {

                    if (bin.CurrentBedehi == 0)
                        continue;
                    else if (bin.CurrentBedehi <= cheq.ValueCurrent)
                    {
                        bin.offerByHolding.Add(cheq.ID, bin.CurrentBedehi);
                        cheq.ValueCurrent -= bin.CurrentBedehi;
                        binHolding.CurrentBedehi -= bin.CurrentBedehi;
                        bin.CurrentBedehi = 0;                        
                    }
                }

                

                return true;
            }
            catch (Exception ex)
            {
                return false;                
            }

        }


        







    }
}
