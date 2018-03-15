using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;

namespace EC_Practicum_2
{
    class Experiment
    {
        Random rnd = new Random();
        public int ColorsCount { get; set; }
        public int GraphSize { get; set; }
        public int PopulationSize { get; set; }
        private Random _random = new Random();
        public Graph[] CurrentPopulation { get; set; }

        private List<Tuple<int, int>> _connections;

        public Experiment(int k, string graphInputPath, int populationSize, string name, int graphSize = 450)
        {
            this.ColorsCount    = k;
            this.PopulationSize = populationSize;
            this.GraphSize      = graphSize;

            CurrentPopulation   = new Graph[populationSize];

            var lines = File.ReadAllLines(graphInputPath);

            _connections = new List<Tuple<int, int>>();

            foreach (string line in lines)
            {
                if (line[0] == 'e')
                {
                    var split = line.Split(' ');
                    _connections.Add(new Tuple<int, int>((Int32.Parse(split[1]) - 1), (Int32.Parse(split[2]) - 1)));
                }
            }

            for (int i = 0; i < PopulationSize; i++)
            {
                var tmp = new Graph(_connections, graphSize, k);
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

        public Graph CrossoverGPX(Graph p1, Graph p2)
        {
            Graph _p1 = DeepClone<Graph>(p1);
            Graph _p2 = DeepClone<Graph>(p2);

            Graph child = new Graph(_connections, GraphSize, ColorsCount);

            var currentParent = _p1;

            while (currentParent.Count > 0) {
                //get greatest cluster from parent
                List<Graph.Vertex> greatestCluster = currentParent.getGreatestColorCluster();
                var colorOfCluster = greatestCluster[0].Color;

                foreach (Graph.Vertex vertex in greatestCluster)
                {
                    child[vertex.Node].Color = colorOfCluster;
                    p1.removeVertex(vertex);
                    p2.removeVertex(vertex);
                }

                if (currentParent == _p1) currentParent = _p2;
                else currentParent = _p1;
            }
            return child;
        }


        public void Run()
        {
            //VDSL(CurrentPopulation[0]);
            //shuffle
            //generate new population
            //for every pair do
            // Crossover function
            // Local Search (to improve)
            // Family selection
            // Add fittest to new population

            var c1 = CrossoverGPX(CurrentPopulation[0], CurrentPopulation[1]);
            Console.WriteLine("debug");

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

        //public static T DeepClone<T>(T obj)
        //{
        //    using (var ms = new MemoryStream())
        //    {
        //        var formatter = new BinaryFormatter();
        //        formatter.Serialize(ms, obj);
        //        ms.Position = 0;

        //        return (T)formatter.Deserialize(ms);
        //    }
        //}
    }
}
