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


        public static void Approximate()
        {

            Prepare_Before_Run();

            Dictionary<string, subsetModel> finalresult = new Dictionary<string, subsetModel>();

            for (double epsilon = 0.9; epsilon >= 0.1; epsilon-=0.1)
            {
                foreach(Bin bin in Bins.Where(x => x.Title != "Holding"))
                {
                    List<KeyValuePair<string, subsetModel>> output = Calculate_Subset(epsilon, bin, Cheqs);
                    KeyValuePair<string, subsetModel> r = output.OrderByDescending(x => x.Value.sum).FirstOrDefault();
                    if (r.Key != null)
                        finalresult.Add(r.Key, r.Value);
                }


                // take the remaining Cheqs into a bin named 'Holding', so as to be assigned.
                HashSet<string> assignedCheqIDs = new HashSet<string>();
                foreach (HashSet<string> index in finalresult.Values.Select(i => i.CheqIDs))
                    assignedCheqIDs.Concat(index);

                List<Cheq> remaining = (from ch in Cheqs where ! assignedCheqIDs.Contains(ch.ID) select ch).ToList();
                // Assign them to 
            }
        }

        private static List<KeyValuePair<string, subsetModel>> Calculate_Subset(double epsilon, Bin bin, List<Cheq> cheqs)
        {
            //List<subsetModel> Union = null;
            bool flag_sumExceeded_Bin_Size = false;
            List<subsetModel> Temp = new List<subsetModel>() { new subsetModel { CheqIDs = new HashSet<string>(), sum = 0 } };
            List<KeyValuePair<string, subsetModel>> dic = new List<KeyValuePair<string, subsetModel>>();
            List<string> OmittedCheqIDs = new List<string>();

            subsetModel result = null;

            foreach (Cheq cheq in cheqs)
            {
                if (flag_sumExceeded_Bin_Size == true)
                    break;
                List<subsetModel> Union = new List<subsetModel>() { new subsetModel { CheqIDs = new HashSet<string>(), sum = 0 } };

                if(result != null)
                {
                    foreach(var i in Temp.Where(z =>z.sum != 0))
                    {
                        if (result.CheqIDs.Except(i.CheqIDs).ToList().Count != 0)
                        {
                            OmittedCheqIDs.AddRange(result.CheqIDs);
                            Temp.Remove(i);
                        }
                    }
                }

                foreach (subsetModel i in Temp)
                {
                    Union.Add(new subsetModel { CheqIDs = merge(i.CheqIDs, cheq.ID) , sum = (cheq.Base+i.sum) });
                }

                Temp.Clear();
                Temp = new List<subsetModel>();
                Union.ForEach(x => Temp.Add(x));



                result = null;
                foreach(subsetModel z in Union.OrderBy(x => x.sum).ToList())
                {
                    ulong min = ulong.MaxValue;
                    if (z.sum > bin.Base)
                    {
                        flag_sumExceeded_Bin_Size = true;
                        break;
                    }
                    else if (z.sum <= bin.Base && z.sum > epsilon * z.sum)
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
                    dic.Add(new KeyValuePair<string, subsetModel>(bin.Title, result));
                }
                    
            }//

            return dic;
        }

        private static HashSet<string> merge(HashSet<string> cheqIDs, string iD)
        {
            return cheqIDs.Add(iD) == true ? cheqIDs: cheqIDs;
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
                        Current = Bedehi,
                        Container = new List<Cheq>()
                    });
                }

                if (ChqNumber != null)
                {
                    Cheqs.Add(new Cheq { ID = ChqNumber, Value = ChqValue, Base = ChqValue });
                }


            }
        }
    }
}
