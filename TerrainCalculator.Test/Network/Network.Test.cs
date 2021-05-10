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
			public WaterNetwork Net;
			public List<Node> Nodes;
			public List<Lake> Lakes;
			public List<River> Rivers;

			public TestNet ()
			{
				Nodes = new List<Node>();
				Rivers = new List<River>();
				Lakes = new List<Lake>();

				Net = new WaterNetwork();
				Lakes.Add(Net.NewLake());
				Rivers.Add(Net.NewRiver());
				foreach (int i in Enumerable.Range(0, 6))
                {
					Nodes.Add(Net.NewNode());
                }

				//  5
				//  4   River
				//  1
				// 2 0  Lake
				//  3

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

			var expectedValues = new Dictionary<Node.Key, List<double>>();

			// RiverWidth
			// Set the lake to the same value, then interpolate the river
			//   60
			//   50
			//   40
			// 40  40
			//   40
			testNet.Nodes[0].RiverWidth.SetFixed(40);
			testNet.Nodes[5].RiverWidth.SetFixed(60);
			expectedValues[Node.Key.RiverWidth] =
				new List<double> { 40, 40, 40, 40, 50, 60 };

			// ShoreWidth
			// Set the lake to the same value, then interpolate the river
			//   50
			//   50
			//   50
			// 60  40
			//   50
			testNet.Nodes[0].ShoreWidth.SetFixed(40);
			testNet.Nodes[2].ShoreWidth.SetFixed(60);
			expectedValues[Node.Key.ShoreWidth] =
				new List<double> { 40, 50, 60, 50, 50, 50 };

			testNet.Nodes[0].ShoreDepth.SetFixed(10);
			expectedValues[Node.Key.ShoreDepth] =
				new List<double> { 10, 10, 10, 10, 10, 10 };

			testNet.Nodes[0].Elevation.SetFixed(40);
			expectedValues[Node.Key.Elevation] =
				new List<double> { 40, 40, 40, 40, 40, 40 };

			testNet.Nodes[0].ShoreDepth.SetFixed(10);
			expectedValues[Node.Key.ShoreDepth] =
				new List<double> { 10, 10, 10, 10, 10, 10 };
			testNet.Nodes[0].RiverSlope.SetFixed(2);
			expectedValues[Node.Key.RiverSlope] =
				new List<double> { 2, 2, 2, 2, 2, 2 };

			testNet.Nodes[0].Elevation.SetFixed(40);
			// tan(2 degrees) * segment length
            // segment length = 144.239705
			double deltaZ = 5.036961;
			expectedValues[Node.Key.Elevation] =
				new List<double> { 40, 40, 40, 40, 40 + deltaZ, 40 + 2 * deltaZ };

			WaterNetwork net = testNet.Net;
			net.InterpolateAll();

			double delta = 0.0001;
			foreach (Node node in testNet.Nodes.GetRange(1, 5))
            {
				foreach (var item in expectedValues)
				{
					var key = item.Key;
					var values = item.Value;
					for (int i = 0; i < testNet.Nodes.Count; i++)
					{
						Assert.That(
							testNet.Nodes[i].ImplicitValues[key].Value,
							Is.EqualTo(values[i]).Within(delta),
							$"Node {i} value {key}");
					}
				}
			}
		}

		[Test]
		public void TestRunTwice()
		{
			TestNet testNet = new TestNet();

			WaterNetwork net = testNet.Net;

			testNet.Nodes[0].RiverSlope.SetFixed(2);
			net.InterpolateAll();
			Assert.That(
				testNet.Nodes[5].RiverSlope.Value,
				Is.EqualTo(2));

			testNet.Nodes[0].RiverSlope.SetFixed(3);
			net.InterpolateAll();
			Assert.That(
				testNet.Nodes[5].RiverSlope.Value,
				Is.EqualTo(3));
		}
	}
}