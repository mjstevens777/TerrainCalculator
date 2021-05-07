using System;
using System.Collections.Generic;
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
}
