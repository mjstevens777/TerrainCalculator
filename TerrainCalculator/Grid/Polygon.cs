using System;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainCalculator.Grid
{
    public class Polygon
    {
        List<Segment> _segments;
        int _minI, _minJ, _maxI, _maxJ;
        int _width { get => _maxJ - _minJ; }
        int _height { get => _maxI - _minI; }

        public Polygon(List<Segment> segments)
        {
            _segments = segments;
        }

        public int[,] Compute()
        {
            // NOTE: RemapToTerrain rounds coords so that we don't have to worry
            // about rounding errors here
            _minJ = (int)_segments[0].Start.Pos.x;
            _maxJ = _minJ;
            _minI = (int)_segments[0].Start.Pos.y;
            _maxI = _minI;
            foreach (var segment in _segments)
            {
                Vector2 pos = segment.Start.Pos;
                if (pos.x < _minJ) _minJ = (int)pos.x;
                if (pos.y < _minI) _minI = (int)pos.y;
                if (pos.x > _maxJ) _maxJ = (int)pos.x;
                if (pos.y > _maxI) _maxI = (int)pos.y;
            }
            _minI-=2; // buffer, rounding error
            _minJ-=2; // buffer, rounding error
            _maxI += 3; // buffer, rounding error, fencepost
            _maxJ += 3; // buffer, rounding error, fencepost

            int lakeCount = _width * _height; 

            bool[,] notLake = new bool[_height, _width];
            foreach (var segment in _segments)
            {
                foreach (var node in segment.Draw())
                {
                    int i = (int)node.Pos.y - _minI;
                    int j = (int)node.Pos.x - _minJ;
                    if (notLake[i, j]) continue;
                    notLake[i, j] = true;
                    lakeCount--;
                }
            }

            // Flood fill the outside. The rest is the inside.
            Stack<int[]> stack = new Stack<int[]>();
            stack.Push(new int[] { 0, 0 });
            while (stack.Count > 0)
            {
                int[] ij = stack.Pop();
                int i = ij[0];
                int j = ij[1];
                if (i < 0 || j < 0) continue;
                if (i >= _height || j >= _width) continue;
                if (notLake[i, j]) continue;
                notLake[i, j] = true;
                lakeCount--;
                // Four-connected
                stack.Push(new int[] { i + 1, j });
                stack.Push(new int[] { i, j + 1 });
                stack.Push(new int[] { i - 1, j });
                stack.Push(new int[] { i, j - 1 });
            }

            int[,] coords = new int[lakeCount, 2];
            int row = 0;
            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width; j++)
                {
                    if (notLake[i, j]) continue;
                    coords[row, 0] = i + _minI;
                    coords[row, 1] = j + _minJ;
                    row++;
                }
            }
            return coords;
        }
    }
}
