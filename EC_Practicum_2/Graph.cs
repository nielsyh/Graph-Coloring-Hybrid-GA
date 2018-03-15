using System;
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
            public int node;
            public List<int> edges;
            public int color;
            //more stuff??
        };

        private Random random = new Random();

        public Graph(string fileName, int graphSize, int k)
        {
            //todo: readsize
            var lines = File.ReadAllLines(fileName);

            for (int i = 0; i < graphSize; i++)
            {
                var v = new Vertex
                {
                    node = i,
                    edges = new List<int>(),
                    //random color?
                    color = random.Next(1, k)
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
            return graph[node].edges;
        }

        public void ConnectNodes(int a, int b)
        {
            if (!graph[a].edges.Contains(b))
            {
                graph[a].edges.Add(b);
            }

            if (!graph[b].edges.Contains(a))
            {
                graph[b].edges.Add(a);
            }
        }

        public void DisconnectNodes(int a, int b)
        {
            if (graph[a].edges.Contains(b))
            {
                graph[a].edges.Remove(b);
            }

            if (graph[b].edges.Contains(a))
            {
                graph[b].edges.Remove(a);
            }
        }


        public int FitnessEval()
        {

            return 0;
        }
    }
}
