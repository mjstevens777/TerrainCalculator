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
        public Node HideNode;
        public Node HighlightNode;

        private WaterNetwork _net;
        private List<GameObject> _primitives;
        private Material _nodeBaseMaterial;
        private Material _nodeHighlightMaterial;

        public void Start()
        {
            Debug.Log("NodeCollection start");

            _net = GetComponent<State>().Net;

            _primitives = new List<GameObject>();

            _nodeBaseMaterial = new Material(Shader.Find("Custom/Props/Prop/Default"));
            _nodeBaseMaterial.color = new Color(0, 0, 1, 1);

            _nodeHighlightMaterial = new Material(Shader.Find("Custom/Props/Prop/Default"));
            _nodeHighlightMaterial.color = new Color(1, 1, 0, 1);
        }

        public void Update()
        {
            HighlightNode = null;
            HideNode = null;
        }

        public void OnDestroy()
        {
            foreach (var primitive in _primitives) Destroy(primitive);
        }

        public void _syncNodes()
        {
            List<Node> nodes = new List<Node>(_net.Nodes);

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

        public void LateUpdate()
        {
            _syncNodes();
            _drawNodes();
        }

        public Node CheckCollisions(Node exclude = null, Node include = null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float maxDist = Camera.main.farClipPlane;
            RaycastHit hit;

            foreach (GameObject primitive in _primitives)
            {
                Node node = primitive.GetComponent<NodeContainer>().Node;
                if (node == exclude) continue;
                if (include != null && node != include) continue;
                _updatePosition(primitive);
                MeshCollider collider = primitive.GetComponent<MeshCollider>();
                if (collider.Raycast(ray, out hit, maxDist))
                {
                    return node;
                }
            }
            return null;
        }

        public bool CheckCollision(Node node)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float maxDist = Camera.main.farClipPlane;
            RaycastHit hit;

            foreach (GameObject primitive in _primitives)
            {
                Node primitiveNode = primitive.GetComponent<NodeContainer>().Node;
                if (node != primitiveNode) continue;
                _updatePosition(primitive);
                MeshCollider collider = primitive.GetComponent<MeshCollider>();
                return collider.Raycast(ray, out hit, maxDist);
            }
            return false;
        }

        private void _drawNodes()
        {
            foreach (GameObject primitive in _primitives)
            {
                _updatePosition(primitive);
                _updateMaterial(primitive);
            }
        }


        private void _updatePosition(GameObject primitive)
        {
            Node node = primitive.GetComponent<NodeContainer>().Node;
            primitive.transform.position = new Vector3(
                node.Pos.x,
                node.Elevation.Value,
                node.Pos.y);

            float width = node.RiverWidth.Value + node.ShoreWidth.Value;
            float depth = node.ShoreDepth.Value;
            primitive.transform.localScale = new Vector3(width, depth * 2, width);
        }

        private void _updateMaterial(GameObject primitive)
        {
            Node node = primitive.GetComponent<NodeContainer>().Node;
            primitive.SetActive(node != HideNode);
            var material = node == HighlightNode ? _nodeHighlightMaterial : _nodeBaseMaterial;
            primitive.GetComponent<Renderer>().material = material;
        }
    }
}
