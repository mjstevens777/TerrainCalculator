using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TerrainCalculator.Network
{
    public class Node
    {
        public enum ImplicitKey
        {
            Z,
            Width,
            Depth,
            Slope
        }

        Network _network;
        public Vector2 Pos;

        // Interpolation gradient
        public Vector2 Grad;

        public bool IsDirty;

        // Control parameters
        public Dictionary<ImplicitKey, FlagDouble> ImplicitValues;

        public Node(Network network)
        {
            _network = network;
            Pos = new Vector2(0, 0);
            Grad = new Vector2(0, 0);
            ImplicitValues = new Dictionary<ImplicitKey, FlagDouble>();
            ImplicitValues[ImplicitKey.Z] = new FlagDouble();
            ImplicitValues[ImplicitKey.Width] = new FlagDouble();
            ImplicitValues[ImplicitKey.Depth] = new FlagDouble();
            ImplicitValues[ImplicitKey.Slope] = new FlagDouble();
        }

        public FlagDouble Z
        {
            get => ImplicitValues[ImplicitKey.Z];
        }

        public FlagDouble Width
        {
            get => ImplicitValues[ImplicitKey.Width];
        }

        public FlagDouble Depth
        {
            get => ImplicitValues[ImplicitKey.Depth];
        }

        public FlagDouble Slope
        {
            get => ImplicitValues[ImplicitKey.Slope];
        }

        public void SetDefault()
        {
            Z.SetFixed(40);
            Width.SetFixed(40);
            Depth.SetFixed(10);
            Slope.SetFixed(1);
        }

        public void ResetImplicit()
        {
            Z.ResetImplicit();
            Width.ResetImplicit();
            Depth.ResetImplicit();
            Slope.ResetImplicit();
        }
    }

    public class Edge : QuikGraph.Edge<Node>
    {
        List<Vector2> InterpPoints;
        private double? _distance;

        public Edge(Node source, Node target, List<Vector2> interp)
            : base(source, target)
        {
            InterpPoints = interp;
        }

        public double Distance
        {
            get
            {
                if (_distance != null) { return (double)_distance; }
                double sum = 0;
                foreach (int i in Enumerable.Range(0, InterpPoints.Count - 1))
                {
                    Vector2 start = InterpPoints[i];
                    Vector2 end = InterpPoints[i + 1];
                    sum += (end - start).magnitude;
                }
                _distance = sum;
                return (double)_distance;
            }
        }
    }

    public class Graph: QuikGraph.BidirectionalGraph<Node, Edge>
    {
        public Graph() : base() { }
    }

    public class ComputeEdge : QuikGraph.Edge<Node>
    {
        public float Weight;

        // source.value = sum edge.target.value * edge.weight for edge in neighbors
        public ComputeEdge(Node source, Node target, float weight)
            : base(source, target)
        {
            Weight = weight;
        }
    }

    public class ComputeGraph : QuikGraph.AdjacencyGraph<Node, ComputeEdge>
    {
        public ComputeGraph() : base() { }
    }
}
