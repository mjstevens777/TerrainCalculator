using System;
using TerrainCalculator.Network;
using UnityEngine;

namespace TerrainCalculator.UnityUI
{
    public class GraphBuilder : MonoBehaviour
    {
        private WaterNetwork _net;

        public bool IsDirty;

        public void Start()
        {
            Debug.Log("GraphBuilder start");
            _net = GetComponent<State>().Net;
        }

        public void Update()
        {
            if (!IsDirty) return;
            IsDirty = false;
            GetComponent<GridBuilder>().IsDirty = true;
            _net.InterpolateAll();
        }
    }
}
