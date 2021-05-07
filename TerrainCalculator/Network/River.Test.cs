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
			Network net = new Network();
			River r1 = net.NewRiver();
			Node n1 = net.NewNode();
			n1.Pos.x = 100;
			n1.Pos.y = 0;
			n1.SetDefault();
			r1.Add(n1);

			Node n2 = net.NewNode();
			n2.Pos.x = 0;
			n2.Pos.y = 100;
			r1.Add(n2);

			Node n3 = net.NewNode();
			n3.Pos.x = -100;
			n3.Pos.y = 0;
			r1.Add(n3);

			Vector2 p;
			float delta = 0.0001f;
			float diagX = 62.5f;
			float diagY = 50f;

			r1.SetDirections();
			p = r1.Interpolate2d(0f);
			Assert.That(p.x, Is.EqualTo(100f).Within(delta));
			Assert.That(p.y, Is.EqualTo(0f).Within(delta));

			p = r1.Interpolate2d(0.01f);
			Assert.That(p.x, Is.EqualTo(99.9801f).Within(delta));
			Assert.That(p.y, Is.EqualTo(0.0298f).Within(delta));

			p = r1.Interpolate2d(0.5f);
			Assert.That(p.x, Is.EqualTo(diagX).Within(delta));
			Assert.That(p.y, Is.EqualTo(diagY).Within(delta));

			p = r1.Interpolate2d(0.99f);
			Assert.That(p.x, Is.EqualTo(1.0099f).Within(delta));
			Assert.That(p.y, Is.EqualTo(99.9702f).Within(delta));

			p = r1.Interpolate2d(1f);
			Assert.That(p.x, Is.EqualTo(0f).Within(delta));
			Assert.That(p.y, Is.EqualTo(100f).Within(delta));

			p = r1.Interpolate2d(1.5f);
			Assert.That(p.x, Is.EqualTo(-diagX).Within(delta));
			Assert.That(p.y, Is.EqualTo(diagY).Within(delta));

			p = r1.Interpolate2d(2f);
			Assert.That(p.x, Is.EqualTo(-100f).Within(delta));
			Assert.That(p.y, Is.EqualTo(0f).Within(delta));
		}
	}
}