using System;
using System.Collections.Generic;
using TerrainCalculator.Network;
using UnityEngine;

namespace TerrainCalculator.UnityUI
{
    class NodeContainer : MonoBehaviour
    {
        public Node Node;
    }

    public class NodeCollection : MonoBehaviour
    {
        private State _state;
        private List<GameObject> _nodePrimitives;
        private Material _nodeBaseMaterial;
        private Material _nodeHighlightMaterial;

        public void Start()
        {
            Debug.Log("NetworkRenderer start");
            _state = gameObject.GetComponent<State>();

            _nodePrimitives = new List<GameObject>();

            _nodeBaseMaterial = new Material(Shader.Find("Custom/Props/Prop/Default"));
            _nodeBaseMaterial.color = new Color(0, 0, 1, 1);

            _nodeHighlightMaterial = new Material(Shader.Find("Custom/Props/Prop/Default"));
            _nodeHighlightMaterial.color = new Color(1, 1, 0, 1);
        }

        public void Update()
        {
            _syncNodes();
            _checkCollision();
        }

        public void LateUpdate()
        {
            _updatePositions();
        }

        private void _syncNodes()
        {
            List<Node> nodes = new List<Node>(_state.Net.Nodes);

            for (int i = _nodePrimitives.Count; i < nodes.Count; i++)
            {
                Debug.Log($"Creating primitive {i}");
                GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Debug.Log("Adding node container");
                primitive.AddComponent<NodeContainer>();
                Debug.Log("Adding mesh collider");
                primitive.AddComponent<MeshCollider>();
                _nodePrimitives.Add(primitive);
            }

            for (int i = _nodePrimitives.Count - 1; i >= nodes.Count; i--)
            {
                Debug.Log($"Destroying primitive {i}");
                GameObject.Destroy(_nodePrimitives[i]);
                _nodePrimitives.RemoveAt(i);
            }

            for (int i = 0; i < nodes.Count; i++)
            {
                _nodePrimitives[i].GetComponent<NodeContainer>().Node = nodes[i];
            }
        }

        private void _updatePositions()
        {
            for (int i = 0; i < _nodePrimitives.Count; i++)
            {
                _updatePosition(_nodePrimitives[i]);
            }
        }


        private void _updatePosition(GameObject primitive)
        {
            Node node = primitive.GetComponent<NodeContainer>().Node;

            Material material = _nodeBaseMaterial;
            if (node == _state.ActiveNode)
            {
                material = _nodeHighlightMaterial;
            }
            primitive.GetComponent<Renderer>().material = material;

            primitive.transform.position = new Vector3(
                node.Pos.x,
                node.Elevation.Value,
                node.Pos.y);

            float width = node.RiverWidth.Value + node.ShoreWidth.Value;
            float depth = node.ShoreDepth.Value;
            primitive.transform.localScale = new Vector3(width, depth * 2, width);
        }
    }
}
