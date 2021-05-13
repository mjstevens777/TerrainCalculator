using System;
using System.Collections.Generic;
using ColossalFramework;
using ColossalFramework.Math;
using TerrainCalculator.Network;
using UnityEngine;

namespace TerrainCalculator.UnityUI
{
    public class NetworkRenderer : MonoBehaviour
    {
        private State _state;
        private List<GameObject> _nodePrimitives;
        private Mesh _nodeMesh;
        private Material _nodeBaseMaterial;
        private Material _nodeHighlightMaterial;
        private Material _edgeMaterial;

        public void Start()
        {
            Debug.Log("NetworkRenderer start");
            _state = gameObject.GetComponent<State>();

            GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _nodeMesh = primitive.GetComponent<MeshFilter>().mesh;

            _nodeBaseMaterial = new Material(Shader.Find("Custom/Props/Prop/Default"));
            _nodeBaseMaterial.color = new Color(0, 0, 1, 1);

            _nodeHighlightMaterial = new Material(Shader.Find("Custom/Props/Prop/Default"));
            _nodeHighlightMaterial.color = new Color(1, 1, 0, 1);

            _edgeMaterial = new Material(Shader.Find("Custom/Props/Prop/Default"));
            _edgeMaterial.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }

        public void LateUpdate()
        {
            _drawNodes();
            _drawEdges();
        }

        private void _drawNodes()
        {
            List<Node> nodes = new List<Node>(_state.Net.Nodes);
            for (int i = 0; i < nodes.Count; i++)
            {
                _drawNode(nodes[i]);
            }
        }

        private void _drawNode(Node node)
        {
            Material material = _nodeBaseMaterial;
            if (node == _state.ActiveNode)
            {
                material = _nodeHighlightMaterial;
            }
            Vector3 position = new Vector3(
                node.Pos.x,
                node.Elevation.Value,
                node.Pos.y);

            float width = node.RiverWidth.Value + node.ShoreWidth.Value;
            float depth = node.ShoreDepth.Value;
            Vector3 scale = new Vector3(width, depth * 2, width);
            Matrix4x4 matrix = Matrix4x4.TRS(position, Quaternion.identity, scale);
            Graphics.DrawMesh(_nodeMesh, matrix, material, 0); //draw nodes
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
            for (int i = 0; i < 3; i++)
            {
                int idx = mesh.triangles[i];
                Vector3 pos = mesh.vertices[idx];
                Debug.Log($"node {idx} = x {pos.x} y {pos.y} z {pos.z}");
            }

            Graphics.DrawMesh(mesh, Matrix4x4.identity, _edgeMaterial, 0);
        }

        private static Mesh _buildMesh(Edge edge)
        {
            Mesh mesh = new Mesh();
            mesh.vertices = edge.BuildVertices();
            mesh.triangles = edge.BuildTriangles();
            return mesh;
        }
    }
}
