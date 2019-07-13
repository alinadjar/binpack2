using LPsolve1.Models;
using LPsolve1.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPsolve1.BinPacking.BestFit
{
    public class BinPack
    {

        public static List<Cheq> Cheqs = null;
        public static List<Bin> Bins = null;        


        public static void BestFit()
        {

            Prepare_Before_Run();


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

        }



        private static void Run_BestFit()
        {
            Bins = Bins.OrderBy(r => r.CurrentBedehi).ToList();
            //Bins = Bins.OrderByDescending(r => r.Current).ToList();
            List<Cheq> sortedCheqs = Cheqs.OrderByDescending(x => x.ValueBase).ToList();

            foreach (var cheq in sortedCheqs)
            {
                bool assignFlag = false;
                foreach (var bin in Bins.OrderBy(x => x.CurrentBedehi).Where(p => p.Title != "Holding"))
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

                binHolding.Base += cheq.ValueCurrent;
                binHolding.CurrentBedehi += cheq.ValueCurrent;

                foreach (var bin in Bins.Where(p => p.Title != "Holding").OrderByDescending(n => n.CurrentBedehi))
                {

                    if (bin.CurrentBedehi == 0)
                        continue;
                    else if (bin.CurrentBedehi <= cheq.ValueCurrent)
                    {
                        cheq.ValueCurrent -= bin.CurrentBedehi;
                        binHolding.CurrentBedehi -= bin.CurrentBedehi;
                        bin.CurrentBedehi = 0;
                    }
                }

                binHolding.Container.Add(cheq);

                return true;
            }
            catch (Exception ex)
            {
                return false;                
            }

        }


        /// <summary>
        /// Read Excel and Store values in in Cheqs and Bins
        /// </summary>
        private static void Prepare_Before_Run()
        {
            Cheqs = new List<Cheq>();
            Bins = new List<Bin>();



            DataSet ds = Read.LoadAllSheets_2_Dataset();
            foreach (DataRow row in ds.Tables["Table1"].Rows)
            {

                string Markaz = String.IsNullOrEmpty(row.ItemArray[0].ToString()) ? null : row.ItemArray[0].ToString();
                ulong Bedehi = String.IsNullOrEmpty(row.ItemArray[1].ToString()) ? 0 : Convert.ToUInt64(row.ItemArray[1]);
                string ChqNumber = String.IsNullOrEmpty(row.ItemArray[2].ToString()) ? null : row.ItemArray[2].ToString();
                ulong ChqValue = String.IsNullOrEmpty(row.ItemArray[3].ToString()) ? 0 : Convert.ToUInt64(row.ItemArray[3]);



                if (Markaz != null)
                {
                    Bins.Add(new Bin
                    {
                        Title = Markaz,
                        Base = Bedehi,
                        CurrentBedehi = Bedehi,
                        Container = new List<Cheq>()
                    });
                }

                if (ChqNumber != null)
                {
                    Cheqs.Add(new Cheq { ID = ChqNumber, ValueCurrent = ChqValue, ValueBase = ChqValue });
                }


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
            Dictionary<string, ulong> dic = new Dictionary<string, ulong>();

            foreach(string permStr in permutation)
            {
                if (Convert.ToUInt64(permStr) == 0)
                    continue;
                // e,g: s = 01001100001
                char[] coefs = permStr.ToArray();
                ulong sum = 0;
                for(int i = 0; i != coefs.Length; i++)
                {
                    if(coefs[i] == '1')
                    {
                        sum += Cheqs[i].ValueCurrent;
                    }
                    
                }// end inner-for

                dic.Add(permStr, sum);
            }// end outer-for


            ulong sizeMaxBin = Bins.Where(m => m.Title != "Holding").Max(l => l.CurrentBedehi);
            //dic.Where(e => e.Value <= sizeMaxBin).OrderByDescending(a => a.Value).ToList()

        }


    }
}
