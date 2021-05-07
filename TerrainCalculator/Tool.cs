using System.Collections.Generic;
using ICities;
using UnityEngine;
using ColossalFramework;

namespace TerrainCalculator
{
	public static class Tool
	{
		static bool carve = false;
		public static bool clearAfterCarve = true;
		public static bool loadMap = false;
		public static bool loadGame = false;
		public static bool loadMapO = true;
		public static bool loadGameO = true;
		public static bool updateHeight = false;
		public static ITerrain terrain;
		public static int res;
		public static ushort[] heights;
		public static List<RiverNode> nodes = new List<RiverNode>();
		public static List<Vector4> updateArea = new List<Vector4>();
		static Vector3[] tri = new Vector3[3];

		public static void UpdateHeight()
		{
			if (nodes.Count > 0 && !carve)
			{
				foreach (RiverNode n in nodes)
				{
					n.position.y = terrain.SampleTerrainHeight(n.position.x, n.position.z);
					n.scale.y = Mathf.Clamp(n.scale.y, 1f, n.position.y);
				}
				foreach (RiverNode n in nodes)
				{
					n.UpdateNode();
				}
			}
		}

		public static void InterpolateH()
		{
			float totalLength = 0;
			foreach (RiverNode n in nodes)
			{
				totalLength += n.length;
			}
			float curLength = 0;
			for (int n = 0; n < nodes.Count - 1; n++)
			{
				float t = curLength / totalLength;
				float start = nodes[0].position.y;
				float end = nodes[nodes.Count - 1].position.y;
				float result = Mathf.Lerp(start, end, t);
				nodes[n].position.y = result;
				curLength += nodes[n].length;
			}
			foreach (RiverNode n in nodes)
			{
				n.UpdateNode();
			}
		}

		public static void InterpolateW()
		{
			float totalLength = 0;
			foreach (RiverNode n in nodes)
			{
				totalLength += n.length;
			}
			float curLength = 0;
			for (int n = 0; n < nodes.Count - 1; n++)
			{
				float t = curLength / totalLength;
				float start = nodes[0].scale.x;
				float end = nodes[nodes.Count - 1].scale.x;
				float result = Mathf.Lerp(start, end, t);
				nodes[n].scale.x = result;
				nodes[n].scale.z = result;
				curLength += nodes[n].length;
			}
			foreach (RiverNode n in nodes)
			{
				n.UpdateNode();
			}
		}

		public static void InterpolateD()
		{
			float totalLength = 0;
			foreach (RiverNode n in nodes)
			{
				totalLength += n.length;
			}
			float curLength = 0;
			for (int n = 0; n < nodes.Count - 1; n++)
			{
				float t = curLength / totalLength;
				float start = nodes[0].scale.y;
				float end = nodes[nodes.Count - 1].scale.y;
				float result = Mathf.Lerp(start, end, t);
				nodes[n].scale.y = result;
				curLength += nodes[n].length;
			}
			foreach (RiverNode n in nodes)
			{
				n.UpdateNode();
			}
		}

		public static void TryRiver()
		{
			int x, z;
			Vector3 lastPos = nodes[0].position;
			Vector3 scale = nodes[0].scale;
			Vector2[,] slopes = Slopes();
			Vector2 velocity = Vector2.zero;
			int maxStep = 10000;
			for (int s = 0; s < maxStep; s++)
			{
				if (lastPos.x < -8640f || lastPos.x > 8640f || lastPos.z < -8640f || lastPos.z > 8640f)
				{
					return;
				}
				terrain.PositionToHeightMapCoord(lastPos.x, lastPos.z, out x, out z);
				lastPos.y = terrain.RawToHeight(heights[x + z * res]);
				for (int n = 0; n < Tool.nodes.Count - 1; n++)
				{
					float dist = (lastPos - Tool.nodes[n].position).sqrMagnitude;
					if (dist * 2 < Tool.nodes[n].scale.x * Tool.nodes[n].scale.x)
					{
						return;
					}
				}
				if ((lastPos - nodes[nodes.Count - 1].position).magnitude > scale.x * 3f)
				{
					scale.x = Mathf.Clamp(scale.x + 2f, 16, 1024f);
					scale.y = Mathf.Clamp(scale.y, 0, lastPos.y);
					scale.z = Mathf.Clamp(scale.z + 2f, 16, 1024f);
					nodes.Add(new RiverNode(lastPos, scale));
				}
				if (Singleton<TerrainManager>.instance.WaterLevel(new Vector2(lastPos.x, lastPos.z)) > nodes[0].scale.y * 0.666f)
				{
					nodes[nodes.Count - 1].SetPosition(lastPos);
					nodes[nodes.Count - 1].scale.y = 0f;
					nodes[nodes.Count - 1].SetScale(nodes[nodes.Count - 1].scale);
					return;
				}
				Vector2 slope = slopes[x, z];
				velocity += slope * 2f;
				Vector3 addVel = velocity.normalized * 16f; //i know but 							
				lastPos = lastPos + new Vector3(addVel.x, 0, addVel.y);
				if (slope.sqrMagnitude > 2.56f)
				{ // 1:10
					velocity *= 0.85f;
				}
				else
				{
					velocity += new Vector2(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(-0.5f, 0.5f)); //good luck
				}
			}
		}

