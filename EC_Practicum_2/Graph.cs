using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC_Practicum_2
{
    class Graph
    {
        public struct Vertex
        {
            public int node;
            public List<int> edges;
        };

        //todo read size
        //const int graphSize = 451;
        public Vertex[] graph;

        public Graph(string fileName, int graphSize)
        {
            graph = new Vertex[graphSize];
            string[] lines = System.IO.File.ReadAllLines(fileName);

            for(int i = 0; i < graphSize; i++)
            {
                Vertex v = new Vertex();
                v.node = i;
                v.edges = new List<int>();
                graph[i] = v;
            }

            foreach (string line in lines)
            {
                if (line[0] == 'e') {
                    string[] split = line.Split(' ');
                    int v1 = Int32.Parse(split[1]);
                    int v2 = Int32.Parse(split[2]);

                    graph[v1].edges.Add(v2);
                    graph[v2].edges.Add(v1);

                    }
                }
            Console.WriteLine("Init graph done..");

        }
    }
}
