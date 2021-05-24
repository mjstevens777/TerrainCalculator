using System;
using System.Collections.Generic;
using NUnit.Framework;
using TerrainCalculator.Grid;
using UnityEngine;

namespace TerrainCalculator.Test.Grid
{
	[TestFixture]
	public class TerrainSetterTest
	{

		[SetUp]
		protected void SetUp()
		{
		}

		[Test]
		public void TestBasic()
		{
			TerrainSetter ts = new TerrainSetter(11, 3);
			int numPublished;
			numPublished = ts.PublishBlocks(null, 1);
			Assert.That(numPublished, Is.EqualTo(0), "Nothing to publish");
			ts.SetAll(0);
			numPublished = ts.PublishBlocks(null, 1);
			Assert.That(numPublished, Is.EqualTo(1), "Publish all");
			numPublished = ts.PublishBlocks(null, 3);
			Assert.That(numPublished, Is.EqualTo(3), "Publish all again");
			numPublished = ts.PublishBlocks(null, 16);
			Assert.That(numPublished, Is.EqualTo(12), "Publish remaining");
			numPublished = ts.PublishBlocks(null);
			Assert.That(numPublished, Is.EqualTo(0), "Done publishing");

			ts.Reset();
			numPublished = ts.PublishBlocks(null, 1);
			Assert.That(numPublished, Is.EqualTo(0), "Reset");
			ts.SetAll(0);
			numPublished = ts.PublishBlocks(null, 16);
			Assert.That(numPublished, Is.EqualTo(16), "Publish all after reset");
			numPublished = ts.PublishBlocks(null);
			Assert.That(numPublished, Is.EqualTo(0), "Done publishing after reset");
		}

		[Test]
		public void TestBounds()
		{
			TerrainSetter ts = new TerrainSetter(11, 3);
			for (int i = 6; i < 9; i++)
            {
				for (int j = 3; j < 6; j++)
                {
					ts.Set(i, j, 0);
                }
            }

			foreach(var block in ts.ReadyBlocks())
            {
				Assert.That(block.MinI, Is.EqualTo(6), "MinI");
				Assert.That(block.MaxI, Is.EqualTo(9), "MaxI");
				Assert.That(block.MinJ, Is.EqualTo(3), "MinJ");
				Assert.That(block.MaxJ, Is.EqualTo(6), "MaxJ");
			}

			int numPublished;
			numPublished = ts.PublishBlocks(null);
			Assert.That(numPublished, Is.EqualTo(1), "Publish first block");
			numPublished = ts.PublishBlocks(null);
			Assert.That(numPublished, Is.EqualTo(0), "After first block");


			for (int i = 0; i < 3; i++)
			{
				for (int j = 9; j < 11; j++)
				{
					ts.Set(i, j, 0);
				}
			}

			foreach (var block in ts.ReadyBlocks())
			{
				Assert.That(block.MinI, Is.EqualTo(0), "MinI");
				Assert.That(block.MaxI, Is.EqualTo(3), "MaxI");
				Assert.That(block.MinJ, Is.EqualTo(9), "MinJ");
				Assert.That(block.MaxJ, Is.EqualTo(11), "MaxJ");
			}

			numPublished = ts.PublishBlocks(null);
			Assert.That(numPublished, Is.EqualTo(1), "Publish second block");
			numPublished = ts.PublishBlocks(null);
			Assert.That(numPublished, Is.EqualTo(0), "After second block");
		}
	}
}