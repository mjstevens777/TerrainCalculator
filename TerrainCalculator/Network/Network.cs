using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TerrainCalculator.Network
{
    public class Network
    {
        List<Lake> _lakes;
        List<River> _rivers;

        public Network()
        {
            _lakes = new List<Lake>();
            _rivers = new List<River>();
        }

        public Node NewNode()
        {
            Node node = new Node(this);
            return node;
        }

        public River NewRiver()
        {
            River river = new River(this);
            _rivers.Add(river);
            return river;
        }

        public Lake NewLake()
        {
            Lake lake = new Lake(this);
            _lakes.Add(lake);
            return lake;
        }

        public ReadOnlyCollection<Node> Nodes
        {
            get
            {
                HashSet<Node> nodes = new HashSet<Node>();
                foreach (River river in _rivers)
                {
                    foreach(Node node in river)
                    {
                        nodes.Add(node);
                    }
                }
                foreach (Lake lake in _lakes)
                {
                    foreach (Node node in lake)
                    {
                        nodes.Add(node);
                    }
                }
                return new List<Node>(nodes).AsReadOnly();
            }
        }
    }
}
