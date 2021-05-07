using System;
namespace TerrainCalculator.Network
{
    public class Lake : Path
    {
        public Lake(Network network) : base(network)
        {
        }

        protected override Node getNodeInBounds(int index)
        {
            return this[mod(index, Count)];
        }

        // https://stackoverflow.com/a/1082938/5945112
        private int mod(int x, int m) => (x % m + m) % m;
    }
}
