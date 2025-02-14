﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPsolve1.Models
{
    public class Bin
    {
        public string Title { get; set; }
        public string CodeMarkaz { get; set; }
        public int Deadline { get; set; }
        public long Base { get; set; }
        public long CurrentBedehi { get; set; }
        public List<Cheq> Container { get; set; }

        // each entry to this dictionary is a  <CheqID, Mablaq>, which refers to the amount deficit, accomodated by holding
        public Dictionary<string, long> offerByHolding { get; set; }
        public Dictionary<string, long> cheqDetails { get; set; }
        public Dictionary<string, long> CheqDetails { get { return this.cheqDetails;  } }

        public int BargeType { get; set; }

        internal void AddAmountReceivedByCheq(string iD, long valueCurrent)
        {
            if (cheqDetails.ContainsKey(iD))
                cheqDetails[iD] += valueCurrent;       
            else
                cheqDetails.Add(iD, valueCurrent);
        }



    }
}
