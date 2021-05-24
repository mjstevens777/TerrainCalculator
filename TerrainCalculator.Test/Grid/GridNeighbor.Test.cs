using System;
using System.Collections.Generic;
using NUnit.Framework;
using TerrainCalculator.Grid;
using TerrainCalculator.UnityUI;
using UnityEngine;

namespace TerrainCalculator.Test.Grid
{
	[TestFixture]
	public class GridNeighborTest
	{
		[Test]
		public void TestRadiusOne()
		{
			float[,] expected = new float[,]
			{
				{ -1, 0, 1.00f },
				{ 0, -1, 1.00f },
				{ 0, 1, 1.00f },
				{ 1, 0, 1.00f },
			};
			assertNeighbors(1, expected);
		}

		[Test]
		public void TestRadiusTwo()
		{
			float[,] expected = new float[,]
			{
				{ -1, -1, 1.41f },
				{ -1, 0, 1.00f },
				{ -1, 1, 1.41f },
				{ 0, -1, 1.00f },
				{ 0, 1, 1.00f },
				{ 1, -1, 1.41f },
				{ 1, 0, 1.00f },
				{ 1, 1, 1.41f },
			};

			assertNeighbors(2, expected);
		}

		[Test]
		public void TestRadiusThreeAndAHalf()
		{
			float[,] expected = new float[,]
			{
				{ -3, -1, 3.16f },
				{ -3, 1, 3.16f },
				{ -2, -1, 2.24f },
				{ -2, 1, 2.24f },
				{ -1, -3, 3.16f },
				{ -1, -2, 2.24f },
				{ -1, -1, 1.41f },
				{ -1, 0, 1.00f },
				{ -1, 1, 1.41f },
				{ -1, 2, 2.24f },
				{ -1, 3, 3.16f },
				{ 0, -1, 1.00f },
				{ 0, 1, 1.00f },
				{ 1, -3, 3.16f },
				{ 1, -2, 2.24f },
				{ 1, -1, 1.41f },
				{ 1, 0, 1.00f },
				{ 1, 1, 1.41f },
				{ 1, 2, 2.24f },
				{ 1, 3, 3.16f },
				{ 2, -1, 2.24f },
				{ 2, 1, 2.24f },
				{ 3, -1, 3.16f },
				{ 3, 1, 3.16f },
			};
			assertNeighbors(3.5f, expected);
		}

		[Test]
		public void TestRadiusFour()
		{
			float[,] expected = new float[,]
			{
				{ -3, -2, 3.61f },
				{ -3, -1, 3.16f },
				{ -3, 1, 3.16f },
				{ -3, 2, 3.61f },
				{ -2, -3, 3.61f },
				{ -2, -1, 2.24f },
				{ -2, 1, 2.24f },
				{ -2, 3, 3.61f },
				{ -1, -3, 3.16f },
				{ -1, -2, 2.24f },
				{ -1, -1, 1.41f },
				{ -1, 0, 1.00f },
				{ -1, 1, 1.41f },
				{ -1, 2, 2.24f },
				{ -1, 3, 3.16f },
				{ 0, -1, 1.00f },
				{ 0, 1, 1.00f },
				{ 1, -3, 3.16f },
				{ 1, -2, 2.24f },
				{ 1, -1, 1.41f },
				{ 1, 0, 1.00f },
				{ 1, 1, 1.41f },
				{ 1, 2, 2.24f },
				{ 1, 3, 3.16f },
				{ 2, -3, 3.61f },
				{ 2, -1, 2.24f },
				{ 2, 1, 2.24f },
				{ 2, 3, 3.61f },
				{ 3, -2, 3.61f },
				{ 3, -1, 3.16f },
				{ 3, 1, 3.16f },
				{ 3, 2, 3.61f },
			};
			assertNeighbors(4, expected);
		}

		private void printNeighbors(float radius)
        {
			List<GridNeighbor> neighbors = GridNeighbor.GetAll(radius);
			Console.WriteLine("float[,] expected = new float[,]");
			Console.WriteLine("{");
			for (int k = 0; k < neighbors.Count; k++)
            {
				Console.WriteLine($"{{ {neighbors[k].I}, {neighbors[k].J}, {neighbors[k].Distance.ToString("0.00")}f }},");
			}
			Console.WriteLine("};");
		}

		private void assertNeighbors(float radius, float[,] expected, float delta = 0.01f)
        {
			List<GridNeighbor> neighbors = GridNeighbor.GetAll(radius);
			Assert.That(neighbors.Count, Is.EqualTo(expected.GetLength(0)), "Number of results");
			for (int k = 0; k < neighbors.Count; k++)
            {
				Assert.That(neighbors[k].I, Is.EqualTo(expected[k, 0]), $"I value at position {k}");
				Assert.That(neighbors[k].J, Is.EqualTo(expected[k, 1]), $"J value at position {k}");
				Assert.That(neighbors[k].Distance, Is.EqualTo(expected[k, 2]).Within(delta), $"Dist value at position {k}");
			}
        }
	}
}