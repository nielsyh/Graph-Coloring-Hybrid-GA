using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC_Practicum_2
{
    class Expiriment
    {
        int k; //start with k colors
        string graphInputPath; //which graph
        int populationSize; //
        int crossOverFunction;

        Graph[] startPopulation;

        public Expiriment(int k, string graphInputPath, int populationSize, string name) {
            this.k = k;
            this.graphInputPath = graphInputPath;
            this.populationSize = populationSize;

            //generate start pop.
            startPopulation = new Graph[populationSize];
            for (int i = 0; i < this.populationSize; i++) {
                Graph tmp = new Graph(graphInputPath, 450, k);
                startPopulation[i] = tmp;
                Console.WriteLine("conflicts: " + tmp.getConflicts());
            }
            Console.WriteLine("Init of " + name + " done..");
        }

        public void shufflePopulation() { }

        public void run() {
            //shuffle

            //generate new population
            //for every pair do
                // Crossover function
                // Local Search (to improve)
                // Family selection
                // Add fittest to new population

            //check if valid solution found, if so decline k, if not continue..



        }
    }


    
}
