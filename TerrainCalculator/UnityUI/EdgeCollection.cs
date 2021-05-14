using System;
using System.Collections.Generic;
using TerrainCalculator.Network;
using UnityEngine;

namespace TerrainCalculator.UnityUI
{
    public class EdgeCollection : MonoBehaviour
    {
        private State _state;
        private Material _edgeMaterial;

        public void Start()
        {
            Debug.Log("NetworkRenderer start");
            _state = gameObject.GetComponent<State>();

            _edgeMaterial = new Material(Shader.Find("Custom/Props/Prop/Default"));
            _edgeMaterial.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }

        public void LateUpdate()
        {
            _drawEdges();
        }

        private void _drawEdges()
        {
            List<Edge> edges = new List<Edge>(_state.Net.Edges);
            for (int i = 0; i < edges.Count; i++)
            {
                _drawEdge(edges[i]);
            }
        }

        private void _drawEdge(Edge edge)
        {
            Vector3 scale = new Vector3(1, 1, 1);
            var mesh = _buildMesh(edge);
            Graphics.DrawMesh(mesh, Matrix4x4.identity, _edgeMaterial, 0);
        }

        private static Mesh _buildMesh(Edge edge)
        {
            Mesh mesh = new Mesh();
            mesh.vertices = edge.BuildVertices();
            mesh.triangles = edge.BuildTriangles();
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}
