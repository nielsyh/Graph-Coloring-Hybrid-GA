using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EC_Practicum_2
{
    class Graph : List<Graph.Vertex>
    {
        public class Vertex
        {
            public int Node { get; set; }
            public List<int> Edges { get; set; }
            public int Color { get; set; }
            
        };

        private int _k;
        private Random random = new Random();

        public Graph(List<Tuple<int, int>> connections, int graphSize, int colorsCount)
        {
            this._k = colorsCount;
            for (int i = 0; i < graphSize; i++)
            {

                var v = new Vertex
                {
                    Node = i,
                    Edges = new List<int>(),
                    Color = random.Next(1, colorsCount + 1)
                };

                Add(v);
            }

            for (int i = 0; i < connections.Count; i++)
                ConnectNodes(connections[i].Item1, connections[i].Item2);

        }


        public List<int> GetEdges(int node)
        {
            return this[node].Edges;
        }

        public void ConnectNodes(int a, int b)
        {
            if (!this[a].Edges.Contains(b))
                this[a].Edges.Add(b);

            if (!this[b].Edges.Contains(a))
                this[b].Edges.Add(a);
        }

        public void DisconnectNodes(int a, int b)
        {
            if (this[a].Edges.Contains(b))
                this[a].Edges.Remove(b);

            if (this[b].Edges.Contains(a))
                this[b].Edges.Remove(a);
        }

        public List<Vertex> getGreatestColorCluster() {
            int[] colorCnt = new int[_k];

            for (int i = 0; i < this.Count; i++) {
                colorCnt[this[i].Color]++;
            }

            int gcnt = colorCnt.Max(); 
            int gc = colorCnt.ToList().IndexOf(gcnt);

            List<Vertex> biggestCluster = new List<Vertex>();

            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].Color == gc) {
                    biggestCluster.Add(this[i]);
                }
            }

            return biggestCluster;
        }

        public void removeNode() { }


        public int GetConflicts()
        {
            var size = Count;
            var ba = new BitArray(size);
            var conflicts = 0;

            for (var i = 0; i < size; i++)
            {
                var currentColor = this[i].Color;
                var currentEdges = GetEdges(i);

                foreach (int neighbor in currentEdges)
                {
                    if (ba[neighbor]) continue;

                    if (this[neighbor].Color == currentColor)
                        conflicts++;
                }

                ba[i] = true;
            }
            return conflicts;
        }

        public void Color(Vertex node, int i)
        {
            node.Color = i;
        }

        public List<int> GetConfiguration()
        {
            return this.Select(v => v.Color).ToList();
        }
    }
}