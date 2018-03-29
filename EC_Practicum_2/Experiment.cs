using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;

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
        public int BestFitness = int.MaxValue;

        //Measurements
        public double VdslCount = 0;
        public double GenerationCount = 0;

        private List<Tuple<int, int>> _connections;

        public Experiment(int k, string graphInputPath, int populationSize, string name, int graphSize = 450)
        {
            ColorsCount = k;
            PopulationSize = populationSize;
            GraphSize = graphSize;

            //Parse Graph text file
            _connections = new List<Tuple<int, int>>();
            var lines = File.ReadAllLines(graphInputPath);
            foreach (string line in lines)
            {
                if (line[0] == 'e')
                {
                    var split = line.Split(' ');
                    _connections.Add(new Tuple<int, int>((Int32.Parse(split[1]) - 1), (Int32.Parse(split[2]) - 1)));
                }
            }

            //initialze all individuals of population.
            CurrentPopulation = new Graph[populationSize];
            for (int i = 0; i < PopulationSize; i++)
            {
                var tmp = new Graph(_connections, graphSize, k);
                VDSL(tmp);
                CurrentPopulation[i] = tmp;
                //Console.WriteLine("conflicts: " + tmp.GetConflicts());
            }

            Console.WriteLine("Init of " + name + " done..");
        }

        public void ShufflePopulation()
        {
            // Knuth shuffle algorithm :: courtesy of Wikipedia :)
            for (int t = 0; t < CurrentPopulation.Length; t++)
            {
                var tmp = CurrentPopulation[t];
                int r = rnd.Next(t, CurrentPopulation.Length);
                CurrentPopulation[t] = CurrentPopulation[r];
                CurrentPopulation[r] = tmp;
            }
        }

        public IEnumerable<Graph> GetNewGeneration(Crossover crossover)
        {
            var newPopulation = new List<Graph>();
            //shuffle current population
            ShufflePopulation();

            var confl = new Dictionary<Graph, int>();

            var startgen = DateTime.Now;
            for (int i = 0; i < PopulationSize; i += 2)
            {

                var p1 = Tuple.Create(CurrentPopulation[i], CurrentPopulation[i].GetConflicts());
                var p2 = Tuple.Create(CurrentPopulation[i + 1], CurrentPopulation[i + 1].GetConflicts());

                Graph t1, t2 = null;

                if (crossover == Crossover.Point)
                {
                    t1 = CrossoverSplit(p1.Item1, p2.Item1);
                    t2 = CrossoverSplit(p1.Item1, p2.Item1);
                }
                else
                {
                    t1 = CrossoverGPX(p1.Item1, p2.Item1);
                    t2 = CrossoverGPX(p1.Item1, p2.Item1);
                }

                var conflictst1 = t1.GetConflicts();
                var conflictst2 = t2.GetConflicts();

                var startLocal = DateTime.Now;
                VDSL(t1);
                VDSL(t2);
                var endLocal = DateTime.Now;
                Console.WriteLine("local search duration " + (endLocal - startLocal));

                var c1 = Tuple.Create(t1, conflictst1);
                var c2 = Tuple.Create(t2, conflictst2);

                //sort parents
                var parents = new List<Tuple<Graph, int>>
                {
                    p1,
                    p2
                };
                parents.Sort((x, y) => -1 * y.Item2.CompareTo(x.Item2));

                //sort children
                var children = new List<Tuple<Graph, int>>
                {
                    c1,
                    c2
                };
                children.Sort((x, y) => -1 * y.Item2.CompareTo(x.Item2));

                var winners = new Graph[2];

                for (int j = 0; j < 2; j++)
                {
                    if (children[0].Item2 > parents[0].Item2)
                    {
                        winners[j] = parents[0].Item1;
                        parents.Remove(parents[0]);
                    }
                    else
                    {
                        winners[j] = children[0].Item1;
                        children.Remove(children[0]);
                    }
                }
                var conflicts = winners[0].GetConflicts();

                if (conflicts < BestFitness) BestFitness = conflicts;
                newPopulation.AddRange(new[] { winners[0], winners[1] });
            }
            var endgen = DateTime.Now;
            Console.WriteLine("Elapsed time for generation " + endgen);
            Console.WriteLine("Best fintess " + BestFitness);
            return newPopulation;
        }

        public Graph CrossoverSplit(Graph g1, Graph g2)
        {
            var c = new Graph(_connections, GraphSize, ColorsCount);

            var item = _random.Next(0, g1.Count);
            for (int i = 0; i < g1.Count; i++)
            {
                if (i < item)
                    c.Color(c[i], g1[i].Color);
                else
                    c.Color(c[i], g2[i].Color);

            }

            return c;
        }


        public Graph CrossoverGPX(Graph p1, Graph p2)
        {
            var _p1 = p1.Clone() as Graph;
            var _p2 = p2.Clone() as Graph;

            var child = new Graph(_connections, GraphSize, ColorsCount);
            var rand = new Random();

            var currentParent = _p1;
            int i = 1;

            while (currentParent.Count > 0)
            {
                //get greatest cluster from parent
                var greatestCluster = new List<Graph.Vertex>();

                greatestCluster = currentParent.GetGreatestColorCluster();

                var crntClr = i;
                if (i > ColorsCount)
                    crntClr = _random.Next(1, ColorsCount + 1);

                foreach (var vertex in greatestCluster)
                {
                    child[vertex.Node].Color = crntClr;
                    _p1.Remove(vertex);
                    _p2.Remove(vertex);
                }

                if (currentParent == _p1) currentParent = _p2;
                else currentParent = _p1;
                i++;
            }
            return child;
        }

        public void Run()
        {
            var watch = Stopwatch.StartNew();
            var lc = 0;

            //very bad fitness
            var bestFitness = 999999999;
            var crossMethod = Crossover.GPX;

            while (BestFitness != 0)
            {
                CurrentPopulation = GetNewGeneration(crossMethod).ToArray();
                var fitness = GetAverageFitness();

                if (fitness < bestFitness)
                {
                    lc = 0;
                    crossMethod = Crossover.GPX;

                    Console.WriteLine("--------------------");
                    Console.WriteLine("Switching to GPX");
                    Console.WriteLine("--------------------");
                }
                else
                {
                    lc++;
                }

                if (lc > 3)
                {
                    crossMethod = Crossover.Point;
                    Console.WriteLine("--------------------");
                    Console.WriteLine("Switching to point");
                    Console.WriteLine("--------------------");
                }

                var variance = ComputeVariance(CurrentPopulation);

                Console.WriteLine("Variance at for new generation : " + variance);

                GenerationCount++;

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine("Solution found for " + ColorsCount);
                Console.WriteLine("------------------------------------");
                Console.WriteLine("cputime: " + elapsedMs);
                Console.WriteLine("vdslcnt: " + VdslCount);
                Console.WriteLine("gencnt:" + GenerationCount);
                Console.WriteLine("------------------------------------");
                Console.WriteLine("best: " + BestFitness);
            }

        }

        private double ComputeVariance(Graph[] currentPopulation)
        {
            //cache conficts to avoid recomputing it below
            var conflicts = currentPopulation.Select(x => x.GetConflicts())
                                             .Select((x, i) => new { index = i, value = x })
                                             .ToDictionary(i => i.index, v => v.value);

            var pop = currentPopulation.Select((x, i) => new { ind = i, val = conflicts[i] * conflicts[i] });
            var mean = pop.Sum(v => conflicts[v.ind]) / currentPopulation.Length;

            //compute variance
            return pop.Sum(x => x.val) / mean;
        }


        //check if valid solution found, if so decline k, if not continue..
        /// <summary>
        /// Vertex descent local search
        /// </summary>
        /// <param name="g">The graph on which to perform the search</param>
        public IEnumerable<int> VDSL(Graph g, double perc = 1)
        {
            var noImprovement = 0;
            var random = new Random();

            while (noImprovement < 30)
            {
                var order = GenerateRandomOrder(g);
                var oldFitness = g.GetConflicts();

                //O(n) local search, set the color of each v to the least frequent color of its neigbors
                foreach (var vertex in order)
                {
                    var cache = new HashSet<int>();
                    var clrcnt = new int[g.ColorCtn + 1];
                    clrcnt[0] = int.MaxValue; //because clr 0 does not exist and I dont want to -1 first everything and then reverse this... =)
                    var list = new List<int>();

                    for (var i = 0; i < g[vertex].Edges.Count; i++)
                    {
                        var pick = -1;
                        pick = g[vertex].Edges[i];
                        list.Add(pick);
                    }

                    foreach (var neighbor in list)
                    {
                        var clr = g[neighbor].Color;
                        clrcnt[clr]++;
                    }

                    g.Color(g[vertex], Array.IndexOf(clrcnt, clrcnt.Min())); //set to colour of the least frequent color of the neighbors (optimal 0)
                }

                if (oldFitness <= g.GetConflicts()) { noImprovement++; }
                VdslCount++;
            }

            //Console.WriteLine("after local search " + g.GetConflicts());
            return g.GetConfiguration();
        }

        public int GetAverageFitness()
        {
            double total = 0;
            for (int i = 0; i < PopulationSize; i++)
                total = total + CurrentPopulation[i].GetConflicts();
            return (int)(total / PopulationSize);
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

            for (var i = 0; i < g.Count; i++)
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
