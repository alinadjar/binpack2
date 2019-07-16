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

            if(Is_TotalBedehi_more_than_sum_Cheq_values)
            {
                // order Bedehi by nearest deadlines first 
                Bins = Bins.OrderBy(x => x.Deadline).ThenBy(r => r.CurrentBedehi).ToList();
            }
            else
            {
                Bins = Bins.OrderBy(r => r.CurrentBedehi).ToList();
                //Bins = Bins.OrderByDescending(r => r.Current).ToList();
            }

            //if( Cheqs.Count <= 20)
            //    Run_IdealFit();
            //else
            Run_BestFit();



            //------------------------------print results:

            foreach (var bin in Bins)
            {
                Console.WriteLine( bin.Title +"  ----------------------------- "+ "Base = "+ bin.Base + "  current = " + bin.CurrentBedehi);
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
                foreach (var bin in Bins.Where(p => p.Title != "Holding").OrderBy(x => x.CurrentBedehi))
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


                List<Bin> listBins =  Is_TotalBedehi_more_than_sum_Cheq_values == true ? 
                    Bins.Where(p => p.Title != "Holding").OrderBy(x => x.Deadline).ThenByDescending(n => n.CurrentBedehi).ToList() :                 
                    Bins.Where(p => p.Title != "Holding").OrderByDescending(n => n.CurrentBedehi).ToList();

                foreach (var bin in listBins)
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


        




        //-------------------------------------------------------
        private static void Run_IdealFit()
        {
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i != Cheqs.Count; i++)
                sb.Append("0");
            Permute.Permutation01(sb.ToString().ToArray(), 0, Cheqs.Count);

            List<string> permutation = Permute.perms;
            Dictionary<string, long> dic = new Dictionary<string, long>();

            foreach(string permStr in permutation)
            {
                if (Convert.ToUInt64(permStr) == 0)
                    continue;
                // e,g: s = 01001100001
                char[] coefs = permStr.ToArray();
                long sum = 0;
                for(int i = 0; i != coefs.Length; i++)
                {
                    if(coefs[i] == '1')
                    {
                        sum += Cheqs[i].ValueCurrent;
                    }
                    
                }// end inner-for

                dic.Add(permStr, sum);
            }// end outer-for


            long sizeMaxBin = Bins.Where(m => m.Title != "Holding").Max(l => l.CurrentBedehi);
            //dic.Where(e => e.Value <= sizeMaxBin).OrderByDescending(a => a.Value).ToList()

        }


    }
}
