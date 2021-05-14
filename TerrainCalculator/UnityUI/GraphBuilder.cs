using System;
using TerrainCalculator.Network;
using UnityEngine;

namespace TerrainCalculator.UnityUI
{
    public class GraphBuilder : MonoBehaviour
    {
        private WaterNetwork _net;

        public void Start()
        {
            Debug.Log("Dragger start");
            _net = GetComponent<State>().Net;
        }

        public void Update()
        {
            _net.InterpolateAll();
        }
    }
}
