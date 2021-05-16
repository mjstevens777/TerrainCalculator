using System;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainCalculator.Grid
{
    public class DrawLine
    {
        public static IEnumerable<Vector3> draw(Vector2 start, Vector2 end)
        {
            float dx = Mathf.Abs(start.x - end.x);
            float dy = Mathf.Abs(start.y - end.y);

            float tStart, tEnd;
            int numSteps;

            if (dx > dy)
            {
                int x1 = Mathf.RoundToInt(start.x);
                int x2 = Mathf.RoundToInt(end.x);
                tStart = ((float)x1 - start.x) / (end.x - start.x);
                tEnd = ((float)x2 - start.x) / (end.x - start.x);
                numSteps = Math.Abs(x2 - x1);
            } else
            {
                int y1 = Mathf.RoundToInt(start.y);
                int y2 = Mathf.RoundToInt(end.y);
                tStart = ((float)y1 - start.y) / (end.y - start.y);
                tEnd = ((float)y2 - start.y) / (end.y - start.y);
                numSteps = Math.Abs(y2 - y1);
            }

            float tStep;
            if (numSteps > 0)
            {
                tStep = (tEnd - tStart) / numSteps;
            } else {
                tStart = 0.5f;
                tStep = 0;
            }

            for (int i = 0; i <= numSteps; i++)
            {
                float t = tStart + tStep * i;
                Vector2 interpXY = (1 - t) * start + t * end;
                Vector3 interp = new Vector3(interpXY.x, interpXY.y, t);
                interp.x = Mathf.Round(interp.x);
                interp.y = Mathf.Round(interp.y);
                yield return interp;
            }
        }
    }
}
