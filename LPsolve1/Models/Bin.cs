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
        public long Base { get; set; }
        public long CurrentBedehi { get; set; }
        public List<Cheq> Container { get; set; }
    }
}
