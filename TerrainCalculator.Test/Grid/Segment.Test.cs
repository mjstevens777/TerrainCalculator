using System;
using System.Collections.Generic;
using NUnit.Framework;
using TerrainCalculator.Grid;
using UnityEngine;

namespace TerrainCalculator.Test.Grid
{
	[TestFixture]
	public class SegmentTest
	{

		[SetUp]
		protected void SetUp()
		{
		}

		[Test]
		public void TestDrawSame()
		{
			SegmentNode start = new SegmentNode(
				new Vector2(4.6f, 2.2f), 0, 0, 0, 0, 0);
			SegmentNode end = new SegmentNode(
				new Vector2(4.6f, 2.2f), 1, 1, 1, 1, 1);
			Segment segment = new Segment(start, end, false);

			List<SegmentNode> nodes = new List<SegmentNode>(segment.Draw());
			Assert.That(nodes.Count, Is.EqualTo(1), "Output length");
			Assert.That(nodes[0].Pos.x, Is.EqualTo(5), "x");
			Assert.That(nodes[0].Pos.y, Is.EqualTo(2), "y");
			Assert.That(nodes[0].Elevation, Is.EqualTo(0.5), "t");

			end = new SegmentNode(
				start.Pos, 1, 1, 1, 1, 1);
			segment = new Segment(start, end, false);
			nodes = new List<SegmentNode>(segment.Draw());
			Assert.That(nodes.Count, Is.EqualTo(1), "Output length");
			Assert.That(nodes[0].Pos.x, Is.EqualTo(5), "x");
			Assert.That(nodes[0].Pos.y, Is.EqualTo(2), "y");
			Assert.That(nodes[0].Elevation, Is.EqualTo(0.5f), "t");
		}

		[Test]
		public void TestDrawX()
		{
			SegmentNode start = new SegmentNode(
				new Vector2(4.6f, 2.2f), 0, 0, 0, 0, 0);
			SegmentNode end = new SegmentNode(
				new Vector2(11.3f, 4.7f), 1, 1, 1, 1, 1);
			Segment segment = new Segment(start, end, false);

			List<int> expectedX = new List<int> { 5, 6, 7, 8, 9, 10, 11 };
			List<int> expectedY = new List<int> { 2, 3, 3, 3, 4, 4, 5 };
			List<float> expectedZ = new List<float> {
				0.060f, 0.209f, 0.358f, 0.507f, 0.657f, 0.806f, 0.955f };
			float delta = 0.001f;

			List<SegmentNode> nodes = new List<SegmentNode>(segment.Draw());
			Assert.That(nodes.Count, Is.EqualTo(expectedX.Count), "Output length");
			for (int i = 0; i < expectedX.Count; i++)
			{
				Assert.That(nodes[i].Pos.x, Is.EqualTo(expectedX[i]), $"x {i}");
				Assert.That(nodes[i].Pos.y, Is.EqualTo(expectedY[i]), $"y {i}");
                Assert.That(nodes[i].Elevation, Is.EqualTo(expectedZ[i]).Within(delta), $"t {i}");
            }

			expectedX.Reverse();
			expectedY.Reverse();
			expectedZ.Reverse();

			segment = new Segment(end, start, false);
			nodes = new List<SegmentNode>(segment.Draw());
			Assert.That(nodes.Count, Is.EqualTo(expectedX.Count), "Output length");
			for (int i = 0; i < expectedX.Count; i++)
			{
				Assert.That(nodes[i].Pos.x, Is.EqualTo(expectedX[i]), $"x {i}");
				Assert.That(nodes[i].Pos.y, Is.EqualTo(expectedY[i]), $"y {i}");
				Assert.That(nodes[i].Elevation, Is.EqualTo(expectedZ[i]).Within(delta), $"t {i}");
			}
		}

		[Test]
		public void TestDrawY()
		{
			SegmentNode start = new SegmentNode(
				new Vector2(3.7f, 5.4f), 0, 0, 0, 0, 0);
			SegmentNode end = new SegmentNode(
				new Vector2(5.4f, 12.7f), 1, 1, 1, 1, 1);
			Segment segment = new Segment(start, end, false);

			List<int> expectedX = new List<int> { 4, 4, 4, 4, 5, 5, 5, 5, 5 };
			List<int> expectedY = new List<int> { 5, 6, 7, 8, 9, 10, 11, 12, 13 };
			List<float> expectedZ = new List<float> {
				-0.055f, 0.082f, 0.219f, 0.356f, 0.493f, 0.630f, 0.767f, 0.904f, 1.041f };
			float delta = 0.001f;

			List<SegmentNode> nodes = new List<SegmentNode>(segment.Draw());
			Assert.That(nodes.Count, Is.EqualTo(expectedX.Count), "Output length");
			for (int i = 0; i < expectedX.Count; i++)
			{
				Assert.That(nodes[i].Pos.x, Is.EqualTo(expectedX[i]), $"x {i}");
				Assert.That(nodes[i].Pos.y, Is.EqualTo(expectedY[i]), $"y {i}");
				Assert.That(nodes[i].Elevation, Is.EqualTo(expectedZ[i]).Within(delta), $"t {i}");
			}

			expectedX.Reverse();
			expectedY.Reverse();
			expectedZ.Reverse();

			segment = new Segment(end, start, false);
			nodes = new List<SegmentNode>(segment.Draw());
			Assert.That(nodes.Count, Is.EqualTo(expectedX.Count), "Output length");
			for (int i = 0; i < expectedX.Count; i++)
			{
				Assert.That(nodes[i].Pos.x, Is.EqualTo(expectedX[i]), $"x {i}");
				Assert.That(nodes[i].Pos.y, Is.EqualTo(expectedY[i]), $"y {i}");
				Assert.That(nodes[i].Elevation, Is.EqualTo(expectedZ[i]).Within(delta), $"t {i}");
			}
		}
	}
}