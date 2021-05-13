using System;
using System.Collections.Generic;

namespace TerrainCalculator.Network
{
    public class Lake : Path
    {
        public Lake(WaterNetwork network) : base(network)
        {
        }

        public override bool IsFlat { get => true; }

        protected override int _wrapIndex(int index)
        {
            int c = Nodes.Count;
            return (index % c + c) % c;
        }

        // https://stackoverflow.com/a/1082938/5945112
        private int mod(int x, int m) => (x % m + m) % m;
    }
}
