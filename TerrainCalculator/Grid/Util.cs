using System;
using UnityEngine;

namespace TerrainCalculator.Grid
{
    public class ZValue : IGridValue<ZValue>
    {
        public float LandSlope { get; private set; }
        public float RiverSlope { get; private set; }
        public float Elevation { get; private set; }

        public ZValue(float landSlope, float riverSlope, float elevation)
        {
            LandSlope = landSlope;
            if (LandSlope < 0) throw new Exception("LandSlope cannot be negative");
            RiverSlope = riverSlope;
            if (RiverSlope < 0) throw new Exception("RiverSlope cannot be negative");
            Elevation = elevation;
        }

        public ZValue NextValue(ZValue start, float distance)
        {
            float thisSlope = start.RiverSlope + LandSlope;
            float otherSlope = (start.RiverSlope + start.LandSlope);
            float slope = (thisSlope + otherSlope) / 2f;
            float deltaZ = slope * distance;
            return new ZValue(
                landSlope: LandSlope,
                riverSlope: start.RiverSlope,
                elevation: start.Elevation + deltaZ);
        }

        public int CompareTo(object obj)
        {
            ZValue other = obj as ZValue;
            return Elevation.CompareTo(other.Elevation);
        }
    }
}
