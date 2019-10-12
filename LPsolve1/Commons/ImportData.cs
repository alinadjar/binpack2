using LPsolve1.Excel;
using LPsolve1.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPsolve1.Commons
{
    public class ImportData
    {

        /// <summary>
        /// Read Excel and Store values in the Cheqs and Bins received
        /// </summary>
        public static bool Prepare_Before_Run(List<Bin> Bins, List<Cheq> Cheqs)
        {




            ulong sumBedehi = 0, sumCheqValue = 0;

            DataSet ds = Read.LoadAllSheets_2_Dataset();
            foreach (DataRow row in ds.Tables["Table1"].Rows)
            {

                string CodeMarkaz = String.IsNullOrEmpty(row.ItemArray[0].ToString()) ? null : row.ItemArray[0].ToString();
                string Markaz = String.IsNullOrEmpty(row.ItemArray[1].ToString()) ? null : row.ItemArray[1].ToString();
                long Bedehi = String.IsNullOrEmpty(row.ItemArray[2].ToString()) ? 0 : Convert.ToInt64(row.ItemArray[2]);
                int deadline = String.IsNullOrEmpty(row.ItemArray[3].ToString()) ? 0 : DateHelper.Convert_StndDate_2_Int(row.ItemArray[3].ToString());
                int BargeType = String.IsNullOrEmpty(row.ItemArray[4].ToString()) ? 1 : Convert.ToInt32(row.ItemArray[4]);
                string ChqNumber = String.IsNullOrEmpty(row.ItemArray[5].ToString()) ? null : row.ItemArray[5].ToString();
                long ChqValue = String.IsNullOrEmpty(row.ItemArray[6].ToString()) ? 0 : Convert.ToInt64(row.ItemArray[6]);


                sumBedehi += (ulong)Bedehi;
                sumCheqValue += (ulong)ChqValue;


                if (Markaz != null)
                {
                    Bins.Add(new Bin
                    {
                        Title = Markaz,
                        CodeMarkaz = CodeMarkaz,
                        Deadline = deadline,
                        BargeType = BargeType,
                        Base = Bedehi,
                        CurrentBedehi = Bedehi,
                        Container = new List<Cheq>(),
                        offerByHolding = new Dictionary<string, long>(),
                        cheqDetails = new Dictionary<string, string>()
                    });
                }

                if (ChqNumber != null)
                {
                    Cheqs.Add(new Cheq { ID = ChqNumber, ValueCurrent = ChqValue, ValueBase = ChqValue });
                }


            }//end forach

            return sumBedehi > sumCheqValue;
        }
    }
}
