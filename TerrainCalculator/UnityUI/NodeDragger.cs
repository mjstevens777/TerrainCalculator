using System;
using System.Collections.Generic;
using ColossalFramework;
using ColossalFramework.Math;
using ColossalFramework.UI;
using TerrainCalculator.Network;
using UnityEngine;

namespace TerrainCalculator.UnityUI
{
    public class NodeDragger : MonoBehaviour
    {
        private Node _node;
        private bool _setElevation;
        public Node SnapNode;

        public void Start()
        {
            enabled = false;
        }

        public void EnterMode(Node node, bool setElevation)
        {
            enabled = true;
            _node = node;
            _setElevation = setElevation;
        }

        public void ExitMode()
        {
            enabled = false;
            _node = null;
            _setElevation = false;
        }

        public void Update()
        {
            if (UIView.IsInsideUI()) return;

            GetComponent<NodeCollection>().HighlightNode = _node;

            SnapNode = null;
            if (_setElevation)
            {
                _updateFromTerrain();
            } else
            {
                _updateFromPlane();
            }

        }

        private void _updateFromTerrain() {
            // TODO: Add a separate flag from _setElevation that controls snapping?
            if (_snapToHovered()) return;

            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 origin = mouseRay.origin;
            Vector3 vector = origin + (mouseRay.direction.normalized * Camera.main.farClipPlane);
            Segment3 ray = new Segment3(origin, vector);
            Vector3 _mousePos;
            if (Singleton<TerrainManager>.instance.RayCast(ray, out _mousePos))
            {
                Debug.Log("Setting position from drag");
                _node.Pos.x = _mousePos.x;
                _node.Pos.y = _mousePos.z;
                _node.Elevation.SetFixed(_mousePos.y);
            }
        }

        private bool _snapToHovered()
        {
            NodeCollection collection = GetComponent<NodeCollection>();
            SnapNode = collection.CheckCollisions(exclude: _node);

            if (SnapNode == null) return false;
            _node.Pos.x = SnapNode.Pos.x;
            _node.Pos.y = SnapNode.Pos.y;
            if (_setElevation) {
                _node.Elevation.SetFixed(SnapNode.Elevation.Value);
            }
            collection.HideNode = _node;
            collection.HighlightNode = SnapNode;
            return true;
        }

        private void _updateFromPlane()
        {
            Plane plane = new Plane(Vector3.up, Vector3.up * _node.Elevation.Value);
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            float enter;
            if (plane.Raycast(mouseRay, out enter))
            {
                Vector3 pos = mouseRay.GetPoint(enter);
                _node.Pos.x = pos.x;
                _node.Pos.y = pos.z;
            }
        }
    }
}
