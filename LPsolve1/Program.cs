using LPsolve1.BinPacking.BestFit;
using LPsolve1.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LPsolve1.SubsetSum;
using LPsolve1.Randomized;

namespace LPsolve1
{
    class Program
    {
        static void Main(string[] args)
        {
            // The code provided will print ‘Hello World’ to the console.
            // Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.
            Console.WriteLine("Starting ......");

            //            When this is run, the following is shown in the debug window:

            //            Objective value: 6315.625
            //              x: 21.875
            //              y: 53.125

            //              And a file model.lp is created with the following contents:

            //            /* Objective function */
            //            max: +143 x + 60 y;

            //                /* Constraints */
            //                +120 x + 210 y <= 15000;
            //                +110 x + 30 y <= 4000;
            //                +x + y <= 75;

            //FormulateSample.Main2();
            //-------------------------------------------


            //BinPack.BestFit();
            //-------------------------------------------


            //int[] set = { 3, 34, 4, 12, 5, 2 };
            //int sum = 9;
            //int n = set.Length;

            //if (Subset_Sum.General(set, n, sum) == true)
            //    Console.WriteLine("Found a subset with given sum");
            //else
            //    Console.WriteLine("No subset with given sum");
            //-------------------------------------------

            //long sample = long.MaxValue;

            //Approximation_SubsetSum.Approximate();



            Randomized_SubsetSum.Approximate_Randomize();

            //Permute.Permutation01("00000000000000000000".ToArray(), 0, 20);
            //Read.readColumns();



            //Write.Write_2_Excel();





            Console.ReadKey();            
        }
    }
}
