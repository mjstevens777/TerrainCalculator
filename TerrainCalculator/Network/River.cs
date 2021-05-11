using System;
namespace TerrainCalculator.Network
{
    public class River : Path
    {
        public River(WaterNetwork network) : base(network)
        {
        }

        public Node First { get => Nodes[0]; }
        public Node Last { get => Nodes[Nodes.Count - 1]; }
    }
}
