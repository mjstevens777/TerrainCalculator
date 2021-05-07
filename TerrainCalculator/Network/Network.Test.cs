using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace TerrainCalculator.Network
{

	[TestFixture]
	public class NetworkTest
	{
		private class TestNet
        {
			public Network Net;
			public List<Node> Nodes;
			public List<Lake> Lakes;
			public List<River> Rivers;

			public TestNet ()
			{
				Nodes = new List<Node>();
				Rivers = new List<River>();
				Lakes = new List<Lake>();

				Net = new Network();
				Lakes.Add(Net.NewLake());
				Rivers.Add(Net.NewRiver());
				foreach (int i in Enumerable.Range(0, 6))
                {
					Nodes.Add(Net.NewNode());
                }

				Nodes[0].Pos.x = 100;
				Nodes[0].Pos.y = 0;
				Nodes[0].SetDefault();
				Lakes[0].Add(Nodes[0]);

				Nodes[1].Pos.x = 0;
				Nodes[1].Pos.y = 100;
				Lakes[0].Add(Nodes[1]);

				Nodes[2].Pos.x = -100;
				Nodes[2].Pos.y = 0;
				Lakes[0].Add(Nodes[2]);

				Nodes[3].Pos.x = 0;
				Nodes[3].Pos.y = -100;
				Lakes[0].Add(Nodes[3]);

				
				Rivers[0].Add(Nodes[1]);

				Nodes[4].Pos.x = 100;
				Nodes[4].Pos.y = 200;
				Rivers[0].Add(Nodes[4]);

				Nodes[5].Pos.x = 0;
				Nodes[5].Pos.y = 300;
				Rivers[0].Add(Nodes[5]);
			}
        }

		[SetUp]
		protected void SetUp()
		{
		}

		[Test]
		public void TestInterpolate()
		{
			TestNet testNet = new TestNet();

			Network net = testNet.Net;
			net.BuildGraph();
			net.InterpolateValue(Node.ImplicitKey.Slope);
			net.InterpolateValue(Node.ImplicitKey.Width);
			net.InterpolateValue(Node.ImplicitKey.Depth);
			net.InterpolateZ();

			foreach (Node node in testNet.Nodes.GetRange(1, 5))
            {
				Assert.IsTrue(node.Slope.IsImplicit);
				Assert.IsTrue(node.Width.IsImplicit);
				Assert.IsTrue(node.Depth.IsImplicit);
			}
		}
	}
}