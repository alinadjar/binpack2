using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPsolve1
{
    public class Permute
    {

        public static List<string> perms = new List<string>();
        public static void Permutation(char[] s, int i, int n)
        {
            if (i == n)
                Console.Write(s);
            else
            {
                for(int j=i; j != n; j++)
                {
                    swap(s, i, j);
                    Permutation(s, i+1, n);
                    swap(s, i, j);
                }
            }
        }

        private static void swap(char[] s, int v1, int v2)
        {
            char temp = s[v2];
            s[v2] = s[v1];
            s[v1] = temp;
        }



        //--------------------------------------------------
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
