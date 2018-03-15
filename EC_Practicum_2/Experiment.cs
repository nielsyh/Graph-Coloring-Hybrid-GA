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
        Random rnd = new Random();
        public int ColorsCount { get; set; }
        public int PopulationSize { get; set; }
        public int CrossOverFunction { get; set; }
        public Graph[] CurrentPopulation { get; set; }

        public Experiment(int k, string graphInputPath, int populationSize, string name)
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
                var tmp = new Graph(connections, 450, k);
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

        public Graph CrossoverGPX(Graph p1, Graph p2) {
            //get biggest cluster from p1

        }


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
