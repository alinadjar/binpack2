using LPsolve1.Excel;
using LPsolve1.Models;
using LPsolve1.SubsetSum;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPsolve1.Randomized
{
    public class Randomized_SubsetSum
    {

        public static List<Cheq> Cheqs = null;
        public static List<Bin> Bins = null;

        public static List<Cheq> CheqsBK = null;


        public static int COUNTER = 100;


        public static void Approximate_Randomize()
        {
            try
            {
                Prepare_Before_Run();

                CheqsBK = new List<Cheq>();
                Cheqs.ForEach(x => CheqsBK.Add(x));

                Dictionary<string, subsetModel> finalresult = new Dictionary<string, subsetModel>();

                //for (double epsilon = 0.9; epsilon >= 0.1; epsilon-=0.1)
                //{
                foreach (Bin bin in Bins.Where(x => x.Title != "Holding").OrderBy(x => x.Base))
                {
                    subsetModel output = Calculate_Subset(bin, Cheqs);
                    int tedad = output?.CheqIDs?.Count ?? 0;
                    if (tedad != 0)
                    {
                        bin.CurrentBedehi -= output.sum;
                        finalresult.Add(bin.Title, output);

                        List<string> selectedCheqs = output.CheqIDs.ToList();
                        selectedCheqs.ForEach(v => {
                            Cheq q = Cheqs.Where(y => y.ID == v).SingleOrDefault();
                            q.ValueCurrent = 0;
                            bin.Container.Add(q);
                        });

                        selectedCheqs.ForEach(v => Cheqs.Remove(Cheqs.Where(y => y.ID == v).SingleOrDefault()));
                    }
                }



                // take the remaining Cheqs into a bin named 'Holding', so as to be assigned.
                List<string> assignedCheqIDs = new List<string>();
                foreach (HashSet<string> index in finalresult.Values.Select(i => i.CheqIDs))
                    assignedCheqIDs.AddRange(index.ToList());

                //List<Cheq> remaining = (from ch in Cheqs where !assignedCheqIDs.Contains(ch.ID) select ch).ToList();
                List<Cheq> remaining = Cheqs.ToList();
                //remaining.AddRange((from ch in CheqsBK where ch.Value != 0 select ch).ToList());

                Bin Holding = Bins.Where(m => m.Title == "Holding").Single();
                foreach (Cheq rCHQ in remaining)
                {
                    Holding.Container.Add(rCHQ);
                    Holding.CurrentBedehi += rCHQ.ValueCurrent;
                    Holding.Base += rCHQ.ValueCurrent;
                }

                foreach (Cheq cheq in Holding.Container.OrderBy(x => x.ValueCurrent))
                {
                    foreach (Bin bin in Bins.Where(t => t.Title != "Holding").OrderBy(y => y.CurrentBedehi))
                    {
                        if (bin.CurrentBedehi != 0)
                        {
                            if (cheq.ValueCurrent <= bin.CurrentBedehi)
                            {
                                bin.CurrentBedehi -= cheq.ValueCurrent;
                                Holding.CurrentBedehi -= cheq.ValueCurrent;
                                cheq.ValueCurrent = 0;
                                break;
                            }
                            else
                            {
                                cheq.ValueCurrent -= bin.CurrentBedehi;
                                Holding.CurrentBedehi -= bin.CurrentBedehi;
                                bin.CurrentBedehi = 0;
                            }
                        }
                    }// loop Bins
                }// loop Cheqs





                //}


                //-------------------------- Print
                foreach (Bin bin in Bins)
                {
                    Console.WriteLine("================== " + bin.Title + " >>> CurrentBedehi = " + bin.CurrentBedehi + " from Base = " + bin.Base);
                    foreach (Cheq cheq in bin.Container.ToList())
                    {
                        Console.WriteLine("         ||| ID Cheq: " + cheq.ID + " Left: " + cheq.ValueCurrent + " form Base: " + cheq.ValueBase);
                    }
                    Console.WriteLine("\n\n");
                }


                Write.Log_2_Excel(Bins);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private static subsetModel Calculate_Subset(Bin bin, List<Cheq> cheqs)
        {
            
            
            Random rand = new Random();



            long minPert = long.MaxValue;
            List<int> minRndList = null;
            List<Cheq> filteredList = null;
            for (int index = 0; index != COUNTER; index++)
            {
                //List<Cheq> filtered = cheqs.OrderBy(c => c.ValueBase).Where(x => x.ValueBase <= bin.Base).ToList();

                filteredList = Cheqs.filterCumSum(bin.Base);




                List<int> rndList = new List<int>();
                long sum = 0;

                filteredList.ForEach(x => {
                    int rnd = rand.Next(0, 2);
                    rndList.Add(rnd);
                    sum += x.ValueBase * rnd;
                });


                if (sum > bin.Base)
                    continue;
                else
                {
                    if (sum < minPert)
                    {
                        minPert = sum;
                        minRndList = rndList;
                    }
                }

            }


            subsetModel result = new subsetModel() {  sum = 0, CheqIDs = new HashSet<string>()};


            if (minPert == long.MaxValue)
            {
                result = Quick_FirstFit(bin, filteredList);
            }
            else
            {

                for(int j = 0; j != minRndList.Count; j++)
                {
                    if(minRndList[j] == 1)
                    {                        
                        Cheq ch = filteredList[j];
                        ch.ValueCurrent = 0;
                        bin.Container.Add(ch);
                        bin.CurrentBedehi -= ch.ValueBase;
                        

                        result.sum += ch.ValueBase;
                        result.CheqIDs.Add(ch.ID);
                    }

                }
            }

            return result;
             
        }

        private static subsetModel Quick_FirstFit(Bin bin, List<Cheq> filteredList)
        {
            subsetModel result = new subsetModel() { sum = 0, CheqIDs = new HashSet<string>() };

            

            foreach(Cheq cheq in filteredList.OrderBy(x => x.ValueBase))
            {
                if(result.sum + cheq.ValueBase < bin.Base)
                {
                    result.sum += cheq.ValueBase;
                    result.CheqIDs.Add(cheq.ID);

                    bin.Container.Add(cheq);
                    bin.CurrentBedehi -= cheq.ValueBase;
                    cheq.ValueCurrent = 0;
                }
            }

            return result;
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
                long Bedehi = String.IsNullOrEmpty(row.ItemArray[1].ToString()) ? 0 : Convert.ToInt64(row.ItemArray[1]);
                string ChqNumber = String.IsNullOrEmpty(row.ItemArray[2].ToString()) ? null : row.ItemArray[2].ToString();
                long ChqValue = String.IsNullOrEmpty(row.ItemArray[3].ToString()) ? 0 : Convert.ToInt64(row.ItemArray[3]);



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



                COUNTER = Cheqs.Count / 1;
            }
        }
    }
}
