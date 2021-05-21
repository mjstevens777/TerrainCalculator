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


			ZValue defaultZ = new ZValue(landSlope: 0.5f, riverSlope: 0, elevation: 500);

			ZValue[,] grid = new ZValue[5, 5];
			for (int i = 0; i < 5; i++)
            {
				for (int j = 0; j < 5; j++)
                {
					grid[i, j] = defaultZ;
                }
            }
			var algorithm = new ProgressiveDijkstra<ZValue>(grid);

			for (int i = 0; i < 5; i++) grid[i, 2] = new ZValue(0.5f, 1, elevation: i + 1);
			grid[2, 2] = new ZValue(0.5f, 1, elevation: 10);  // Make an outlier above the rest
			algorithm.Lock(2, 2);
			for (int i = 0; i < 5; i++) algorithm.Lock(i, 2);

			bool done = algorithm.IterateMulti(100);
			Assert.IsFalse(done, "Loop should not terminate yet");
			done = algorithm.IterateMulti(100);
			Assert.IsTrue(done, "Loop should terminate");


			float[,] expectedZ = new float[,]
			{
				{4f, 2.5f, 1f, 2.5f, 4f },
				{4.62f, 3.12f, 2f, 3.12f, 4.62f },
				{5.24f, 4.12f, 10f, 4.12f, 5.24f },
				{6.24f, 5.5f, 4f, 5.5f, 6.24f },
				{7.62f, 6.12f, 5f, 6.12f, 7.62f },
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

		[Test]
		public void TestBlocks()
		{
			


			ZValue defaultZ = new ZValue(landSlope: 0.1f, riverSlope: 0, elevation: 500);

			ZValue[,] grid = new ZValue[1081, 1081];
			for (int i = 0; i < 1081; i++)
			{
				for (int j = 0; j < 1081; j++)
				{
					grid[i, j] = defaultZ;
				}
			}

			var algorithm = new ProgressiveDijkstra<ZValue>(grid, neighborRadius: 1);

			for (int i = 0; i < 1081; i+=900)
            {
				for (int j = 0; j < 1081; j += 900)
                {
					grid[i, j] = new ZValue(0.1f, 0, elevation: 0);
					algorithm.Lock(i, j);
				}
			}


			bool isDone = false;
			int[,] blockSet = new int[1081, 1081];
			for (int iteration = 0; iteration < 500; iteration++)
            {
				isDone = algorithm.IterateMulti(100000);
				int minI, minJ, maxI, maxJ;
				
				while (algorithm.GetBlockReady(out minI, out minJ, out maxI, out maxJ))
                {
					Console.WriteLine($"Block ready on step {iteration} {minI} {minJ} {maxI} {maxJ}");
					for (int i = minI; i < maxI; i++)
                    {
						for (int j = minJ; j < maxJ; j++)
                        {
							blockSet[i, j]++;
                        }
                    }
				}

				if (isDone)
                {
					Console.WriteLine($"Finished on step {iteration}");
					break;
                }
            }
			Assert.IsTrue(isDone, "Loop should terminate");

			for (int i = 0; i < 1081; i++)
			{
				for (int j = 0; j < 1081; j++)
				{
					Assert.That(blockSet[i, j], Is.EqualTo(1), $"Pixel {i} {j} should be part of one block");
				}
			}
		}
	}
}