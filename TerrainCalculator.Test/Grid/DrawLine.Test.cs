using System;
using System.Collections.Generic;
using NUnit.Framework;
using TerrainCalculator.Grid;
using UnityEngine;

namespace TerrainCalculator.Test.Grid
{
	[TestFixture]
	public class DrawLineTest
	{

		[SetUp]
		protected void SetUp()
		{
		}

		[Test]
		public void TestDrawSame()
		{
			Vector2 start = new Vector2(4.6f, 2.2f);
			Vector2 end = new Vector2(4.7f, 2.3f);

			List<Vector3> positions = new List<Vector3>(DrawLine.draw(start, end));
			Assert.That(positions.Count, Is.EqualTo(1), "Output length");
			Assert.That(positions[0].x, Is.EqualTo(5), "x");
			Assert.That(positions[0].y, Is.EqualTo(2), "y");
			Assert.That(positions[0].z, Is.EqualTo(0.5), "t");

			positions = new List<Vector3>(DrawLine.draw(start, start));
			Assert.That(positions.Count, Is.EqualTo(1), "Output length");
			Assert.That(positions[0].x, Is.EqualTo(5), "x");
			Assert.That(positions[0].y, Is.EqualTo(2), "y");
			Assert.That(positions[0].z, Is.EqualTo(0.5), "t");
		}

		[Test]
		public void TestDrawX()
		{
			Vector2 start = new Vector2(4.6f, 2.2f);
			Vector2 end = new Vector2(11.3f, 4.7f);

			List<int> expectedX = new List<int> { 5, 6, 7, 8, 9, 10, 11 };
			List<int> expectedY = new List<int> { 2, 3, 3, 3, 4, 4, 5 };
			List<float> expectedT = new List<float> {
				0.060f, 0.209f, 0.358f, 0.507f, 0.657f, 0.806f, 0.955f };
			float delta = 0.001f;

			List<Vector3> positions = new List<Vector3>(DrawLine.draw(start, end));
			Assert.That(positions.Count, Is.EqualTo(expectedX.Count), "Output length");
			for (int i = 0; i < expectedX.Count; i++)
			{
				Assert.That(positions[i].x, Is.EqualTo(expectedX[i]), $"x {i}");
				Assert.That(positions[i].y, Is.EqualTo(expectedY[i]), $"y {i}");
                Assert.That(positions[i].z, Is.EqualTo(expectedT[i]).Within(delta), $"t {i}");
            }

			expectedX.Reverse();
			expectedY.Reverse();
			expectedT.Reverse();
			for (int i = 0; i < expectedT.Count; i++) expectedT[i] = 1 - expectedT[i];

			positions = new List<Vector3>(DrawLine.draw(end, start));
			Assert.That(positions.Count, Is.EqualTo(expectedX.Count), "Output length");
			for (int i = 0; i < expectedX.Count; i++)
			{
				Assert.That(positions[i].x, Is.EqualTo(expectedX[i]), $"reverse x {i}");
				Assert.That(positions[i].y, Is.EqualTo(expectedY[i]), $"reverse y {i}");
			}
		}

		[Test]
		public void TestDrawY()
		{
			Vector2 start = new Vector2(3.7f, 5.4f);
			Vector2 end = new Vector2(5.4f, 12.7f);

			List<int> expectedX = new List<int> { 4, 4, 4, 4, 5, 5, 5, 5, 5 };
			List<int> expectedY = new List<int> { 5, 6, 7, 8, 9, 10, 11, 12, 13 };
			List<float> expectedT = new List<float> {
				-0.055f, 0.082f, 0.219f, 0.356f, 0.493f, 0.630f, 0.767f, 0.904f, 1.041f };
			float delta = 0.001f;

			List<Vector3> positions = new List<Vector3>(DrawLine.draw(start, end));
			Assert.That(positions.Count, Is.EqualTo(expectedX.Count), "Output length");
			for (int i = 0; i < expectedX.Count; i++)
			{
				Assert.That(positions[i].x, Is.EqualTo(expectedX[i]), $"x {i}");
				Assert.That(positions[i].y, Is.EqualTo(expectedY[i]), $"y {i}");
				Assert.That(positions[i].z, Is.EqualTo(expectedT[i]).Within(delta), $"t {i}");
			}

			expectedX.Reverse();
			expectedY.Reverse();
            expectedT.Reverse();
            for (int i = 0; i < expectedT.Count; i++) expectedT[i] = 1 - expectedT[i];

            positions = new List<Vector3>(DrawLine.draw(end, start));
			Assert.That(positions.Count, Is.EqualTo(expectedX.Count), "Output length");
			for (int i = 0; i < expectedX.Count; i++)
			{
				Assert.That(positions[i].x, Is.EqualTo(expectedX[i]), $"reverse x {i}");
				Assert.That(positions[i].y, Is.EqualTo(expectedY[i]), $"reverse y {i}");
				Assert.That(positions[i].z, Is.EqualTo(expectedT[i]).Within(delta), $"t {i}");
			}
		}
	}
}