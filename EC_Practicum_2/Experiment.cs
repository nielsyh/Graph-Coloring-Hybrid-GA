using System;
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

        public Experiment(int k, string graphInputPath, int populationSize, string name)
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
                var tmp = new Graph(connections, 450, k);
                StartPopulation[i] = tmp;
                Console.WriteLine("conflicts: " + tmp.GetConflicts());
            }

            Console.WriteLine("Init of " + name + " done..");
        }

        public void ShufflePopulation() { }

        public void Run()
        {
            //shuffle
            //generate new population
            //for every pair do
            // Crossover function
            // Local Search (to improve)
            // Family selection
            // Add fittest to new population

            //check if valid solution found, if so decline k, if not continue..
        }

        public async void VDSL(Graph g)
        {
            await Task.Run(() =>
             {

             });
        }

        public int conflicts() => 0;
    }



}
