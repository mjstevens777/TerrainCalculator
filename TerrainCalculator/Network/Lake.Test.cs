using NUnit.Framework;
using UnityEngine;

namespace TerrainCalculator.Network
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
			Network net = new Network();
			Lake l1 = net.NewLake();
			Node n1 = net.NewNode();
			n1.Pos.x = 100;
			n1.Pos.y = 0;
			n1.SetDefault();
			l1.Add(n1);

			Node n2 = net.NewNode();
			n2.Pos.x = 0;
			n2.Pos.y = 100;
			l1.Add(n2);

			Node n3 = net.NewNode();
			n3.Pos.x = -100;
			n3.Pos.y = 0;
			l1.Add(n3);

			Node n4 = net.NewNode();
			n4.Pos.x = 0;
			n4.Pos.y = -100;
			l1.Add(n4);

			float delta = 0.0001f;
			float diag = 62.5f;
			Vector2 p;

			l1.SetDirections();
			p = l1.Interpolate2d(0f);
			Assert.That(p.x, Is.EqualTo(100f).Within(delta));
			Assert.That(p.y, Is.EqualTo(0f).Within(delta));

			p = l1.Interpolate2d(0.5f);
			Assert.That(p.x, Is.EqualTo(diag).Within(delta));
			Assert.That(p.y, Is.EqualTo(diag).Within(delta));

			p = l1.Interpolate2d(1f);
			Assert.That(p.x, Is.EqualTo(0f).Within(delta));
			Assert.That(p.y, Is.EqualTo(100f).Within(delta));

			p = l1.Interpolate2d(1.5f);
			Assert.That(p.x, Is.EqualTo(-diag).Within(delta));
			Assert.That(p.y, Is.EqualTo(diag).Within(delta));

			p = l1.Interpolate2d(2f);
			Assert.That(p.x, Is.EqualTo(-100f).Within(delta));
			Assert.That(p.y, Is.EqualTo(0f).Within(delta));

			p = l1.Interpolate2d(2.5f);
			Assert.That(p.x, Is.EqualTo(-diag).Within(delta));
			Assert.That(p.y, Is.EqualTo(-diag).Within(delta));

			p = l1.Interpolate2d(3f);
			Assert.That(p.x, Is.EqualTo(0f).Within(delta));
			Assert.That(p.y, Is.EqualTo(-100f).Within(delta));

			p = l1.Interpolate2d(3.5f);
			Assert.That(p.x, Is.EqualTo(diag).Within(delta));
			Assert.That(p.y, Is.EqualTo(-diag).Within(delta));

			p = l1.Interpolate2d(4f);
			Assert.That(p.x, Is.EqualTo(100f).Within(delta));
			Assert.That(p.y, Is.EqualTo(0f).Within(delta));
		}
	}
}