		public static void CarveRiver()
		{
			carve = true;
			updateArea.Clear();
			foreach (RiverNode n in nodes)
			{
				CarveNode(n);
			}
			if (clearAfterCarve)
			{
				nodes.Clear();
			}
			foreach (Vector4 v in updateArea)
			{
				TerrainModify.UpdateArea(v.x, v.y, v.z, v.w, true, false, false);
			}
			carve = false;
		}

		static void CarveNode(RiverNode n)
		{
			int row = n.vertices.Count / 6;
			for (int r = 0; r < row; r++)
			{
				int index = r * 6;
				float height = n.heights[r];
				tri[0] = n.vertices[index + 1]; //single point of triangle needed for depth calculation
				tri[1] = n.vertices[index + 0];
				tri[2] = n.vertices[index + 2];
				CarveTriangle(tri, height);
				tri[0] = n.vertices[index + 4]; //single point of triangle needed for depth calculation
				tri[1] = n.vertices[index + 3];
				tri[2] = n.vertices[index + 5];
				CarveTriangle(tri, height);
			}
			updateArea.Add(new Vector4(n.lx, n.lz, n.hx, n.hz));
		}

		static void CarveTriangle(Vector3[] p, float off)
		{
			float lx = float.MaxValue;
			float hx = float.MinValue;
			float lz = float.MaxValue;
			float hz = float.MinValue;
			foreach (Vector3 v in p)
			{ //is there a beter way for this?
				if (v.x < lx)
				{
					lx = v.x;
				}
				if (v.x > hx)
				{
					hx = v.x;
				}
				if (v.z < lz)
				{
					lz = v.z;
				}
				if (v.z > hz)
				{
					hz = v.z;
				}
			}
			int lxi, lzi, hxi, hzi;
			terrain.PositionToHeightMapCoord(lx, lz, out lxi, out lzi);
			terrain.PositionToHeightMapCoord(hx, hz, out hxi, out hzi);
			for (int z = lzi; z <= hzi; z++)
			{
				for (int x = lxi; x <= hxi; x++)
				{
					float px, pz;
					terrain.HeightMapCoordToPosition(x, z, out px, out pz);
					float heightFactor = IsInsideTriangle(px, pz, p);
					float height = Mathf.Clamp(p[1].y - (off * heightFactor), 0f, 1024f);
					if (heightFactor > 0)
					{
						heights[x + (z * res)] = terrain.HeightToRaw(height);
					}
				}
			}
		}

		static float IsInsideTriangle(float x, float z, Vector3[] points)
		{
			float w1 = ((points[0].x * (points[2].z - points[0].z)) + ((z - points[0].z) * (points[2].x - points[0].x)) - (x * (points[2].z - points[0].z))) / (((points[1].z - points[0].z) * (points[2].x - points[0].x)) - ((points[1].x - points[0].x) * (points[2].z - points[0].z)));
			float w2 = (z - points[0].z - (w1 * (points[1].z - points[0].z))) / (points[2].z - points[0].z);
			if (w1 > 0 && w2 > 0 && (w1 + w2) < 1f)
			{
				return Mathf.Sin((w1 + w2) * Mathf.PI);
			}
			return -1f;
		}

		static Vector2[,] Slopes()
		{
			Vector2[,] slopes = new Vector2[res, res];
			for (int z = 0; z < res; z++)
			{
				for (int x = 0; x < res; x++)
				{
					float h = terrain.RawToHeight(heights[x + z * res]);
					float xn = 0;
					float zn = 0;
					float ph;
					if (x - 1 >= 0)
					{
						ph = terrain.RawToHeight(heights[(x - 1) + z * res]);
						xn += ph - h;
					}
					else
					{
						ph = terrain.RawToHeight(heights[(x + 1) + z * res]);
						xn += (ph - h) * 0.5f;
					}
					if (x + 1 < res)
					{
						ph = terrain.RawToHeight(heights[(x + 1) + z * res]);
						xn -= ph - h;
					}
					else
					{
						ph = terrain.RawToHeight(heights[(x - 1) + z * res]);
						xn -= (ph - h) * 0.5f;
					}
					if (z - 1 >= 0)
					{
						ph = terrain.RawToHeight(heights[x + (z - 1) * res]);
						zn += ph - h;
					}
					else
					{
						ph = terrain.RawToHeight(heights[x + (z + 1) * res]);
						zn += (ph - h) * 0.5f;
					}
					if (z + 1 < res)
					{
						ph = terrain.RawToHeight(heights[x + (z + 1) * res]);
						zn -= ph - h;
					}
					else
					{
						ph = terrain.RawToHeight(heights[x + (z - 1) * res]);
						zn -= (ph - h) * 0.5f;
					}
					slopes[x, z] = new Vector2(xn, zn); //cheap and effective				
				}
			}
			return slopes;
		}

		public static Vector3 Bezier(Vector3 p0, Vector3 p1, Vector3 p2, float t) //it's called be'zsjeeeee; source 'Coding Math' youtube
		{
			Vector3 result = Vector3.zero;
			result.x = Mathf.Pow(1 - t, 2) * p0.x + (1 - t) * 2 * t * p1.x + t * t * p2.x;
			result.y = Mathf.Pow(1 - t, 2) * p0.y + (1 - t) * 2 * t * p1.y + t * t * p2.y;
			result.z = Mathf.Pow(1 - t, 2) * p0.z + (1 - t) * 2 * t * p1.z + t * t * p2.z;
			return result;
		}
	}
}
