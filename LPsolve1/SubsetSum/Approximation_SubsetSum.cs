using LPsolve1.Excel;
using LPsolve1.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPsolve1.SubsetSum
{
    class Approximation_SubsetSum
    {

        public static List<Cheq> Cheqs = null;
        public static List<Bin> Bins = null;

        public static List<Cheq> CheqsBK = null;
        


        public static void Approximate()
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

                foreach( Cheq cheq in Holding.Container.OrderBy(x => x.ValueCurrent))
                {
                    foreach(Bin bin in Bins.Where(t => t.Title != "Holding").OrderBy(y => y.CurrentBedehi))
                    {
                        if(bin.CurrentBedehi != 0)
                        {
                            if(cheq.ValueCurrent <= bin.CurrentBedehi)
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
                foreach(Bin bin in Bins)
                {
                    Console.WriteLine("================== " + bin.Title+" >>> CurrentBedehi = " + bin.CurrentBedehi + " from Base = "+bin.Base);
                    foreach(Cheq cheq in bin.Container.ToList())
                    {
                        Console.WriteLine("         ||| ID Cheq: " + cheq.ID + " Left: " + cheq.ValueCurrent + " form Base: " + cheq.ValueBase);
                    }
                    Console.WriteLine("\n\n");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static subsetModel Calculate_Subset(Bin bin, List<Cheq> cheqs)
        {
            //List<subsetModel> Union = null;
            //bool flag_sumExceeded_Bin_Size = false;
            bool flag_anything_Optimized = true;
            List<subsetModel> flyingSubset = new List<subsetModel>() { new subsetModel { CheqIDs = new HashSet<string>(), sum = 0 } };
            //List<KeyValuePair<string, subsetModel>> dic = new List<KeyValuePair<string, subsetModel>>();

            subsetModel result = null, minPert = null;

            foreach (Cheq cheq in cheqs.OrderBy(x => x.ValueBase))
            {
                //if (flag_sumExceeded_Bin_Size == true)
                //    break;
                List<subsetModel> Union = new List<subsetModel>();

                //if(result != null)
                //{
                //    foreach(var i in Temp.Where(z =>z.sum != 0))
                //    {
                //        if (result.CheqIDs.Except(i.CheqIDs).ToList().Count != 0)
                //        {
                //            OmittedCheqIDs.AddRange(result.CheqIDs);
                //            Temp.Remove(i);
                //        }
                //    }
                //}

                if (flag_anything_Optimized == false) // nothing optimized by the previous cheq
                {
                    return minPert;
                }

                foreach (subsetModel i in flyingSubset)
                {
                    Union.Add(i);
                    Union.Add(new subsetModel { CheqIDs = merge(i.CheqIDs, cheq.ID), sum = (cheq.ValueBase + i.sum) });
                }

                flyingSubset.Clear();
                flyingSubset = new List<subsetModel>();
                Union.ForEach(x => flyingSubset.Add(x));



                result = null;
                flag_anything_Optimized = false;
                ulong min = ulong.MaxValue;
                foreach (subsetModel z in Union.OrderBy(x => x.sum).ToList())
                {
                    if (z.sum == 0) continue;

                    if (z.sum > bin.Base)
                    {
                        //flag_sumExceeded_Bin_Size = true;
                        break;
                    }
                    //else if (z.sum <= bin.Base && z.sum > epsilon * z.sum)
                    else if (z.sum <= bin.Base)
                    {
                        if (bin.Base - z.sum < min)
                        {
                            min = bin.Base - z.sum;
                            result = z;
                        }
                    }
                }//

                if (result != null)
                {
                    //dic.Add(cheq.ID, result);
                    //dic.Add(new KeyValuePair<string, subsetModel>(bin.Title, result));
                    flag_anything_Optimized = true;
                    minPert = result;
                }
            }//

            return minPert;
        }

        private static HashSet<string> merge(HashSet<string> cheqIDs, string iD)
        {
            HashSet<string> clone = new HashSet<string>();
            foreach(var i in cheqIDs)
                 clone.Add(i);

            return clone.Add(iD) == true ? clone : clone;
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
    }
}
