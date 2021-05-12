using System;
using UnityEngine;

namespace TerrainCalculator.UnityUI
{
    public class GraphBuilder : MonoBehaviour
    {
        State _state;

        public void Start()
        {
            Debug.Log("Dragger start");
            _state = gameObject.GetComponent<State>();
        }

        public void Update()
        {
            _state.Net.InterpolateAll();
        }
    }
}
