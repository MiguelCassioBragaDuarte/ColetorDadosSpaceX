using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColetorDadosSpaceX.Models
{
    public class Stats
    {
        public int TotalLaunches { get; set; }
        public int SuccessfulLaunches { get; set; }
        public int FailedLaunches { get; set; }
        public double SuccessRate { get; set; }
    }
}
