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
        public bool IsDragging;

        private Node _node;
        private bool _setElevation;
        private Camera _camera;
        public Vector3 MousePos;
        public Node SnapNode;

        public void Start()
        {
            Debug.Log("Dragger start");
            _camera = Camera.main;
        }

        public void StartDrag(Node node, bool setElevation)
        {
            _node = node;
            _setElevation = setElevation;
            SnapNode = null;
        }

        public void StopDrag()
        {
            _node = null;
            _setElevation = false;
        }

        public void Update()
        {
            if (_node == null) return;
            if (UIView.IsInsideUI()) return;

            GetComponent<NodeCollection>().HighlightNode = _node;

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

            Ray mouseRay = _camera.ScreenPointToRay(Input.mousePosition);
            Vector3 origin = mouseRay.origin;
            Vector3 vector = origin + (mouseRay.direction.normalized * _camera.farClipPlane);
            Segment3 ray = new Segment3(origin, vector);
            if (Singleton<TerrainManager>.instance.RayCast(ray, out MousePos))
            {
                Debug.Log("Setting position from drag");
                _node.Pos.x = MousePos.x;
                _node.Pos.y = MousePos.z;
                _node.Elevation.SetFixed(MousePos.y);
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
            collection.HideNode = SnapNode;
            return true;
        }

        private void _updateFromPlane()
        {
            Plane plane = new Plane(Vector3.up, Vector3.up * _node.Elevation.Value);
            Ray mouseRay = _camera.ScreenPointToRay(Input.mousePosition);
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
