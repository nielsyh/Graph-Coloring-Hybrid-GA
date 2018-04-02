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
                var E = new Experiment(k, "../../le450_15c.txt", 80, "CALC Fitness Correlation Coefficient");
                E.CalcFitnessCorrelationCoefficient();
                E.Run();
                

                k--;
            }
        }
    }
}
