using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC_Practicum_2
{
    class Graph : List<Graph.Vertex>
    {
        public struct Vertex
        {
            public int Node { get; set; }
            public List<int> Edges { get; set; }
            public int Color { get; set; }
            //more stuff??
        };

        private Random random = new Random();

        public Vertex[] graph;
        int graphSize;

        public Graph(string fileName, int graphSize, int k)
        {
            var lines = File.ReadAllLines(fileName);

            for (int i = 0; i < graphSize; i++)
            {

                var v = new Vertex
                {
                    Node = i,
                    Edges = new List<int>(),
                    Color = random.Next(1, k)
                };

                Add(v);
            }

            foreach (string line in lines)
            {
                if (line[0] == 'e')
                {
                    string[] split = line.Split(' ');
                    ConnectNodes((Int32.Parse(split[1]) - 1), (Int32.Parse(split[2]) - 1));
                }
            }
        }


        public List<int> GetEdges(int node)
        {
            return graph[node].Edges;
        }

        public void ConnectNodes(int a, int b)
        {
            if (!graph[a].Edges.Contains(b))
                graph[a].Edges.Add(b);

            if (!graph[b].Edges.Contains(a))
                graph[b].Edges.Add(a);
        }

        public void DisconnectNodes(int a, int b)
        {
            if (graph[a].Edges.Contains(b))
                graph[a].Edges.Remove(b);

            if (graph[b].Edges.Contains(a))
                graph[b].Edges.Remove(a);
        }


        public int GetConflicts()
        {
            var ba = new BitArray(graphSize);
            var conflicts = 0;

            for (var i = 0; i < graphSize; i++)
            {
                var currentColor = graph[i].Color;
                var currentEdges = GetEdges(i);

                foreach (int neighbor in currentEdges)
                {
                    if (ba[neighbor]) continue;

                    if (graph[neighbor].Color == currentColor)
                        conflicts++;
                }

                ba[i] = true;
            }
            return conflicts;
        }
    }
}