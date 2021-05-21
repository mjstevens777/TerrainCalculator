using System;
using System.Collections.Generic;
using ICities;
using UnityEngine;

namespace TerrainCalculator.Grid
{
    public class SegmentNode
    {
        public Vector2 Pos;

        public float Elevation;
        public float ShoreWidth;
        public float ShoreDepth;
        public float RiverWidth;
        public float RiverSlope;

        public SegmentNode(Vector2 pos, float elevation, float shoreWidth,
                           float shoreDepth, float riverWidth, float riverSlope)
        {
            Pos = pos;
            Elevation = elevation;
            ShoreWidth = shoreWidth;
            ShoreDepth = shoreDepth;
            RiverWidth = riverWidth;
            RiverSlope = riverSlope;
        }

        public SegmentNode RemapToTerrain(ITerrain terrain)
        {
            int x, z;
            terrain.PositionToHeightMapCoord(Pos.x, Pos.y, out x, out z);
            return new SegmentNode(
                new Vector2(x, z),
                elevation: Elevation,
                shoreWidth: ShoreWidth,
                shoreDepth: ShoreDepth,
                riverWidth: RiverWidth,
                riverSlope: RiverSlope);
        }
    }

    public class Segment
    {
        public SegmentNode Start;
        public SegmentNode End;
        public bool IsLake;

        public Segment(SegmentNode start, SegmentNode end, bool isLake)
        {
            Start = start;
            End = end;
            IsLake = isLake;
        }

        public void RemapToTerrain(ITerrain terrain)
        {
            Start = Start.RemapToTerrain(terrain);
            End = End.RemapToTerrain(terrain);
        }

        public IEnumerable<SegmentNode> Draw()
        {
            Vector2 delta = End.Pos - Start.Pos;
            float dx = Mathf.Abs(delta.x);
            float dy = Mathf.Abs(delta.y);

            float tStart, tEnd;
            int numSteps;

            if (dx > dy)
            {
                int x1 = Mathf.RoundToInt(Start.Pos.x);
                int x2 = Mathf.RoundToInt(End.Pos.x);
                tStart = ((float)x1 - Start.Pos.x) / delta.x ;
                tEnd = ((float)x2 - Start.Pos.x) / delta.x;
                numSteps = Math.Abs(x2 - x1);
            } else
            {
                int y1 = Mathf.RoundToInt(Start.Pos.y);
                int y2 = Mathf.RoundToInt(End.Pos.y);
                tStart = ((float)y1 - Start.Pos.y) / delta.y;
                tEnd = ((float)y2 - Start.Pos.y) / delta.y;
                numSteps = Math.Abs(y2 - y1);
            }

            float tStep;
            if (numSteps > 0)
            {
                tStep = (tEnd - tStart) / numSteps;
            } else
            {
                tStart = 0.5f;
                tStep = 0;
            }

            for (int i = 0; i <= numSteps; i++)
            {
                SegmentNode node = Interp(tStart + tStep * i);
                node.Pos.x = Mathf.Round(node.Pos.x);
                node.Pos.y = Mathf.Round(node.Pos.y);
                yield return node;
            }
        }

        public SegmentNode Interp(float tVal)
        {
            Lerp t = new Lerp(tVal);
            return new SegmentNode(
                t.Interp(Start.Pos, End.Pos),
                elevation: t.Interp(Start.Elevation, End.Elevation),
                riverSlope: t.Interp(Start.RiverSlope, End.RiverSlope),
                riverWidth: t.Interp(Start.RiverWidth, End.RiverWidth),
                shoreDepth: t.Interp(Start.ShoreDepth, End.ShoreDepth),
                shoreWidth: t.Interp(Start.ShoreWidth, End.ShoreWidth));
        }
    }
}
