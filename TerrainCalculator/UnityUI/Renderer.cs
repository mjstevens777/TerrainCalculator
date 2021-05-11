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
        public State state;
        private GameObject _primitive;

        private Material _nodeMaterial;

        public void Start()
        {
            Debug.Log("NetworkRenderer start");
            state = gameObject.GetComponent<State>();

            _nodeMaterial = new Material(Shader.Find("Sprites/Default"));
            _nodeMaterial.color = new Color(1, 1, 0, 0.75f);

            _primitive = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _primitive.GetComponent<Renderer>().material = _nodeMaterial;
            _primitive.transform.position = Vector3.up * 200;
        }

        public void Update()
        {
            Camera cam = Camera.main;
            Ray mouseRay = cam.ScreenPointToRay(Input.mousePosition);
            Vector3 origin = mouseRay.origin;
            Vector3 vector = origin + (mouseRay.direction.normalized * cam.farClipPlane);
            Segment3 ray = new Segment3(origin, vector); //using colossal math just for this
            Vector3 hitpos;
            if (Singleton<TerrainManager>.instance.RayCast(ray, out hitpos))
            {
                if (state.ActiveNode != null)
                {
                    Debug.Log("NetworkRenderer setting pos from mouse");
                    state.ActiveNode.Pos.x = hitpos.x;
                    state.ActiveNode.Pos.y = hitpos.z;
                    state.ActiveNode.Elevation.SetFixed(hitpos.y);
                }
            }
        }

        public void LateUpdate()
        {
            if (state.ActiveNode != null)
            {
                Debug.Log("NetworkRenderer rendering pos from node");
                Debug.Log($"x {state.ActiveNode.Pos.x}");
                Debug.Log($"y {state.ActiveNode.Elevation.Value}");
                Debug.Log($"z {state.ActiveNode.Pos.y}");
                Debug.Log($"p {_primitive == null}");
                Debug.Log($"p.t {_primitive.transform == null}");
                Debug.Log($"p.t.p {_primitive.transform.position == null}");
                _primitive.transform.position = new Vector3(
                    state.ActiveNode.Pos.x,
                    state.ActiveNode.Elevation.Value,
                    state.ActiveNode.Pos.y);
            }
        }
    }
}
