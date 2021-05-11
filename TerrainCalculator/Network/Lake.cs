﻿using System;
using System.Collections.Generic;

namespace TerrainCalculator.Network
{
    public class Lake : Path
    {
        public Lake(WaterNetwork network) : base(network)
        {
        }

        protected override int _wrapIndex(int index)
        {
            int c = Nodes.Count;
            return (index % c + c) % c;
        }

        // https://stackoverflow.com/a/1082938/5945112
        private int mod(int x, int m) => (x % m + m) % m;

        public override List<Edge> GetEdges()
        {
            List<Edge> edges = base.GetEdges();
            foreach(Edge edge in edges)
            {
                edge.Flat = true;
            }
            return edges;
        }
    }
}
