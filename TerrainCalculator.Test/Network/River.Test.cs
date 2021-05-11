using NUnit.Framework;
using UnityEngine;

namespace TerrainCalculator.Network
{

    [TestFixture]
	public class RiverTest
	{

		[SetUp]
		protected void SetUp()
		{
		}

		[Test]
		public void TestInterpolate()
		{
			WaterNetwork net = new WaterNetwork();
			River r1 = net.NewRiver();
			Node n1 = net.NewNode();
			n1.Pos.x = 100;
			n1.Pos.y = 0;
			n1.SetDefault();
			r1.Nodes.Add(n1);

			Node n2 = net.NewNode();
			n2.Pos.x = 0;
			n2.Pos.y = 100;
			r1.Nodes.Add(n2);

			Node n3 = net.NewNode();
			n3.Pos.x = -100;
			n3.Pos.y = 0;
			r1.Nodes.Add(n3);

			Vector2 p;
			float delta = 0.0001f;
			float diagX = 62.5f;
			float diagY = 50f;
			float edgeLength = 144.239705f;

			var edges = r1.GetEdges();

			foreach (Edge edge in edges)
			{
				Assert.That(edge.InterpPoints.Count, Is.EqualTo(Path.NumSegments + 1));
				Assert.That(edge.Distance, Is.EqualTo(edgeLength).Within(delta));
			}

			p = edges[0].InterpPoints[0];
			Assert.That(p.x, Is.EqualTo(100f).Within(delta));
			Assert.That(p.y, Is.EqualTo(0f).Within(delta));

			p = p = edges[0].InterpPoints[1];
			Assert.That(p.x, Is.EqualTo(99.7815f).Within(delta));
			Assert.That(p.y, Is.EqualTo(0.3259f).Within(delta));

			p = edges[0].InterpPoints[Path.NumSegments / 2];
			Assert.That(p.x, Is.EqualTo(diagX).Within(delta));
			Assert.That(p.y, Is.EqualTo(diagY).Within(delta));

			p = edges[0].InterpPoints[Path.NumSegments - 1];
			Assert.That(p.x, Is.EqualTo(3.44075f).Within(delta));
			Assert.That(p.y, Is.EqualTo(99.67407f).Within(delta));

			p = edges[0].InterpPoints[Path.NumSegments];
			Assert.That(p.x, Is.EqualTo(0f).Within(delta));
			Assert.That(p.y, Is.EqualTo(100f).Within(delta));

			p = edges[1].InterpPoints[0];
			Assert.That(p.x, Is.EqualTo(0f).Within(delta));
			Assert.That(p.y, Is.EqualTo(100f).Within(delta));

			p = edges[1].InterpPoints[Path.NumSegments / 2];
			Assert.That(p.x, Is.EqualTo(-diagX).Within(delta));
			Assert.That(p.y, Is.EqualTo(diagY).Within(delta));

			p = edges[1].InterpPoints[Path.NumSegments];
			Assert.That(p.x, Is.EqualTo(-100f).Within(delta));
			Assert.That(p.y, Is.EqualTo(0f).Within(delta));
		}
	}
}