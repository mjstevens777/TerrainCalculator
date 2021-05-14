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
        private List<GameObject> _primitives;
        private Material _nodeBaseMaterial;
        private Material _nodeHighlightMaterial;
        public Node HoveredNode;

        public void Start()
        {
            Debug.Log("NetworkRenderer start");
            _state = gameObject.GetComponent<State>();

            _primitives = new List<GameObject>();

            _nodeBaseMaterial = new Material(Shader.Find("Custom/Props/Prop/Default"));
            _nodeBaseMaterial.color = new Color(0, 0, 1, 1);

            _nodeHighlightMaterial = new Material(Shader.Find("Custom/Props/Prop/Default"));
            _nodeHighlightMaterial.color = new Color(1, 1, 0, 1);
        }

        public void Update()
        {
            _syncNodes();
            _checkCollisions();
        }

        public void LateUpdate()
        {
            _updatePositions();
        }

        private void _syncNodes()
        {
            List<Node> nodes = new List<Node>(_state.Net.Nodes);

            for (int i = _primitives.Count; i < nodes.Count; i++)
            {
                Debug.Log($"Creating primitive {i}");
                GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Debug.Log("Adding node container");
                primitive.AddComponent<NodeContainer>();
                Debug.Log("Adding mesh collider");
                primitive.AddComponent<MeshCollider>();
                _primitives.Add(primitive);
            }

            for (int i = _primitives.Count - 1; i >= nodes.Count; i--)
            {
                Debug.Log($"Destroying primitive {i}");
                GameObject.Destroy(_primitives[i]);
                _primitives.RemoveAt(i);
            }

            for (int i = 0; i < nodes.Count; i++)
            {
                _primitives[i].GetComponent<NodeContainer>().Node = nodes[i];
            }
        }

        private void _checkCollisions()
        {
            _updatePositions();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float maxDist = Camera.main.farClipPlane;
            RaycastHit hit;
            
            foreach (GameObject primitive in _primitives)
            {
                MeshCollider collider = primitive.GetComponent<MeshCollider>();
                if (collider.Raycast(ray, out hit, maxDist))
                {
                    Node node = primitive.GetComponent<NodeContainer>().Node;
                    if (node == _state.ActiveNode) continue;
                    if (node != HoveredNode)
                    {
                        Debug.Log($"Hovering on node");
                    }
                    HoveredNode = node;
                    return;
                }
            }
            if (HoveredNode != null) { 
                Debug.Log($"Unhovering node");
            }
            HoveredNode = null;
        }

        private void _updatePositions()
        {
            foreach (GameObject primitive in _primitives)
            {
                _updatePosition(primitive);
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
