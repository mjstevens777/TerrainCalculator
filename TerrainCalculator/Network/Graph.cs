using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TerrainCalculator.Network
{
    public class Node
    {
        public enum Key
        {
            Elevation,
            ShoreWidth,
            ShoreDepth,
            RiverWidth,
            RiverSlope
        }

        WaterNetwork _network;
        public Vector2 Pos;

        // Interpolation gradient
        public Vector2 Grad;

        public bool IsDirty;

        // Control parameters
        public Dictionary<Key, FlagDouble> ImplicitValues;

        public Node(WaterNetwork network)
        {
            _network = network;
            Pos = new Vector2(0, 0);
            Grad = new Vector2(0, 0);
            ImplicitValues = new Dictionary<Key, FlagDouble>();
            ImplicitValues[Key.Elevation] = new FlagDouble();
            ImplicitValues[Key.ShoreWidth] = new FlagDouble();
            ImplicitValues[Key.ShoreDepth] = new FlagDouble();
            ImplicitValues[Key.RiverWidth] = new FlagDouble();
            ImplicitValues[Key.RiverSlope] = new FlagDouble();
        }

        public FlagDouble Elevation
        {
            get => ImplicitValues[Key.Elevation];
        }

        public FlagDouble ShoreWidth
        {
            get => ImplicitValues[Key.ShoreWidth];
        }

        public FlagDouble ShoreDepth
        {
            get => ImplicitValues[Key.ShoreDepth];
        }

        public FlagDouble RiverWidth
        {
            get => ImplicitValues[Key.RiverWidth];
        }

        public FlagDouble RiverSlope
        {
            get => ImplicitValues[Key.RiverSlope];
        }

        public void SetDefault()
        {
            Elevation.SetFixed(40);
            ShoreWidth.SetFixed(10);
            ShoreDepth.SetFixed(10);
            RiverWidth.SetFixed(20);
            RiverSlope.SetFixed(2);
        }

        public void ResetImplicit()
        {
            Elevation.ResetImplicit();
            ShoreWidth.ResetImplicit();
            ShoreDepth.ResetImplicit();
            RiverWidth.ResetImplicit();
            RiverSlope.ResetImplicit();
        }
    }

    public class Edge : QuikGraph.Edge<Node>
    {
        public List<Vector2> InterpPoints;
        public bool Flat;
        private double? _distance;

        public Edge(Node source, Node target, List<Vector2> interp, bool flat)
            : base(source, target)
        {
            InterpPoints = interp;
            Flat = flat;
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
