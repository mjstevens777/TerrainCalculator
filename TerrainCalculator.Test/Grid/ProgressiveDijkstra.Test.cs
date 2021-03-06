using System;
using System.Collections.Generic;
using NUnit.Framework;
using TerrainCalculator.Grid;
using TerrainCalculator.UnityUI;
using UnityEngine;

namespace TerrainCalculator.Test.Grid
{
	[TestFixture]
	public class ProgressiveDijkstraTest
	{

		[SetUp]
		protected void SetUp()
		{
		}

		[Test]
		public void TestZValueSimple()
		{
			GridValue defaultZ = new GridValue(landSlope: 0.5f);

			GridValue[,] grid = new GridValue[5, 5];
			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j < 5; j++)
				{
					grid[i, j] = defaultZ;
				}
			}
			var algorithm = new ProgressiveDijkstra<GridValue>(grid);

			for (int i = 0; i < 5; i++) grid[i, 2] = new GridValue(0.5f, 1, 1, 1, elevation: i + 1, shoreDistance: 0);
			grid[2, 2] = new GridValue(0.5f, 1, 1, 1, elevation: 10, shoreDistance: 0);  // Make an outlier above the rest
			algorithm.Lock(2, 2);
			for (int i = 0; i < 5; i++) algorithm.Lock(i, 2);

			bool done = algorithm.IterateMulti(50);
			Assert.IsFalse(done, "Loop should not terminate yet");
			done = algorithm.IterateMulti(50);
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
		public void TestFullSize()
		{
			GridValue defaultZ = new GridValue(landSlope: 0.1f);

			GridValue[,] grid = new GridValue[1081, 1081];
			for (int i = 0; i < 1081; i++)
			{
				for (int j = 0; j < 1081; j++)
				{
					grid[i, j] = defaultZ;
				}
			}

			var algorithm = new ProgressiveDijkstra<GridValue>(grid, neighborRadius: 1);

			// Set a couple of sparse points
			for (int i = 0; i < 1081; i += 900)
			{
				for (int j = 0; j < 1081; j += 900)
				{
					grid[i, j] = new GridValue(0.1f, 0, 1, 1, elevation: 0, shoreDistance: 0);
					algorithm.Lock(i, j);
				}
			}

			bool isDone = false;
			int[,] valueSet = new int[1081, 1081];
			for (int iteration = 0; iteration < 500; iteration++)
			{
				isDone = algorithm.IterateMulti(100000);

				while (true)
				{
					int i, j;
					bool found = algorithm.GetReady(out i, out j, out _);
					if (!found) break;
					valueSet[i, j]++;
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
					Assert.That(valueSet[i, j], Is.EqualTo(1), $"Pixel {i} {j} should be part of one block");
				}
			}
		}

		[Test]
		public void TestShoreDistance()
		{
			GridValue defaultValue = new GridValue(landSlope: 0.1f);

			GridValue[,] grid = new GridValue[11, 1];
			for (int i = 0; i < 11; i++)
			{
				grid[i, 0] = defaultValue;
			}

			var algorithm = new ProgressiveDijkstra<GridValue>(grid, neighborRadius: 1);
			grid[5, 0] = new GridValue(
				landSlope: 0.1f, riverSlope: 0, riverWidth: 3, shoreWidth: 4, shoreDepth: 0.1f, elevation: 1, shoreDistance: 0);
			algorithm.Lock(5, 0);

			bool isDone = algorithm.IterateMulti(1000);
			Assert.IsTrue(isDone, "Loop should terminate");


			// Breakpoints at 1.5 and 3.5 from center
			float[] expectedDist = new float[]
				{ 2, 2, 1.75f, 1.25f, 0.6666f, 0, 0.66666f, 1.25f, 1.75f, 2, 2 };
			float[] expectedZ = new float[]
				{ 1.5f, 1.4f, 1.3f, 1.2f, 1.1f, 1, 1.1f, 1.2f, 1.3f, 1.4f, 1.5f };

			for (int i = 0; i < 11; i++)
			{
				GridValue value = algorithm.Get(i, 0);
				Assert.That(value.ShoreDistance, Is.EqualTo(expectedDist[i]).Within(0.01f), $"Distance at {i}");
				Assert.That(value.RiverWidth, Is.EqualTo(3).Within(0.01f), $"RiverWidth at {i}");
				Assert.That(value.ShoreWidth, Is.EqualTo(4).Within(0.01f), $"ShoreWidth at {i}");
				Assert.That(value.Elevation, Is.EqualTo(expectedZ[i]).Within(0.01f), $"Elevation at {i}");
				Console.WriteLine($"{i} {value.Elevation} {value.FinalElevation}");
			}
		}
	}

	[TestFixture]
	public class GridValueTest
	{

		[Test]
		public void TestFinalZ()
		{
			assertFinalZ(0, 10, 4, 6);
			assertFinalZ(0.5f, 10, 4, 6);
			assertFinalZ(1, 10, 4, 6);
            assertFinalZ(1.25f, 10, 4, 6.625f);
            assertFinalZ(1.5f, 10, 4, 8);
            assertFinalZ(1.75f, 10, 4, 9.375f);
            assertFinalZ(2, 10, 4, 10);
			assertFinalZ(3, 10, 4, 10);
		}

		private void assertFinalZ(float shoreDistance, float elevation, float shoreDepth,
								  float expected, float delta = 0.01f)
        {
			GridValue value = new GridValue(
				0, 0, 0, 0,
				shoreDepth: shoreDepth, elevation: elevation,
				shoreDistance: shoreDistance);
			Assert.That(value.FinalElevation, Is.EqualTo(expected).Within(delta), $"d={shoreDistance}");
		}
	}
}