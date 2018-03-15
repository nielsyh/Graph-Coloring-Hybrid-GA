using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC_Practicum_2
{
    class Experiment
    {
        public int ColorsCount { get; set; }
        public int PopulationSize { get; set; }
        public int CrossOverFunction { get; set; }
        public Graph[] StartPopulation { get; set; }
        private Random _random = new Random();

        public Experiment(int k, string graphInputPath, int populationSize, string name, int graphSize = 450)
        {
            ColorsCount = k;
            PopulationSize = populationSize;

            StartPopulation = new Graph[populationSize];

            var lines = File.ReadAllLines(graphInputPath);

            var connections = new List<Tuple<int, int>>();

            foreach (string line in lines)
            {
                if (line[0] == 'e')
                {
                    var split = line.Split(' ');
                    connections.Add(new Tuple<int, int>((Int32.Parse(split[1]) - 1), (Int32.Parse(split[2]) - 1)));
                }
            }

            for (int i = 0; i < PopulationSize; i++)
            {
                var tmp = new Graph(connections, graphSize, k);
                StartPopulation[i] = tmp;
                Console.WriteLine("conflicts: " + tmp.GetConflicts());
            }

            Console.WriteLine("Init of " + name + " done..");
        }

        public void ShufflePopulation() { }

        public void Run()
        {
            VDSL(StartPopulation[0]);
            //shuffle
            //generate new population
            //for every pair do
            // Crossover function
            // Local Search (to improve)
            // Family selection
            // Add fittest to new population

            //check if valid solution found, if so decline k, if not continue..
        }


        /// <summary>
        /// Vertex descent local search
        /// </summary>
        /// <param name="g">The graph on which to perform the search</param>
        public IEnumerable<int> VDSL(Graph g)
        {
            //Iterate in random order 
            var order = GenerateRandomOrder(g);

            //Console.WriteLine(order.Select(x => x.ToString()).Aggregate((x, y) => x.ToString() + " " + y.ToString()));
            var initialConflicts = g.GetConflicts();
            var initialConfiguration = g.GetConfiguration();

            var bestConflictsMinimizer = initialConflicts;
            var bestConflictConfiguration = initialConfiguration.Select(c => c).ToList();

            Console.WriteLine("Before local search: " + initialConflicts);

            foreach (var node in g)
            {
                var minimalConflicts = bestConflictsMinimizer;

                int bestColor = node.Color;
                int originalColor = bestColor;

                for (var i = 1; i <= ColorsCount; i++)
                {
                    if (i == originalColor) continue;

                    g.Color(node, i);
                    var newConflicts = g.GetConflicts();

                    //Console.Write(newConflicts + "|");

                    if (newConflicts < minimalConflicts)
                    {
                        minimalConflicts = newConflicts;
                        bestColor = i;
                    }
                    else
                        g.Color(node, bestColor);
                }
                Console.WriteLine();

                if (minimalConflicts < bestConflictsMinimizer)
                {
                    bestConflictsMinimizer = minimalConflicts;
                    bestConflictConfiguration = g.GetConfiguration();
                }

            }

            if (bestConflictsMinimizer > initialConflicts)
                bestConflictConfiguration = initialConfiguration;

            Console.WriteLine("After local search " + bestConflictsMinimizer);
            return bestConflictConfiguration;
        }

        /// <summary>
        /// Maps every index to a random index(could be optimized cause of the while cycle can, in theory, run indefinitely)
        /// Altho, in practice this works very fast
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        private IEnumerable<int> GenerateRandomOrder(Graph g)
        {
            var ba = new BitArray(g.Count);
            var data = new int[g.Count];

            for (int i = 0; i < g.Count; i++)
            {
                //Random next is exclusive of the upper bound
                var n = _random.Next(1, g.Count + 1);

                while (ba[n - 1])
                    n = _random.Next(1, g.Count + 1);

                ba[n - 1] = true;

                data[i] = n - 1;
            }

            return data;
        }
    }
}
