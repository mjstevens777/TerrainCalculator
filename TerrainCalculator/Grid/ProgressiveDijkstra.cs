using System;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainCalculator.Grid
{
    public interface IGridValue<T> : IComparable
    {
        T NextValue(T startValue, int di, int dj);
    }

    public class ProgressiveDijkstra<GridValue> where GridValue : IGridValue<GridValue>
    {
        
        protected class GridNode : IComparable
        {
            public int I;
            public int J;
            public GridValue Value;

            public GridNode(int i, int j, GridValue value)
            {
                I = i;
                J = j;
                Value = value;
            }

            public int CompareTo(object obj)
            {
                var other = obj as GridNode;
                return Value.CompareTo(other.Value);
            }
        }

        public int Width;
        public int Height;
        protected GridValue[,] _grid;
        protected bool[,] _visited;
        private int _remaining;

        private MinHeap<GridNode> _pq;

        public int NumIterations;
        public int NeighborRadius;

        public ProgressiveDijkstra(int numIterations = 30, int neighborRadius = 2)
        {
            NumIterations = numIterations;
            NeighborRadius = neighborRadius;
        }

        public void Reset(GridValue[,] grid)
        {
            Height = grid.GetLength(0);
            Width = grid.GetLength(1);

            _pq = new MinHeap<GridNode>();
            _grid = grid;
            _visited = new bool[Height, Width];
            _remaining = Height * Width;

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (_grid[i, j] == null) throw new ArgumentNullException("All grid values must be set");
                }
            }
        }

        public void Lock(int i, int j)
        {
            GridNode node = new GridNode(i, j, _grid[i, j]);
            if (_isVisited(node)) throw new ArgumentException("Node already visited");
            _iterate(node);
        }

        public GridValue Get(int i, int j)
        {
            return _grid[i, j];
        }

        public bool Iterate()
        {
            if (_remaining == 0)
            {
                _pq.Clear();  // Free up some memory
            }
            if (_pq.Size == 0) return true;
            GridNode node = _pq.Extract();
            _iterate(node);
            return false;
        }

        private void _iterate(GridNode node)
        {
            if (_isVisited(node)) return;
            _visit(node);
            foreach (GridNode neighbor in _getNeighbors(node))
            {
                _pq.Insert(neighbor);
            }
        }

        private IEnumerable<GridNode> _getNeighbors(GridNode node)
        {
            for (int di = -NeighborRadius; di <= NeighborRadius; di++)
            {
                for (int dj = -NeighborRadius; dj <= NeighborRadius; dj++)
                {
                    if (di == 0 && dj == 0) continue;
                    int i = node.I + di;
                    int j = node.J + dj;
                    if (!_inBounds(i, j)) continue;
                    GridValue otherValue = _grid[i, j];
                    GridValue nextValue = otherValue.NextValue(node.Value, di, dj);
                    GridNode nextNode = new GridNode(
                        node.I + di, node.J + dj, nextValue);
                    yield return nextNode;
                }
            }
        }

        protected virtual bool _inBounds(int i, int j)
        {
            return i >= 0 &&
                   j >= 0 &&
                   i < Height &&
                   j < Width;
        }

        private bool _isVisited(GridNode node) => _visited[node.I, node.J];

        private void _visit(GridNode node)
        {
            if (_visited[node.I, node.J]) return;
            _grid[node.I, node.J] = node.Value;
            _visited[node.I, node.J] = true;
            _remaining--;
        }
    }
}
