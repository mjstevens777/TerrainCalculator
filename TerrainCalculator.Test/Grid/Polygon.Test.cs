using System;
using System.Collections.Generic;
using NUnit.Framework;
using TerrainCalculator.Grid;
using UnityEngine;

namespace TerrainCalculator.Test.Grid
{
	[TestFixture]
	public class PolygonTest
	{

		[SetUp]
		protected void SetUp()
		{
		}

		[Test]
		public void TestBasic()
		{
			SegmentNode start = new SegmentNode(
				new Vector2(5f, 3f), 0, 0, 0, 0, 0);
			SegmentNode mid = new SegmentNode(
				new Vector2(10f, 8f), 0, 0, 0, 0, 0);
			SegmentNode end = new SegmentNode(
				new Vector2(5f, 8f), 0, 0, 0, 0, 0);
			List<Segment> segments = new List<Segment>();
			segments.Add(new Segment(start, mid, true));
			segments.Add(new Segment(mid, end, true));
			segments.Add(new Segment(end, start, true));

			Polygon poly = new Polygon(segments);
			int[,] coords = poly.Compute();
			int[,] expected = new int[,]
			{
				{5, 6},
				{6, 6},
				{6, 7},
				{7, 6},
				{7, 7},
				{7, 8},
            };
			Assert.That(coords.GetLength(0), Is.EqualTo(expected.GetLength(0)), "Num points");
			Assert.That(coords.GetLength(1), Is.EqualTo(expected.GetLength(1)), "Columns");
			for (int i = 0; i < expected.GetLength(0); i++)
            {
				Assert.That(coords[i, 0], Is.EqualTo(expected[i, 0]), $"i {i}");
				Assert.That(coords[i, 1], Is.EqualTo(expected[i, 1]), $"j {i}");
			}
		}
	}
}