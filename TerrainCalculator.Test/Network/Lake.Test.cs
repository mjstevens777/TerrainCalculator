using NUnit.Framework;
using UnityEngine;
using TerrainCalculator.Network;

namespace TerrainCalculator.Test.Network
{

    [TestFixture]
	public class LakeTest
	{

		[SetUp]
		protected void SetUp()
		{
		}

		[Test]
		public void TestInterpolate()
		{
			WaterNetwork net = new WaterNetwork();
			Lake l1 = net.NewLake();
			Node n1 = net.NewNode();
			n1.Pos.x = 100;
			n1.Pos.y = 0;
			n1.SetDefault();
			l1.Nodes.Add(n1);

			Node n2 = net.NewNode();
			n2.Pos.x = 0;
			n2.Pos.y = 100;
			l1.Nodes.Add(n2);

			Node n3 = net.NewNode();
			n3.Pos.x = -100;
			n3.Pos.y = 0;
			l1.Nodes.Add(n3);

			Node n4 = net.NewNode();
			n4.Pos.x = 0;
			n4.Pos.y = -100;
			l1.Nodes.Add(n4);

			float delta = 0.0001f;
			float diag = 62.5f;
			float edgeLength = 148.60828f;
			Vector2 p;

			var edges = l1.GetEdges();

			foreach(Edge edge in edges)
            {
				Assert.That(edge.InterpPoints.Count, Is.EqualTo(Path.NumSegments + 1));
				Assert.That(edge.Distance, Is.EqualTo(edgeLength).Within(delta));
			}

			p = edges[0].InterpPoints[0];
			Assert.That(p.x, Is.EqualTo(100f).Within(delta));
			Assert.That(p.y, Is.EqualTo(0f).Within(delta));

			p = edges[0].InterpPoints[Path.NumSegments / 2];
			Assert.That(p.x, Is.EqualTo(diag).Within(delta));
			Assert.That(p.y, Is.EqualTo(diag).Within(delta));

			p = edges[0].InterpPoints[Path.NumSegments];
			Assert.That(p.x, Is.EqualTo(0f).Within(delta));
			Assert.That(p.y, Is.EqualTo(100f).Within(delta));

			p = edges[1].InterpPoints[Path.NumSegments / 2];
			Assert.That(p.x, Is.EqualTo(-diag).Within(delta));
			Assert.That(p.y, Is.EqualTo(diag).Within(delta));

			p = edges[1].InterpPoints[Path.NumSegments];
			Assert.That(p.x, Is.EqualTo(-100f).Within(delta));
			Assert.That(p.y, Is.EqualTo(0f).Within(delta));

			p = edges[2].InterpPoints[Path.NumSegments / 2];
			Assert.That(p.x, Is.EqualTo(-diag).Within(delta));
			Assert.That(p.y, Is.EqualTo(-diag).Within(delta));

			p = edges[2].InterpPoints[Path.NumSegments];
			Assert.That(p.x, Is.EqualTo(0f).Within(delta));
			Assert.That(p.y, Is.EqualTo(-100f).Within(delta));

			p = edges[3].InterpPoints[Path.NumSegments / 2];
			Assert.That(p.x, Is.EqualTo(diag).Within(delta));
			Assert.That(p.y, Is.EqualTo(-diag).Within(delta));

			p = edges[3].InterpPoints[Path.NumSegments];
			Assert.That(p.x, Is.EqualTo(100f).Within(delta));
			Assert.That(p.y, Is.EqualTo(0f).Within(delta));
		}
	}
}