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
                var k = 30;

                while (true)
                {
                    var e = new Experiment(k, "../../dsjc250.5.txt", 100, "CALC Fitness Correlation Coefficient", "250x.5.txt" + i + "_k" + k + "_");
                    if (k <= 4) break;
                    e.Run();
                    k--;
                }

            }
        }
    }
}
