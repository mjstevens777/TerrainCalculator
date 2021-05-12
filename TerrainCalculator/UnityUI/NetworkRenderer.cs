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

            _nodeBaseMaterial = new Material(Shader.Find("Sprites/Default"));
            _nodeBaseMaterial.color = new Color(0, 0, 1, 0.75f);

            _nodeHighlightMaterial = new Material(Shader.Find("Sprites/Default"));
            _nodeHighlightMaterial.color = new Color(1, 1, 0, 0.75f);

            _edgeMaterial = new Material(Shader.Find("Sprites/Default"));
            _edgeMaterial.color = new Color(0.5f, 0.5f, 0.5f, 0.75f);
        }

        public void LateUpdate()
        {
            List<Node> nodes = new List<Node>(_state.Net.Nodes);

            for (int i = 0; i < nodes.Count; i++)
            {
                Node node = nodes[i];
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
                Vector3 scale = new Vector3(width, depth, width);
                Matrix4x4 matrix = Matrix4x4.TRS(position, Quaternion.identity, scale);
                Graphics.DrawMesh(_nodeMesh, matrix, material, 0); //draw nodes
            }

            List<Edge> edges = new List<Edge>(_state.Net.Edges);

            for (int i = 0; i < edges.Count; i++)
            {
                Edge edge = edges[i];
                float startZ = edge.Source.Elevation.Value;
                float endZ = edge.Target.Elevation.Value;
                Vector3 scale = new Vector3(1, 1, 1);
                for (int j = 0; j < edge.InterpPoints.Count; j++)
                {
                    // TODO: Distance-based interpolation
                    float t = (float)j / (float)(edge.InterpPoints.Count - 1);
                    float z = (1 - t) * startZ + t * endZ;
                    Vector2 pos2 = edge.InterpPoints[j];
                    Vector3 position = new Vector3(pos2.x, z, pos2.y);
                    Matrix4x4 matrix = Matrix4x4.TRS(position, Quaternion.identity, scale);
                    Graphics.DrawMesh(_nodeMesh, matrix, _edgeMaterial, 0); //draw edge nodes
                }
            }
        }
    }
}
