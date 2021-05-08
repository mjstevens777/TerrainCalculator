using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using QuikGraph;
using QuikGraph.Algorithms;
using QuikGraph.Algorithms.Search;
using QuikGraph.Algorithms.ShortestPath;

namespace TerrainCalculator.Network
{
    public class WaterNetwork
    {
        List<Lake> _lakes;
        List<River> _rivers;
        Graph _graph;

        public WaterNetwork()
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

        class NodeException : Exception
        {
            public Node Node;
            public NodeException(string message, Node node) : base(message)
            {
                Node = node;
            }
        }

        public void InterpolateAll()
        {
            _buildGraph();
            foreach(Node node in _graph.Vertices)
            {
                node.ResetImplicit();
            }
            _interpolateValue(Node.ImplicitKey.ShoreWidth);
            _interpolateValue(Node.ImplicitKey.ShoreDepth);
            _interpolateValue(Node.ImplicitKey.RiverWidth);
            _interpolateValue(Node.ImplicitKey.RiverSlope);
            _interpolateZ();
        }

        private void _buildGraph()
        {
            _graph = new Graph();

            foreach (River river in _rivers)
            {
                foreach (Edge edge in river.GetEdges())
                {
                    _graph.AddVerticesAndEdge(edge);
                }
            }

            foreach (Lake lake in _lakes)
            {
                foreach (Edge edge in lake.GetEdges())
                {
                    _graph.AddVerticesAndEdge(edge);
                }
            }
        }

        private void _interpolateZ()
        {
            List<Node> fixedNodes = new List<Node>();
            foreach(Node node in _graph.Vertices)
            {
                if (node.Elevation.IsFixed) fixedNodes.Add(node);
            }

            Func<Edge, double> edgeCost = e => e.Flat ? 0.0 : e.Distance;

            foreach (Node root in fixedNodes)
            {
                var dfs = new DepthFirstSearchAlgorithm<Node, Edge>(_graph);

                dfs.TreeEdge += (edge) =>
                {
                    if (!edge.Source.Elevation.IsSet) return;
                    if (edge.Target.Elevation.IsSet)
                    {
                        throw new NodeException("Elevation defined in too many places", edge.Target);
                    }
                    if (edge.Flat)
                    {
                        edge.Target.Elevation.SetImplicit(edge.Source.Elevation.Value);
                    } else
                    {
                        double angle = (edge.Source.RiverSlope.Value + edge.Target.RiverSlope.Value) / 2.0;
                        double angleRad = Math.PI * angle / 180.0;
                        double slope = Math.Tan(angleRad);
                        double deltaZ = slope * edge.Distance;
                        edge.Target.Elevation.SetImplicit(edge.Source.Elevation.Value + deltaZ);
                    }
                };

                dfs.SetRootVertex(root);
                dfs.Compute();
            }

            foreach (Node node in _graph.Vertices)
            {
                if (!node.Elevation.IsSet) throw new NodeException("No elevation defined for node", node);
            }
        }

        private void _interpolateValue(Node.ImplicitKey key)
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
                        _interpolateChainValue(chain, key);
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
                        _interpolateChainValue(chain, key);
                        wasSet = true;
                    }
                    chains = river.GetEndpointChains(key);
                    foreach (List<Node> chain in chains)
                    {
                        _interpolateEndpointChainValue(chain, key);
                        wasSet = true;
                    }
                }
            }

            foreach (Node node in _graph.Vertices)
            {
                
                if (!node.ImplicitValues[key].IsSet) throw new NodeException($"No {key.ToString()} value was set", node);
            }
        }

        private void _interpolateChainValue(List<Node> chain, Node.ImplicitKey key)
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
                FlagDouble value = end.ImplicitValues[key];
                if (value.IsSet) throw new NodeException($"{key.ToString()} defined multiple times", end);
                value.SetImplicit(t * endValue + (1 - t) * startValue);
            }
        }

        private void _interpolateEndpointChainValue(List<Node> chain, Node.ImplicitKey key)
        {
            double value = chain[0].ImplicitValues[key].Value;
            foreach (Node node in chain.GetRange(1, chain.Count - 1))
            {
                FlagDouble implicitValue = node.ImplicitValues[key];
                if (implicitValue.IsSet) throw new NodeException($"{key.ToString()} defined multiple times", node);
                node.ImplicitValues[key].SetImplicit(value);
            }
        }
    }
}