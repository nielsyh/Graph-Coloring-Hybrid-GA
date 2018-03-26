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
        public int BestFitness = int.MaxValue;


        //Measurements
        public double VdslCount         = 0;
        public double GenerationCount  = 0;
        public double CpuTime;

        private List<Tuple<int, int>> _connections;

        public Experiment(int k, string graphInputPath, int populationSize, string name, int graphSize = 450)
        {
            this.ColorsCount = k;
            this.PopulationSize = populationSize;
            this.GraphSize = graphSize;

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

        public IEnumerable<Graph> GetNewGeneration()
        {
            var newPopulation = new List<Graph>();

            //shuffle current population
            ShufflePopulation();

            for (int i = 0; i < PopulationSize; i += 2)
            {
                var p1 = Tuple.Create(CurrentPopulation[i], CurrentPopulation[i].GetConflicts());
                var p2 = Tuple.Create(CurrentPopulation[i + 1], CurrentPopulation[i + 1].GetConflicts());


                var t1 = CrossoverGPX(p1.Item1, p2.Item1);
                var t2 = CrossoverGPX(p1.Item1, p2.Item1);

                VDSL(t1);
                VDSL(t2);

                var c1 = Tuple.Create(t1, t1.GetConflicts());
                var c2 = Tuple.Create(t2, t2.GetConflicts());

                //sort parents
                List<Tuple<Graph, int>> parents = new List<Tuple<Graph, int>>();
                parents.Add(p1);
                parents.Add(p2);
                parents.Sort((x, y) => -1 * y.Item2.CompareTo(x.Item2));

                //sort children
                List<Tuple<Graph, int>> children = new List<Tuple<Graph, int>>();
                children.Add(c1);
                children.Add(c2);
                children.Sort((x, y) => -1 * y.Item2.CompareTo(x.Item2));

                //get best 2, return those
                Graph[] winners = new Graph[2];
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

                //Console.WriteLine("winners: " + winners[0].GetConflicts() + " , " + winners[1].GetConflicts());
                if (winners[0].GetConflicts() < BestFitness) BestFitness = winners[0].GetConflicts();
                newPopulation.AddRange(new[] { winners[0], winners[1] });
            }

            return newPopulation;
        }

        public Graph CrossoverGPX(Graph p1, Graph p2)
        {
            var _p1 = p1.Clone() as Graph;
            var _p2 = p2.Clone() as Graph;

            var child = new Graph(_connections, GraphSize, ColorsCount);

            var currentParent = _p1;
            int i = 1;

            while (currentParent.Count > 0)
            //for (int i = 1; i < ColorsCount + 1; i++)
            {
                //get greatest cluster from parent
                List<Graph.Vertex> greatestCluster = currentParent.GetGreatestColorCluster();

                var crntClr = i;
                if (i > ColorsCount)
                {
                    crntClr = _random.Next(1, ColorsCount + 1);
                }

                foreach (Graph.Vertex vertex in greatestCluster)
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
            //while until no improvement
            //implement selection
            //
            var watch = System.Diagnostics.Stopwatch.StartNew();
            
            while (BestFitness != 0)
            {
                CurrentPopulation = GetNewGeneration().ToArray();
                GenerationCount++;
                Console.WriteLine("avg: " + getAverageFitness());
                Console.WriteLine("best: " + BestFitness);
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

        //check if valid solution found, if so decline k, if not continue..
        /// <summary>
        /// Vertex descent local search
        /// </summary>
        /// <param name="g">The graph on which to perform the search</param>
        public IEnumerable<int> VDSL(Graph g)
        {
            var noImprovement = 0;

            while (noImprovement < 100)
            {

                //Iterate in random order WHY? TODO: answer this <-
                var order = GenerateRandomOrder(g).ToList();
                var oldFitness = g.GetConflicts();

                //O(n) local search, set the color of each v to the least frequent color of its neigbors
                foreach(var vertex in order)
                {
                    var clrcnt = new int[g.ColorCtn + 1];
                    clrcnt[0] = int.MaxValue; //because clr 0 does not exist and I dont want to -1 first everything and then reverse this... =)

                    foreach (int neighbor in g[vertex].Edges)
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

        public int getAverageFitness()
        {
            double total = 0;

            for (int i = 0; i < PopulationSize; i++)
            {
                total = total + CurrentPopulation[i].GetConflicts();
            }

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
