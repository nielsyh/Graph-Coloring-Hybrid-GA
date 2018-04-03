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
                var k = 18;

                while (true)
                {
                    if (k == 14) break;

                    var e = new Experiment(k, "../../le450_15c.txt", 120, "CALC Fitness Correlation Coefficient", "run" + i + "_k" + k + "_");

                    e.Run();
                    k--;
                }

            }
        }
    }
}
