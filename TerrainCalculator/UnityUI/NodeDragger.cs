using System;
using System.Collections.Generic;
using ColossalFramework;
using ColossalFramework.Math;
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

        public void Start()
        {
            Debug.Log("Dragger start");
            _camera = Camera.main;
        }

        public void StartDrag(Node node, bool setElevation)
        {
            _node = node;
            _setElevation = setElevation;
        }

        public void StopDrag()
        {
            _node = null;
            _setElevation = false;
        }

        public void Update()
        {
            Debug.Log("Dragger update");
            if (_node == null) return;
            Debug.Log("Node exists");
            Ray mouseRay = _camera.ScreenPointToRay(Input.mousePosition);
            Vector3 origin = mouseRay.origin;
            Vector3 vector = origin + (mouseRay.direction.normalized * _camera.farClipPlane);
            Segment3 ray = new Segment3(origin, vector); //using colossal math just for this
            if (Singleton<TerrainManager>.instance.RayCast(ray, out MousePos))
            {
                Debug.Log("Setting position from drag");
                _node.Pos.x = MousePos.x;
                _node.Pos.y = MousePos.z;
                if (_setElevation)
                {
                    _node.Elevation.SetFixed(MousePos.y);
                }
            }
        }
    }
}
