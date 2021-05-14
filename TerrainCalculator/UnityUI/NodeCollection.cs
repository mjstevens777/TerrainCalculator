using System;
using System.Collections.Generic;
using TerrainCalculator.Network;
using UnityEngine;

namespace TerrainCalculator.UnityUI
{
    public class NodeCollection : MonoBehaviour
    {
        private State _state;
        private List<GameObject> _nodePrimitives;
        private Mesh _nodeMesh;
        private Material _nodeBaseMaterial;
        private Material _nodeHighlightMaterial;
        private Material _edgeMaterial;
        Node _hoveredNode;

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

            Graphics.DrawMesh(_nodeMesh, _getMatrix(node), material, 0); //draw nodes
        }

        private Matrix4x4 _getMatrix(Node node)
        {
            Vector3 position = new Vector3(
                node.Pos.x,
                node.Elevation.Value,
                node.Pos.y);

            float width = node.RiverWidth.Value + node.ShoreWidth.Value;
            float depth = node.ShoreDepth.Value;
            Vector3 scale = new Vector3(width, depth * 2, width);
            Matrix4x4 matrix = Matrix4x4.TRS(position, Quaternion.identity, scale);
            return matrix;
        }
    }
}
