using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC_Practicum_2
{
    class Program
    {
        static void Main(string[] args)
        {
            var tc = new MyTestClass();
            tc.TestGraph();
            tc.TestCluster();
            int k = 5;
            while (true)
            {
                var Expiriment = new Experiment(k, "../../dsjc250.5.txt", 80, "Expiriment 1");
                Expiriment.Run();

                k--;
            }
        }
    }
}
