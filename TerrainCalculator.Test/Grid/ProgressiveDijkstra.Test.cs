using System;
using System.Collections.Generic;
using NUnit.Framework;
using TerrainCalculator.Grid;
using TerrainCalculator.UnityUI;
using UnityEngine;

namespace TerrainCalculator.Test.Grid
{
	[TestFixture]
	public class ZCalculatorTest
	{

		[SetUp]
		protected void SetUp()
		{
		}

		[Test]
		public void TestZValue()
		{
			var algorithm = new ProgressiveDijkstra<ZValue>();


			ZValue defaultZ = new ZValue(landSlope: 0.5f, riverSlope: 0, elevation: 500);

			ZValue[,] grid = new ZValue[5, 5];
			for (int i = 0; i < 5; i++)
            {
				for (int j = 0; j < 5; j++)
                {
					grid[i, j] = defaultZ;
                }
            }

			for (int i = 0; i < 5; i++) grid[i, 2] = new ZValue(0.5f, 1, elevation: i + 1);
			grid[2, 2] = new ZValue(0.5f, 1, elevation: 10);  // Make an outlier above the rest
			algorithm.Reset(grid);
			for (int i = 0; i < 5; i++) algorithm.Lock(i, 2);

			bool done = false;
			for (int i = 0; i < 200; i++) {
				done = algorithm.Iterate();
				if (done) break;
			}
			Assert.IsTrue(done, "Loop should terminate");


			float[,] expectedZ = new float[,]
			{
				{4f, 2.5f, 1f, 2.5f, 4f },
				{4.35f, 3.12f, 2f, 3.12f, 4.35f },
				{5.24f, 4.12f, 10f, 4.12f, 5.24f },
				{6.24f, 5.35f, 4f, 5.35f, 6.24f },
				{7.35f, 6.12f, 5f, 6.12f, 7.35f },
			};

			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j < 5; j++)
				{
					float actual = algorithm.Get(i, j).Elevation;
					Assert.That(actual, Is.EqualTo(expectedZ[i, j]).Within(0.01f),
								$"Elevation a {i} {j}");
				}
			}
		}
	}
}