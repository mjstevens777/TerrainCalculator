using System.Collections.Generic;
using UnityEngine;

namespace TerrainCalculator
{
	public class RiverNode
	{
		public Vector3 position;
		public Vector3 scale;
		public RiverNode parent;
		public RiverNode child;
		public List<Vector3> vertices = new List<Vector3>();
		public float lx, lz, hx, hz;
		public List<float> heights = new List<float>();
		public float length = 0f;
		public Mesh mesh;

		public RiverNode(Vector3 _position, Vector3 _scale)
		{
			position = _position;
			scale = _scale;
			if (Tool.nodes.Count > 0)
			{
				parent = Tool.nodes[Tool.nodes.Count - 1];
				parent.child = this;
				UpdateNeighbours();
			}
			UpdateNode();
		}

		public void SetPosition(Vector3 _position)
		{
			position = _position;
			UpdateNode();
			UpdateNeighbours();
		}

		public void SetScale(Vector3 _scale)
		{
			scale = _scale;
			UpdateNode();
			UpdateNeighbours();
		}

		public void RemoveNode()
		{
			if (parent != null)
			{
				parent.child = child;
				parent.UpdateNode();
			}
			if (child != null)
			{
				child.parent = parent;
				child.UpdateNode();
			}
			Tool.nodes.Remove(this);
			mesh = null;
			parent = null;
			child = null;
			//this = Sparta;			
		}

		public void UpdateNode()
		{
			lx = float.MaxValue;
			hx = float.MinValue;
			lz = float.MaxValue;
			hz = float.MinValue;
			heights.Clear();
			if (parent == null && child == null)
			{
				mesh = null;
				return;
			}
			if (parent != null && child != null)
			{
				CreateMesh(CreateBend());
				return;
			}
			CreateMesh(CreateEnd());
		}

		void CreateMesh(List<Vector3> dots) //"CreateMess"
		{
			int rows = (dots.Count / 2) - 1;
			vertices.Clear();
			for (int t = 0; t < rows; t++)
			{
				int vIndex = t * 2;
				vertices.Add(dots[vIndex + 0]); //r
				vertices.Add(dots[vIndex + 1]); //l
				vertices.Add(dots[vIndex + 2]); //r
				vertices.Add(dots[vIndex + 3]); //l
				vertices.Add(dots[vIndex + 2]); //r
				vertices.Add(dots[vIndex + 1]); //l
			}
			int[] triangles = new int[vertices.Count];
			Vector3[] normals = new Vector3[vertices.Count];
			for (int i = 0; i < vertices.Count; i++)
			{
				triangles[i] = i;
				normals[i] = Vector3.up; //you call this normal?
			}
			mesh = new Mesh();
			mesh.vertices = vertices.ToArray();
			mesh.triangles = triangles;
			mesh.normals = normals;
			mesh.RecalculateBounds();
		}

		List<Vector3> CreateEnd()
		{
			List<Vector3> dots = new List<Vector3>();
			Vector3 startP;
			Vector3 endP;
			Vector3 startS;
			Vector3 endS;
			if (parent == null)
			{
				startP = position;
				endP = (position + child.position) * 0.5f;
				startS = scale;
				endS = (scale + child.scale) * 0.5f;
			}
			else
			{
				startP = (position + parent.position) * 0.5f;
				endP = position;
				startS = (scale + parent.scale) * 0.5f;
				endS = scale;
			}
			Vector3 dir = endP - startP;
			float dist = Mathf.Clamp(dir.magnitude * (2f / scale.x), 1f, 16384f);
			int step = Mathf.RoundToInt(dist);
			Vector3 addP = (dir) / (float)step;
			Vector3 addS = (endS - startS) / (float)step;
			dir = dir.normalized;
			Vector3 offset = Vector3.Cross(Vector3.up, dir);
			length = 0;
			Vector3 lastpos = startP; //
			for (int i = 0; i <= step; i++)
			{
				Vector3 pos = startP + (addP * i);
				Vector3 sca = startS + (addS * i);
				length += (pos - lastpos).magnitude;
				lastpos = pos;
				dots.Add(NBound(pos + (offset * sca.x * 0.5f)));
				dots.Add(NBound(pos - (offset * sca.x * 0.5f)));
				heights.Add(sca.y);
			}
			return dots;
		}

		List<Vector3> CreateBend()
		{
			List<Vector3> dots = new List<Vector3>();
			Vector3 startP = (parent.position + position) * 0.5f;
			Vector3 midP = position;
			Vector3 endP = (child.position + position) * 0.5f;
			Vector3 startS = (parent.scale + scale) * 0.5f;
			Vector3 midS = scale;
			Vector3 endS = (child.scale + scale) * 0.5f;
			Vector3 startD = midP - startP;
			Vector3 endD = endP - midP;
			Quaternion startQ = Quaternion.LookRotation(startD, Vector3.up);
			Quaternion endQ = Quaternion.LookRotation(endD, Vector3.up);
			float dist = Mathf.Clamp((endD.magnitude + startD.magnitude) * (2f / scale.x), 1f, 16384f);
			int step = Mathf.RoundToInt(dist);
			length = 0;
			Vector3 lastpos = startP; //			
			for (int i = 0; i <= step; i++)
			{
				float t = (float)i / (float)step;
				Vector3 pos = Tool.Bezier(startP, midP, endP, t);
				Vector3 sca = Tool.Bezier(startS, midS, endS, t);
				Vector3 dir = Quaternion.Lerp(startQ, endQ, t) * Vector3.forward;
				Vector3 offset = Vector3.Cross(Vector3.up, dir);
				length += (pos - lastpos).magnitude;
				lastpos = pos;
				dots.Add(NBound(pos + (offset * sca.x * 0.5f)));
				dots.Add(NBound(pos - (offset * sca.x * 0.5f)));
				heights.Add(sca.y);
			}
			return dots;
		}

		Vector3 NBound(Vector3 v)
		{
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
			return v;
		}

		void UpdateNeighbours()
		{
			if (parent != null)
			{
				parent.UpdateNode();
			}
			if (child != null)
			{
				child.UpdateNode();
			}
		}
	}
}
