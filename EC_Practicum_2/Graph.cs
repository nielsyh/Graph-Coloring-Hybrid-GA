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
            //more stuff??
        };

        public Vertex[] graph;

        public Graph(string fileName, int graphSize)
        {
            //todo: readsize
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
                    connectNodes((Int32.Parse(split[1]) - 1), (Int32.Parse(split[2]) - 1));
                    }
                }
            Console.WriteLine("Init graph done..");
        }


        public List<int> getEdges(int node) {
            return graph[node].edges;
        }

        public void connectNodes(int a, int b) {
            if (!graph[a].edges.Contains(b)) {
                graph[a].edges.Add(b);
            }

            if (!graph[b].edges.Contains(a))
            {
                graph[b].edges.Add(a);
            }
        }

        public void disconnectNodes(int a, int b) {
            if (graph[a].edges.Contains(b))
            {
                graph[a].edges.Remove(b);
            }

            if (graph[b].edges.Contains(a))
            {
                graph[b].edges.Remove(a);
            }
        }
    }
}
