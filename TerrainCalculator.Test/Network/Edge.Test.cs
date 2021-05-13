using System;
using System.Collections.Generic;
using NUnit.Framework;
using TerrainCalculator.Network;
using TerrainCalculator.UnityUI;
using UnityEngine;

namespace TerrainCalculator.Test.Network
{
	[TestFixture]
	public class EdgeTest
	{

		[SetUp]
		protected void SetUp()
		{
		}

		[Test]
		public void TestBuildMesh()
		{
			WaterNetwork net = new WaterNetwork();
			River river = net.NewRiver();
			Node a = net.NewNode();
			Node b = net.NewNode();

			a.ShoreWidth.SetFixed(10);
			b.ShoreWidth.SetFixed(20);
			a.RiverWidth.SetFixed(20);
			b.RiverWidth.SetFixed(30);
			a.ShoreDepth.SetFixed(10);
			b.ShoreDepth.SetFixed(20);
			a.Elevation.SetFixed(40);
			b.Elevation.SetFixed(50);

			a.Pos.Set(0, 0);
			b.Pos.Set(100, 0);

			Vector2 aGrad = new Vector2(0, 200);
			Vector2 bGrad = new Vector2(0, -200);

			Edge edge = new Edge(a, b, river, aGrad, bGrad);

			Vector3[] vertices = edge.BuildVertices();

			Assert.That(vertices.Length, Is.EqualTo(93));

			int[] vertIndices = { 0, 1, 2,  45, 46, 47, 90, 91, 92};
			float[] vertX = { -15, 0, 15, 50, 50, 50, 125, 100, 75 };
			float[] vertY = { 40, 50, 40, 45, 60, 45, 50, 70, 50 };
			float midZ = 50f;
			float[] vertZ = { 0, 0, 0, midZ + 20, midZ, midZ - 20, 0, 0, 0 };

			float delta = 0.001f;
			for (int i = 0; i < vertIndices.Length; i++)
            {
				Vector3 vertex = vertices[vertIndices[i]];
				Assert.That(vertex.x, Is.EqualTo(vertX[i]).Within(delta),
							$"vertex {vertIndices[i]} x value");
				Assert.That(vertex.y, Is.EqualTo(vertY[i]).Within(delta),
					        $"vertex {vertIndices[i]} y value");
				Assert.That(vertex.z, Is.EqualTo(vertZ[i]).Within(delta),
							$"vertex {vertIndices[i]} z value");
			}

			int[] triangles = edge.BuildTriangles();

			int[,] edgeCounts = new int[vertices.Length, vertices.Length];
			for (int i = 0; i < triangles.Length; i+=3)
            {
				edgeCounts[triangles[i + 0], triangles[i + 1]]++;
				edgeCounts[triangles[i + 1], triangles[i + 2]]++;
				edgeCounts[triangles[i + 2], triangles[i + 0]]++;

				Vector3 p1 = vertices[triangles[i + 0]];
				Vector3 p2 = vertices[triangles[i + 1]];
				Vector3 p3 = vertices[triangles[i + 2]];

				var normal = Vector3.Cross(p2 - p1, p3 - p1);
				Assert.Greater(normal.y, 0, $"triangle at {i} points up");
			}


			Assert.AreEqual(0, edgeCounts[0, 1], "Edge 0 1");
			Assert.AreEqual(1, edgeCounts[1, 0], "Edge 1 0");
			Assert.AreEqual(0, edgeCounts[1, 2], "Edge 1 2");
			Assert.AreEqual(1, edgeCounts[2, 1], "Edge 2 1");
			Assert.AreEqual(1, edgeCounts[1, 4], "Edge 1 4");
			Assert.AreEqual(1, edgeCounts[4, 1], "Edge 4 1");
			Assert.AreEqual(1, edgeCounts[4, 5], "Edge 4 5");
			Assert.AreEqual(1, edgeCounts[5, 4], "Edge 5 4");

			Assert.AreEqual(0, edgeCounts[92, 91], "Edge 92 91");
			Assert.AreEqual(1, edgeCounts[91, 92], "Edge 91 92");
		}

		private int _toEdge(int a, int b, int count) => a * count + b;
	}
}