﻿using System;
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
            int k = 25;
            while (true) {
                var a = new Experiment(k, "../../le450_15c.txt", 100, "Expiriment 1");
                a.Run();
                k--;
            }
            
            Console.ReadLine();
        }
    }
}
