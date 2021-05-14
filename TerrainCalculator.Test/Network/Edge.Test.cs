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

			Assert.That(vertices.Length, Is.EqualTo(124));

			int[] vertIndices = { 0, 1, 2, 3,  60, 61, 62, 63, 120, 121, 122, 123};
			float[] vertX = { -15, 0, 0, 15, 50, 50, 50, 50, 125, 100, 100, 75 };
			float[] vertY = { 40, 50, 50, 40, 45, 60, 60, 45, 50, 70, 70, 50 };
			float midZ = 50f;
			float[] vertZ = { 0, 0, 0, 0, midZ + 20, midZ, midZ, midZ - 20, 0, 0, 0, 0 };

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

			// Check normals
			for (int i = 0; i < triangles.Length; i += 3)
			{
				Vector3 p1 = vertices[triangles[i + 0]];
				Vector3 p2 = vertices[triangles[i + 1]];
				Vector3 p3 = vertices[triangles[i + 2]];

				var normal = Vector3.Cross(p2 - p1, p3 - p1);
				Assert.Greater(normal.y, 0, $"triangle at {i} points up");
			}

			// Construct adjacency matrix
			int[,] edgeCounts = new int[vertices.Length, vertices.Length];
			for (int i = 0; i < triangles.Length; i+=3)
            {
				edgeCounts[triangles[i + 0], triangles[i + 1]]++;
				edgeCounts[triangles[i + 1], triangles[i + 2]]++;
				edgeCounts[triangles[i + 2], triangles[i + 0]]++;
			}

			// Make sure there are no duplicates
			for (int i = 0; i < vertices.Length; i++)
			{
				for (int j = 0; i < vertices.Length; i++)
				{
					Assert.That(edgeCounts[i, j], Is.InRange(0, 1), $"Edge in range {i} {j}");
				}
			}

			// Check edge values
			Action<int, int, int, int> assertEdge = (int p, int q, int fw, int bw) =>
			{
				Assert.AreEqual(fw, edgeCounts[p, q], $"Edge {p} {q}");
				Assert.AreEqual(bw, edgeCounts[q, p], $"Edge {q} {p}");
			};
			// Note: Expect CW because we are in XZ
			Action<int, int> assertCWEdge = (int p, int q) => assertEdge(p, q, 1, 0);
			Action<int, int> assertCCWEdge = (int p, int q) => assertEdge(p, q, 0, 1);
			Action<int, int> assertBothEdge = (int p, int q) => assertEdge(p, q, 1, 1);
			Action<int, int> assertEmptyEdge = (int p, int q) => assertEdge(p, q, 0, 0);
			Action<int, int, int, int> assertDiagonal = (int p, int q, int r, int s) =>
			{
				// p q
				// r s
				if (edgeCounts[r, q] == 1)
				{
					assertEdge(r, q, 1, 1);
					assertEdge(p, s, 0, 0);

				}
				else
				{
					assertEdge(p, s, 1, 1);
					assertEdge(r, q, 0, 0);
				}
			};


            // 120-121 122-123
            //  | / |   | / |
            // 116-117 118-119
            // 
            // 4-56-7
            // |/||/|
            // 0-12-3
            assertCCWEdge(0, 1);
			assertBothEdge(4, 5);
			assertCCWEdge(4, 0);
            assertCCWEdge(1, 5);
			assertDiagonal(0, 1, 4, 5);

			assertCCWEdge(2, 3);
			assertCCWEdge(3, 7);
			assertBothEdge(7, 6);
            assertCCWEdge(6, 2);
            assertDiagonal(2, 3, 6, 7);

			assertBothEdge(116, 117);
			assertCCWEdge(117, 121);
			assertCCWEdge(121, 120);
			assertCCWEdge(120, 116);
			assertDiagonal(116, 117, 120, 121);

			assertBothEdge(118, 119);
			assertCCWEdge(119, 123);
			assertCCWEdge(123, 122);
			assertCCWEdge(122, 118);
			assertDiagonal(118, 119, 122, 123);
		}

		private int _toEdge(int a, int b, int count) => a * count + b;
	}
}