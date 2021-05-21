using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using QuikGraph;
using QuikGraph.Algorithms;
using QuikGraph.Algorithms.Search;
using QuikGraph.Algorithms.ShortestPath;
using UnityEngine;

namespace TerrainCalculator.Network
{
    public class Graph : QuikGraph.BidirectionalGraph<Node, Edge>
    {
        public Graph() : base() { }
    }

    public class WaterNetwork
    {
        List<Node> _nodes;
        List<Lake> _lakes;
        List<River> _rivers;
        Graph _graph;

        public WaterNetwork()
        {
            _nodes = new List<Node>();
            _lakes = new List<Lake>();
            _rivers = new List<River>();
        }

        public Node NewNode()
        {
            Node node = new Node(this);
            _nodes.Add(node);
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

        public void RemoveRiver(River river)
        {
            _rivers.Remove(river);
        }

        public void RemoveLake(Lake lake)
        {
            _lakes.Remove(lake);
        }

        public void RemoveNode(Node node)
        {
            List<River> riversToRemove = new List<River>();
            foreach (var river in _rivers) {
                river.Nodes.Remove(node);
                if (river.Nodes.Count < 2)
                {
                    riversToRemove.Add(river);
                }
            }
            foreach(var river in riversToRemove)
            {
                RemoveRiver(river);
            }
            List<Lake> lakesToRemove = new List<Lake>();
            foreach (var lake in _lakes)
            {
                lake.Nodes.Remove(node);
                if (lake.Nodes.Count < 2)
                {
                    lakesToRemove.Add(lake);
                }
            }
            foreach (var lake in lakesToRemove)
            {
                RemoveLake(lake);
            }
        }

        public IEnumerable<Node> Nodes
        {
            get
            {
                if (_graph == null) return Enumerable.Empty<Node>();
                return _graph.Vertices;
            }
        }

        public IEnumerable<Edge> Edges
        {
            get
            {
                if (_graph == null) return Enumerable.Empty<Edge>();
                return _graph.Edges;
            }
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
            _interpolateValue(Node.Key.ShoreWidth);
            _interpolateValue(Node.Key.ShoreDepth);
            _interpolateValue(Node.Key.RiverWidth);
            _interpolateValue(Node.Key.RiverSlope);
            _interpolateZ();
        }

        private void _buildGraph()
        {
            _graph = new Graph();

            foreach (River river in _rivers)
            {
                _graph.AddVertex(river.First); // In case there are no edges
                foreach (Edge edge in river.GetEdges())
                {
                    _graph.AddVerticesAndEdge(edge);
                }
            }

            foreach (Lake lake in _lakes)
            {
                _graph.AddVertex(lake.First); // In case there are no edges
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

            Func<Edge, double> edgeCost = e => e.IsLake ? 0.0 : e.Distance;

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
                    if (edge.IsLake)
                    {
                        edge.Target.Elevation.SetImplicit(edge.Source.Elevation.Value);
                    } else
                    {
                        float angle = (edge.Source.RiverSlope.Value + edge.Target.RiverSlope.Value) / 2.0f;
                        float angleRad = Mathf.Deg2Rad * angle;
                        float slope = Mathf.Tan(angleRad);
                        float deltaZ = slope * edge.Distance;
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

        private void _interpolateValue(Node.Key key)
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
                    bool firstSet = river.First.ImplicitValues[key].IsSet;
                    bool lastSet = river.Last.ImplicitValues[key].IsSet;
                    int numSet = 0;
                    if (river.First.ImplicitValues[key].IsSet) numSet++;
                    if (river.Last.ImplicitValues[key].IsSet) numSet++;
                    if (numSet > maxNumSet) maxNumSet = numSet;
                }

                foreach (River river in _rivers)
                {
                    bool firstSet = river.Nodes[0].ImplicitValues[key].IsSet;
                    bool lastSet = river.Nodes[river.Nodes.Count - 1].ImplicitValues[key].IsSet;
                    int numSet = 0;
                    if (river.First.ImplicitValues[key].IsSet) numSet++;
                    if (river.Last.ImplicitValues[key].IsSet) numSet++;
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

        private void _interpolateChainValue(List<Node> chain, Node.Key key)
        {
            CumulativeLerp cumLerp = new CumulativeLerp();
            List<Edge> edges = new List<Edge>();
            foreach (int i in Enumerable.Range(0, chain.Count - 1))
            {
                Node start = chain[i];
                Node end = chain[i + 1];
                Edge edge;
                _graph.TryGetEdge(start, end, out edge);
                if (edge == null) _graph.TryGetEdge(end, start, out edge);
                if (edge == null) throw new IndexOutOfRangeException("Could not find edge for chain");
                edges.Add(edge);
                cumLerp.Add(edge.Distance);
            }
            float startValue = chain[0].ImplicitValues[key].Value;
            float endValue = chain[chain.Count - 1].ImplicitValues[key].Value;

            foreach (int i in Enumerable.Range(0, chain.Count - 2))
            {
                Node node = chain[i + 1];
                FlagDouble value = node.ImplicitValues[key];
                if (value.IsSet) throw new NodeException($"{key.ToString()} defined multiple times", node);
                value.SetImplicit(cumLerp[i].Interp(startValue, endValue));
            }
        }

        private void _interpolateEndpointChainValue(List<Node> chain, Node.Key key)
        {
            float value = chain[0].ImplicitValues[key].Value;
            foreach (Node node in chain.GetRange(1, chain.Count - 1))
            {
                FlagDouble implicitValue = node.ImplicitValues[key];
                if (implicitValue.IsSet) throw new NodeException($"{key.ToString()} defined multiple times", node);
                node.ImplicitValues[key].SetImplicit(value);
            }
        }
    }
}