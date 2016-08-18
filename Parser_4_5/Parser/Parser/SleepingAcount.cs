using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    public class SleepingAcount
    {  
        public SleepingAcount(string p1, double p2)
        {
             
            this.Login = p1;
            this.TimeTo = p2;
        }
        public double TimeTo { get; set; }
        public string Login { get; set; }

    }
}
