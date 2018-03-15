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
            public int color;
            //more stuff??
        };


        Random rnd = new Random();

        public Vertex[] graph;
        int graphSize;

        public Graph(string fileName, int graphSize, int k)
        {
            //todo: readsize
            this.graphSize = graphSize;
            graph = new Vertex[graphSize];
            string[] lines = System.IO.File.ReadAllLines(fileName);

            for(int i = 0; i < graphSize; i++)
            {
                Vertex v = new Vertex();
                v.node = i;
                v.edges = new List<int>();

                //random color?
                v.color = rnd.Next(1, k);
                graph[i] = v;
            }

            foreach (string line in lines)
            {
                if (line[0] == 'e') {
                    string[] split = line.Split(' ');
                    connectNodes((Int32.Parse(split[1]) - 1), (Int32.Parse(split[2]) - 1));
                    }
                }
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


        public int getConflicts() {
            bool[] checkedNodes = new bool[this.graphSize];
            int conflicts = 0;

            for (int i = 0; i < this.graphSize; i++) {
                int currentColor = this.graph[i].color;

                List<int> currentEdges = this.getEdges(i);
                foreach (int neighbor in currentEdges) {
                    if (checkedNodes[neighbor]) continue;

                    if (graph[neighbor].color == currentColor) {
                        conflicts++;
                    }
                }
                checkedNodes[i] = true;

            }

            return conflicts;
        }
    }
}
