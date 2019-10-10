using LPsolve1.BinPacking.BestFit;
using LPsolve1.GroupPacking;
using LPsolve1.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LPsolve1.Randomized;

namespace LPsolve1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting ......");




            //BinPack.BestFit();
            GroupPack.GroupFit();
            //-------------------------------------------




            //Randomized_SubsetSum.Approximate_Randomize();
            //-------------------------------------------



            // DO NOT USE THIS,
            // aimed to calculate Ideal solution on small-scaled input.
            //Approximation_SubsetSum.Approximate();
            //-------------------------------------------




            Console.ReadKey();            
        }
    }
}
