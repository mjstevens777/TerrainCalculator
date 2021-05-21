using System;
using UnityEngine;

namespace TerrainCalculator.Grid
{
    public class GridValue : IGridValue<GridValue>
    {
        public float LandSlope { get; private set; }
        public float RiverSlope { get; private set; }
        public float RiverWidth { get; private set; }
        public float ShoreWidth { get; private set; }
        public float ShoreDepth { get; private set; }
        public float Elevation { get; private set; }
        public float ShoreDistance { get; private set; }

        public GridValue(float landSlope, float riverSlope = 0, float riverWidth = 0,
                         float shoreWidth = 0, float shoreDepth = 0,
                         float elevation = 1024, float shoreDistance = 2)
        {
            LandSlope = landSlope;
            if (LandSlope < 0) throw new Exception("LandSlope cannot be negative");
            RiverSlope = riverSlope;
            if (RiverSlope < 0) throw new Exception("RiverSlope cannot be negative");
            Elevation = elevation;
            RiverWidth = riverWidth;
            ShoreWidth = shoreWidth;
            ShoreDistance = shoreDistance;
            ShoreDepth = shoreDepth;
        }

        public float FinalElevation
        {
            get
            {
                if (ShoreDistance >= 2) return Elevation;
                else if (ShoreDistance > 1) return Elevation - (2 - ShoreDistance) * ShoreDepth;
                else return Elevation - ShoreDepth;
            }
        }

        public GridValue NextValue(GridValue start, float distance)
        {
            float thisSlope = start.RiverSlope + LandSlope;
            float otherSlope = (start.RiverSlope + start.LandSlope);
            float slope = (thisSlope + otherSlope) / 2f;
            float deltaZ = slope * distance;

            float relativeRiverDistance = 2 * distance / start.RiverWidth;
            float relativeShoreDistance = 2 * distance / start.ShoreWidth;

            // Apply the first part of the distance to river width
            float remainingRiverDistance = Mathf.Max(1 - start.ShoreDistance, 0);
            float deltaDistance = Mathf.Min(remainingRiverDistance, relativeRiverDistance);
            // Apply the remaining part to shore width
            deltaDistance += relativeShoreDistance * (1 - deltaDistance / relativeRiverDistance);
            float newShoreDistance = Mathf.Min(2, start.ShoreDistance + deltaDistance);

            return new GridValue(
                landSlope: LandSlope,
                riverSlope: start.RiverSlope,
                riverWidth: start.RiverWidth,
                shoreWidth: start.ShoreWidth,
                shoreDepth: start.ShoreDepth,
                elevation: start.Elevation + deltaZ,
                shoreDistance: newShoreDistance);
        }

        public int CompareTo(object obj)
        {
            GridValue other = obj as GridValue;
            return Elevation.CompareTo(other.Elevation);
        }
    }
}
