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
            MyTestClass tc = new MyTestClass();
            tc.TestGraph();
            tc.TestCluster();
            int k = 30;
            while (true) {
                var a = new Experiment(k, "../../dsjc250.5.txt", 100, "Expiriment 1");
                a.Run();
                k--;
            }
        }
    }
}
