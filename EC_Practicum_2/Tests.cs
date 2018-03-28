using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EC_Practicum_2
{
    [TestFixture]
    public class MyTestClass
    {
        [Test]
        public void TestGraph()
        {
            var g1 = new Graph(new List<Tuple<int, int>>()
            {
                new Tuple<int, int>(0,1),
                new Tuple<int, int>(1,2),
                new Tuple<int, int>(2,0)
            }, 3, 1);

            for (var i = 0; i < g1.ColorCtn; i++)
                for (var j = 0; j < g1.Count; j++)
                    g1.Color(g1[j], i + 1);

            var vertices = g1.GetGreatestColorCluster();

            Assert.That(vertices.Count() == 3);

        }


        [Test]
        public void TestCluster()
        {
            var g1 = new Graph(new List<Tuple<int, int>>()
            {
                new Tuple<int, int>(0,1),
                new Tuple<int, int>(1,2),
                new Tuple<int, int>(2,0),
                new Tuple<int, int>(3,0),
                new Tuple<int, int>(4,2)
            }, 5, 2);

             for (var j = 0; j < g1.Count; j++)
                g1.Color(g1[j], 1);

            g1[0].Color = 2;
            g1[1].Color = 2;

            var vertices = g1.GetGreatestColorCluster();

            Assert.That(vertices.Count() == 3);

        }
    }
}
