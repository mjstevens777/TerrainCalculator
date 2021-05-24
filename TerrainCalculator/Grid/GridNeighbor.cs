using System;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainCalculator.Grid
{


    public class GridNeighbor
    {
        public int I;
        public int J;
        public float Distance;

        public GridNeighbor(int i, int j, float dist)
        {
            I = i;
            J = j;
            Distance = dist;
        }

        public static List<GridNeighbor> GetAll(float radius)
        {
            List<GridNeighbor> neighbors = new List<GridNeighbor>();

            int low = Mathf.FloorToInt(-radius);
            int high = Mathf.CeilToInt(radius);
            for (int di = low; di <= high; di++)
            {
                for (int dj = low; dj <= high; dj++)
                {
                    if (di == 0 && dj == 0) continue;
                    float distance = Mathf.Sqrt(di * di + dj * dj);
                    if (distance > radius) continue;
                    // We can decompose a step of (2, 2) into two steps (1, 1)
                    // Use the GCF to find the general case when a step is redundant
                    if (_gcf(di, dj) > 1) continue;
                    neighbors.Add(new GridNeighbor(di, dj, distance));
                }
            }
            return neighbors;
        }

        private static int _gcf(int a, int b)
        {
            a = Math.Abs(a);
            b = Math.Abs(b);
            if (a == 0) return b;
            if (b == 0) return a;
            // https://stackoverflow.com/a/41766138/5945112
            while (a != 0 && b != 0)
            {
                if (a > b)
                    a %= b;
                else
                    b %= a;
            }

            return a | b;
        }
    }
}
