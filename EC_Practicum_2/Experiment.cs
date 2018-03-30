using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;
using System.Collections.Concurrent;

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
        private object _lock = new object();

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
            var cache = new ConcurrentBag<Graph>();
            CurrentPopulation = new Graph[populationSize];
            var tasks = new List<Task>();
            for (int i = 0; i < PopulationSize; i++)
            {
                var tmp = new Graph(_connections, graphSize, k);
                int j = i;

                var t = Task.Run(() =>
                {
                    VDSL(tmp);
                    cache.Add(tmp);
                });

                tasks.Add(t);
            }

            Task.WaitAll(tasks.ToArray());

            int cnt = 0;
            foreach (var item in cache)
            {
                CurrentPopulation[cnt] = item;
                cnt++;
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

        private class Match
        {
            public Graph G1 { get; set; }
            public int G1Conflicts { get; set; }
            public Graph G2 { get; set; }
            public int G2Conflicts { get; set; }
        }

        public IEnumerable<Graph> GetNewGeneration(Crossover crossover)
        {
            var newPopulation = new BlockingCollection<Graph>();
            //shuffle current population
            ShufflePopulation();

            var confl = new Dictionary<Graph, int>();
            var startgen = DateTime.Now;
            var q = new List<Match>();

            //cache all the conflicts count
            var _conflictHash = new ConcurrentDictionary<Graph, int>();

            for (int i = 0; i < PopulationSize; i += 2)
            {
                var match = new Match()
                {
                    G1 = CurrentPopulation[i],
                    G1Conflicts = _conflictHash[CurrentPopulation[i]] = CurrentPopulation[i].GetConflicts(),
                    G2 = CurrentPopulation[i + 1],
                    G2Conflicts = _conflictHash[CurrentPopulation[i + 1]] = CurrentPopulation[i + 1].GetConflicts()
                };
                q.Add(match);
            }

            int size = GraphSize;
            var cc = ColorsCount;

            Parallel.ForEach(q, x =>
            {
                Graph t1, t2 = null;

                t1 = CrossoverGPX(x.G1, x.G2, size, cc);
                t2 = CrossoverGPX(x.G1, x.G2, size, cc);

                var t1x = Task.Run(() =>
                {
                    VDSL(t1);
                });

                var t2x = Task.Run(() =>
                {
                    VDSL(t2);
                });

                Task.WaitAll(new[] {
                    t1x, t2x
                });

                var conflictst1 = t1.GetConflicts();
                var conflictst2 = t2.GetConflicts();

                _conflictHash[t1] = conflictst1;
                _conflictHash[t2] = conflictst2;

                //sort parents
                var parents = new List<Tuple<Graph, int>>
                    {
                        new Tuple<Graph, int>(x.G1,x.G1Conflicts),
                        new Tuple<Graph, int>(x.G2,x.G2Conflicts)
                    };


                parents.Sort((v, y) => -1 * y.Item2.CompareTo(v.Item2));

                //sort children
                var children = new List<Tuple<Graph, int>>
                    {
                        new Tuple<Graph, int>(t1,conflictst1),
                        new Tuple<Graph, int>(t2,conflictst2)
                    };
                children.Sort((v, y) => -1 * y.Item2.CompareTo(v.Item2));

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

                var conflicts = _conflictHash[winners[0]];
                if (conflicts < BestFitness) BestFitness = conflicts;

                newPopulation.Add(winners[0]);
                newPopulation.Add(winners[1]);

            });
            Console.WriteLine("Best fintess " + BestFitness);
            Console.WriteLine("AVG Fitness: " + getAverageFitness());
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

        public Graph CrossoverGPX(Graph p1, Graph p2, int size, int colors)
        {
            var _p1 = p1.Clone() as Graph;
            var _p2 = p2.Clone() as Graph;

            var child = new Graph(_connections, size, colors);
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
                {
                    lock (_lock)
                    {
                        crntClr = _random.Next(1, ColorsCount + 1);
                    }
                }

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

            //very bad fitness
            var crossMethod = Crossover.GPX;

            while (BestFitness != 0)
            {
                CurrentPopulation = GetNewGeneration(crossMethod).ToArray();
                var variance = ComputeVariance(CurrentPopulation);
                Console.WriteLine("Variance at for new generation : " + variance);
                GenerationCount++;
            }
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("Solution found for " + ColorsCount);
            Console.WriteLine("------------------------------------");
            Console.WriteLine("cputime: " + elapsedMs);
            Console.WriteLine("vdslcnt: " + VdslCount);
            Console.WriteLine("gencnt:" + GenerationCount);
            Console.WriteLine("------------------------------------");

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
        public void VDSL(Graph g)
        {
            var noImprovement = 0;
            var iterCount = 0;

            while (noImprovement < 20)
            {
                var oldFitness = g.GetConflicts();

                //O(n) local search, set the color of each v to the least frequent color of its neigbors
                for (int i = 0; i < g.Count; i++)
                {
                    var vertex = g[i];

                    var clrcnt = new int[g.ColorCtn + 1];
                    clrcnt[0] = int.MaxValue; //because clr 0 does not exist and I dont want to -1 first everything and then reverse this... =)

                    for (int e = 0; e < g[i].Edges.Count; e++)
                    {
                        var clr = g[e].Color;
                        clrcnt[clr]++;
                    }

                    g.Color(vertex, Array.IndexOf(clrcnt, clrcnt.Min())); //set to colour of the least frequent color of the neighbors (optimal 0)


                }

                var conflicts = g.GetConflicts();
                if (oldFitness <= conflicts) { noImprovement++; }
                else
                {
                    noImprovement = 0;
                    iterCount = 0;
                    if (iterCount > 15)
                    {
                        Console.WriteLine("improvement after: " + iterCount);
                    }
                }
                lock (_lock)
                {
                    VdslCount++;
                    iterCount++;
                }
            }
        }

        public int getAverageFitness()
        {
            double total = 0;
            for (int i = 0; i < PopulationSize; i++)
            {
                total = total + CurrentPopulation[i].GetConflicts();
            }
            return (int)(total / PopulationSize);
        }

        public void Shuffle(Graph g)
        {
            var random = new Random();
            var n = g.Count;
            while (n > 1)
            {
                n--;
                var i = random.Next(n + 1);
                var temp = g[i];
                g[i] = g[n];
                g[n] = temp;
            }
        }

    }
}