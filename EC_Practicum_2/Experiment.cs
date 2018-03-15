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
        Random rnd = new Random();
        public int ColorsCount { get; set; }
        public int PopulationSize { get; set; }
        public int CrossOverFunction { get; set; }
        private Random _random = new Random();
        public Graph[] CurrentPopulation { get; set; }

        public Experiment(int k, string graphInputPath, int populationSize, string name, int graphSize = 450)
        {
            ColorsCount = k;
            PopulationSize = populationSize;

            CurrentPopulation = new Graph[populationSize];

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
                CurrentPopulation[i] = tmp;
                Console.WriteLine("conflicts: " + tmp.GetConflicts());
            }

            Console.WriteLine("Init of " + name + " done..");
        }

        public void ShufflePopulation() {
            // Knuth shuffle algorithm :: courtesy of Wikipedia :)
            for (int t = 0; t < CurrentPopulation.Length; t++)
            {
                var tmp = CurrentPopulation[t];
                int r = rnd.Next(t, CurrentPopulation.Length);
                CurrentPopulation[t] = CurrentPopulation[r];
                CurrentPopulation[r] = tmp;
            }
        }

        //TODO: 
        public Graph[] GetNewGeneration() {
            Graph[] newPopulation = new Graph[PopulationSize];

            //shuffle current population
            ShufflePopulation();
            for(int i = 0; i < PopulationSize; i += 2)
            {
                var p1 = CurrentPopulation[i];
                var p2 = CurrentPopulation[i+1];

                //todo: GEN CHIDREN WITH CROSSOVER
                //improve WITH LOCAL SEARCH

                var c1 = p1;
                var c2 = p2;



            }

            return newPopulation;
        }

        //public Graph CrossoverGPX(Graph p1, Graph p2) {
        //    Graph child = new Graph()

        //    //get greatest cluster from p1
        //    var greatestCluster = p1.getGreatestColorCluster();
        //}


        public void Run()
        {
            VDSL(CurrentPopulation[0]);
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

                    Console.Write(newConflicts + "|");

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
