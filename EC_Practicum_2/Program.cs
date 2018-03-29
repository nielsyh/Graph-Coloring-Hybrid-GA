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
            int k = 25;
            while (true)
            {
                var Expiriment = new Experiment(k, "../../le450_15c.txt", 100, "Expiriment 1");

                var t1 = Task.Run(() =>
                {
                    Expiriment.Run();
                });

                var t2 = Task.Run(() =>
                {
                    Expiriment.Run();
                });

                //Wait for all tasks
                Task.WaitAll(new[] { t1, t2 });

                k--;
            }
        }
    }
}
