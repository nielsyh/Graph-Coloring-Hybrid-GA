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

            for (var i = 0; i < 5; i++)
            {
                var k = 21;

                while (true)
                {
                    var e = new Experiment(k, "../../le450_15c.txt", 100, "CALC Fitness Correlation Coefficient", "vs.5.txt" + i + "_k" + k + "_");
                    if (k <= 14) break;
                    e.Run();
                    k--;
                }

            }
        }
    }
}
