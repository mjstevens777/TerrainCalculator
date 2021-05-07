using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TerrainCalculator.Network
{
    public class Network
    {
        List<Lake> _lakes;
        List<River> _rivers;
        Graph _graph;

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
                    foreach (Node node in river)
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

        public void BuildGraph()
        {
            _graph = new Graph();
            foreach (Node node in Nodes)
            {
                _graph.AddVertex(node);
            }

            foreach (River river in _rivers)
            {
                foreach (Edge edge in river.GetEdges())
                {
                    _graph.AddEdge(edge);
                }
            }

            foreach (Lake lake in _lakes)
            {
                foreach (Edge edge in lake.GetEdges())
                {
                    _graph.AddEdge(edge);
                }
            }
        }

        public void InterpolateZ()
        {

        }

        public void InterpolateValue(Node.ImplicitKey key)
        {
            bool wasSet = true;
            while (wasSet)
            {
                wasSet = false;
                foreach (Lake lake in _lakes)
                {
                    List<List<Node>> chains = lake.GetChains(key);
                    foreach (List<Node> chain in chains)
                    {
                        interpolateChainValue(chain, key);
                        wasSet = true;
                    }
                }

                int maxNumSet = 1; // Ignore chains with no value set
                foreach (River river in _rivers)
                {
                    bool firstSet = river[0].ImplicitValues[key].IsSet;
                    bool lastSet = river[river.Count - 1].ImplicitValues[key].IsSet;
                    int numSet = 0;
                    if (river[0].ImplicitValues[key].IsSet) numSet++;
                    if (river[river.Count - 1].ImplicitValues[key].IsSet) numSet++;
                    if (numSet > maxNumSet) maxNumSet = numSet;
                }

                foreach (River river in _rivers)
                {
                    bool firstSet = river[0].ImplicitValues[key].IsSet;
                    bool lastSet = river[river.Count - 1].ImplicitValues[key].IsSet;
                    int numSet = 0;
                    if (river[0].ImplicitValues[key].IsSet) numSet++;
                    if (river[river.Count - 1].ImplicitValues[key].IsSet) numSet++;
                    if (numSet < maxNumSet) continue;
                    List<List<Node>> chains = river.GetChains(key);
                    foreach (List<Node> chain in chains)
                    {
                        interpolateChainValue(chain, key);
                        wasSet = true;
                    }
                    chains = river.GetEndpointChains(key);
                    foreach (List<Node> chain in chains)
                    {
                        interpolateEndpointChainValue(chain, key);
                        wasSet = true;
                    }
                }
            }
        }

        private void interpolateChainValue(List<Node> chain, Node.ImplicitKey key)
        {
            double totalDistance = 0;
            foreach (int i in Enumerable.Range(0, chain.Count - 1))
            {
                Node start = chain[i];
                Node end = chain[i + 1];
                Edge edge;
                _graph.TryGetEdge(start, end, out edge);
                if (edge == null)
                {
                    _graph.TryGetEdge(end, start, out edge);
                }
                if (edge == null) throw new IndexOutOfRangeException("Could not find edge for chain");
                totalDistance += edge.Distance;
            }
            double startValue = chain[0].ImplicitValues[key].Value;
            double endValue = chain[chain.Count - 1].ImplicitValues[key].Value;

            double cumDistance = 0;
            foreach (int i in Enumerable.Range(0, chain.Count - 2))
            {
                Node start = chain[i];
                Node end = chain[i + 1];
                Edge edge;
                _graph.TryGetEdge(start, end, out edge);
                if (edge == null) throw new IndexOutOfRangeException("Could not find edge for chain");
                cumDistance += edge.Distance;
                double t = cumDistance / totalDistance;
                end.ImplicitValues[key].SetImplicit(t * endValue + (1 - t) * startValue);
            }
        }

        private void interpolateEndpointChainValue(List<Node> chain, Node.ImplicitKey key)
        {
            double value = chain[0].ImplicitValues[key].Value;
            foreach (int i in Enumerable.Range(1, chain.Count - 1))
            {
                chain[i].ImplicitValues[key].SetImplicit(value);
            }
        }
    }
}