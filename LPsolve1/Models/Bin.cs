using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPsolve1.Models
{
    public class Bin
    {
        public string  Title { get; set; }
        public ulong Base { get; set; }
        public ulong CurrentBedehi { get; set; }
        public List<Cheq> Container { get; set; }
    }
}
