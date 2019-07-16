using LPsolve1.Commons;
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
        public static bool Is_TotalBedehi_more_than_sum_Cheq_values = false;

        public static List<Cheq> CheqsBK = null;


        public static int COUNTER = 100;


        public static void Approximate_Randomize()
        {

            Cheqs = new List<Cheq>();
            Bins = new List<Bin>();

            try
            {
                Is_TotalBedehi_more_than_sum_Cheq_values = ImportData.Prepare_Before_Run(Bins, Cheqs);
                if (Is_TotalBedehi_more_than_sum_Cheq_values)
                {
                    // order Bedehi by nearest deadlines first 
                    Bins = Bins.OrderBy(x => x.Deadline).ThenBy(r => r.CurrentBedehi).ToList();
                }
                else
                {
                    Bins = Bins.OrderBy(r => r.CurrentBedehi).ToList();
                    //Bins = Bins.OrderByDescending(r => r.Current).ToList();
                }

                COUNTER = Cheqs.Count; // / 1;

                // Let's take a backup of Cheqs
                CheqsBK = new List<Cheq>();
                Cheqs.ForEach(x => CheqsBK.Add(x));



                Dictionary<string, subsetModel> finalresult = new Dictionary<string, subsetModel>();


                foreach (Bin bin in Bins.Where(x => x.Title != "Holding"))
                {
                    subsetModel output = Calculate_Subset(bin, Cheqs);
                    int tedad = output?.CheqIDs?.Count ?? 0;
                    if (tedad != 0)
                    {
                        bin.CurrentBedehi -= output.sum;
                        finalresult.Add(bin.Title, output);

                        List<string> selectedCheqs = output.CheqIDs.ToList();
                        selectedCheqs.ForEach(v =>
                        {
                            Cheq q = Cheqs.Where(y => y.ID == v).SingleOrDefault();
                            q.ValueCurrent = 0;
                            bin.Container.Add(q);
                        });

                        selectedCheqs.ForEach(v => Cheqs.Remove(Cheqs.Where(y => y.ID == v).SingleOrDefault()));
                    }
                }



                // take the remaining Cheqs into a bin named 'Holding', so as to be assigned.

                List<Cheq> remaining = Cheqs.ToList();


                //List<string> assignedCheqIDs = new List<string>();
                //foreach (HashSet<string> index in finalresult.Values.Select(i => i.CheqIDs))
                //    assignedCheqIDs.AddRange(index.ToList());

                //List<Cheq> remaining = (from ch in Cheqs where !assignedCheqIDs.Contains(ch.ID) select ch).ToList();

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
                    List<Bin> listBins = Is_TotalBedehi_more_than_sum_Cheq_values == true ?
                        Bins.Where(t => t.Title != "Holding").OrderBy(x => x.Deadline).ThenBy(y => y.CurrentBedehi).ToList() :
                        Bins.Where(t => t.Title != "Holding").OrderBy(y => y.CurrentBedehi).ToList();
                    foreach (Bin bin in listBins)
                    {
                        if (bin.CurrentBedehi != 0)
                        {
                            if (cheq.ValueCurrent <= bin.CurrentBedehi)
                            {
                                bin.offerByHolding.Add(cheq.ID, cheq.ValueCurrent);
                                bin.CurrentBedehi -= cheq.ValueCurrent;
                                Holding.CurrentBedehi -= cheq.ValueCurrent;
                                cheq.ValueCurrent = 0;
                                break;
                            }
                            else
                            {
                                bin.offerByHolding.Add(cheq.ID, bin.CurrentBedehi);
                                cheq.ValueCurrent -= bin.CurrentBedehi;
                                Holding.CurrentBedehi -= bin.CurrentBedehi;
                                bin.CurrentBedehi = 0;
                            }
                        }
                    }// loop Bins
                }// loop Cheqs






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
            filteredList = Cheqs.filterCumSum(bin.Base);


            for (int index = 0; index != COUNTER; index++)
            {
                //List<Cheq> filtered = cheqs.OrderBy(c => c.ValueBase).Where(x => x.ValueBase <= bin.Base).ToList();






                List<int> rndList = new List<int>();
                long sum = 0;

                filteredList.ForEach(x =>
                {
                    int rnd = rand.Next(0, 2);
                    rndList.Add(rnd);
                    sum += x.ValueBase * rnd;
                });


                if (sum > bin.Base)
                    continue;
                else
                {
                    if ((bin.Base - sum) < minPert)
                    {
                        minPert = bin.Base - sum;
                        minRndList = rndList;
                    }
                }

            }


            subsetModel result = new subsetModel() { sum = 0, CheqIDs = new HashSet<string>() };


            if (minPert == long.MaxValue)
            {
                result = Quick_FirstFit(bin, filteredList);
            }
            else
            {

                for (int j = 0; j != minRndList.Count; j++)
                {
                    if (minRndList[j] == 1)
                    {
                        Cheq ch = filteredList[j];
                        //ch.ValueCurrent = 0;
                        //bin.Container.Add(ch);
                        //bin.CurrentBedehi -= ch.ValueBase;


                        result.sum += ch.ValueBase;
                        result.CheqIDs.Add(ch.ID);
                    }

                }
            }

            return result;

        }

        private static subsetModel Quick_FirstFit(Bin bin, List<Cheq> filteredList)
        {
            Console.WriteLine("############################################################################");
            subsetModel result = new subsetModel() { sum = 0, CheqIDs = new HashSet<string>() };



            //if (filteredList.Count > 10)
            //{
            foreach (Cheq cheq in filteredList.OrderByDescending(x => x.ValueBase))
            {
                if (result.sum + cheq.ValueBase < bin.Base)
                {
                    result.sum += cheq.ValueBase;
                    result.CheqIDs.Add(cheq.ID);

                    //bin.Container.Add(cheq);
                    //bin.CurrentBedehi -= cheq.ValueBase;
                    //cheq.ValueCurrent = 0;
                }
            }
            //}
            //else
            //{
            //    StringBuilder sb = new StringBuilder();
            //    filteredList.ForEach(x => sb.Append("0"));
            //    Randomized_SubsetSum.perms = new List<string>();
            //    Permutation01(sb.ToString().ToCharArray(), 0, sb.ToString().Length);

            //    foreach(string str in perms)
            //    {

            //    }
            //}


            return result;
        }



        //public static void General_SubsetSum(int[] set, int n, int sum, List<Cheq> r)
        //{
        //    // Returns true if there is a subset of set[] with sum 
        //    // equal to given sum 

        //    // Base Cases 
        //    if (sum == 0)
        //    {
        //        return true;
        //    }

        //    if (n == 0 && sum != 0)
        //        return false;

        //    // If last element is greater than sum,  
        //    // then ignore it 
        //    if (set[n - 1] > sum)
        //        return General_SubsetSum(set, n - 1, sum);

        //    /* else, check if sum can be obtained  
        //    by any of the following 
        //    (a) including the last element 
        //    (b) excluding the last element */
        //    return General_SubsetSum(set, n - 1, sum) || General_SubsetSum(set, n - 1, sum - set[n - 1]);
        //}


        public static List<string> perms = new List<string>();

        public static void Permutation01(char[] s, int i, int n)
        {
            if (i == n)
            {
                Console.WriteLine(s);
                perms.Add(new string(s));
            }
            else
            {

                Permutation01(s, i + 1, n);
                swap01(s, i);
                Permutation01(s, i + 1, n);
                swap01(s, i);

            }
        }

        private static void swap01(char[] s, int v1)
        {
            s[v1] = s[v1] == '0' ? '1' : '0';
        }


    }
}
