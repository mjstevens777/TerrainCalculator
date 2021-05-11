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
        private State _state;
        private Camera _camera;

        public void Start()
        {
            Debug.Log("Dragger start");
            _state = gameObject.GetComponent<State>();
            _camera = Camera.main;
        }

        public void Update()
        {
            if (_state.ActiveNode == null) return;
            if (!_isDragging()) return;
            Ray mouseRay = _camera.ScreenPointToRay(Input.mousePosition);
            Vector3 origin = mouseRay.origin;
            Vector3 vector = origin + (mouseRay.direction.normalized * _camera.farClipPlane);
            Segment3 ray = new Segment3(origin, vector); //using colossal math just for this
            Vector3 hitpos;
            if (Singleton<TerrainManager>.instance.RayCast(ray, out hitpos))
            {
                if (_state.ActiveNode == null)
                {
                    _state.ActiveNode.Pos.x = hitpos.x;
                    _state.ActiveNode.Pos.y = hitpos.z;
                    _state.ActiveNode.Elevation.SetFixed(hitpos.y);
                }
            }
        }

        private bool _isDragging()
        {
            return (_state.Mode == State.ModeType.DRAG_NODE) ||
                   (_state.Mode == State.ModeType.PLACE_NODE);
        }
    }
}
