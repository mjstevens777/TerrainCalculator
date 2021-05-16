using System;
using ColossalFramework.UI;
using TerrainCalculator.Network;
using UnityEngine;

namespace TerrainCalculator.UnityUI
{
    public class PlaceNodeMode : MonoBehaviour
    {
        private WaterNetwork _net;
        private Path _path;
        private Node _node;

        public void Start()
        {
            enabled = false;
            _net = GetComponent<State>().Net;
        }

        public void EnterMode(Path path)
        {
            GetComponent<GraphBuilder>().IsDirty = true;
            Debug.Log($"Entering node placement mode");
            enabled = true;
            _path = path;
            _node = _net.NewNode();
            path.Nodes.Add(_node);
            if (path.Nodes.Count == 1)
            {
                Debug.Log($"Place first node");
                _node.SetDefault();
                GetComponent<NodeDragger>().EnterMode(_node, setElevation: true);
            }
            else
            {
                Debug.Log($"Place subsequent node");
                GetComponent<NodeDragger>().EnterMode(_node, setElevation: false);
            }
        }

        public void ExitMode()
        {
            GetComponent<NodeDragger>().ExitMode();
            _path.Nodes.Remove(_node);
            if (_path.Nodes.Count < 2) _deletePath();
            _path = null;
            _node = null;
            enabled = false;
        }

        public void DeletePath()
        {
            _deletePath();
            GetComponent<State>().EnterBase();
        }

        public void Update()
        {
            if (UIView.IsInsideUI()) return;

            bool isSnapped = _checkForSnap();

            if (Input.GetMouseButtonDown(0))
            {
                // Left mouse
                if (isSnapped)
                {
                    GetComponent<State>().EnterBase();
                    return;
                }
                _placeNode();
            }
            if (Input.GetMouseButtonDown(1))
            {
                // Right mouse
                if (_path.Nodes.Count <= 1) {
                    // Can't go back, done
                    DeletePath();
                }
                else
                {
                    // Go back
                    _goBack();
                }
            }
        }

        private bool _checkForSnap()
        {
            if (_path.Nodes.Count < 2) return false;
            var collection = GetComponent<NodeCollection>();
            Node prev = _path.Nodes[_path.Nodes.Count - 2];
            bool found = collection.CheckCollision(prev);
            if (!found) return false;

            GetComponent<GraphBuilder>().IsDirty = true;
            _node.Pos.x = prev.Pos.x;
            _node.Pos.y = prev.Pos.y;
            collection.HideNode = _node;
            // TODO: Fix highlighting and hide edge
            collection.HighlightNode = prev;
            return true;
        }
        private void _deletePath()
        {
            GetComponent<GraphBuilder>().IsDirty = true;
            _path.Nodes.Clear();
            if (_path is River)
            {
                Debug.Log($"Delete River");
                _net.RemoveRiver((River)_path);
            }
            else if (_path is Lake)
            {
                Debug.Log($"Delete Lake");
                _net.RemoveLake((Lake)_path);
            }
        }

        private void _placeNode()
        {
            GetComponent<GraphBuilder>().IsDirty = true;
            Debug.Log("Place node");
            Node snapNode = GetComponent<NodeDragger>().SnapNode;
            if (snapNode != null)
            {
                Debug.Log("Snap node to existing");
                _path.Nodes.Remove(_node);
                if (_path.Nodes.Contains(snapNode)) {
                    // Double click
                    GetComponent<State>().EnterBase();
                    return;
                }
                _path.Nodes.Add(snapNode);
            }
            Node node = _net.NewNode();
            // Set to existing location
            node.Pos.x = _node.Pos.x;
            node.Pos.y = _node.Pos.y;
            node.Elevation.Value = _node.Elevation.Value;
            // Set as new active node
            _path.Nodes.Add(node);
            _node = node;
            GetComponent<NodeDragger>().EnterMode(_node, setElevation: false);
        }

        private void _goBack()
        {
            GetComponent<GraphBuilder>().IsDirty = true;
            _path.Nodes.Remove(_node);
            _node = _path.Last;
            GetComponent<NodeDragger>().EnterMode(_node, setElevation: _node.Elevation.IsFixed);
        }
    }
}
