using System;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainCalculator.Grid
{
    public interface IGridValue<T> : IComparable
    {
        T NextValue(T startValue, float distance);
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

        public int Width { get; private set; }
        public int Height { get; private set; }
        public int BlockSize { get; private set; }
        public int BlockWidth { get => Width / BlockSize + 1; }
        public int BlockHeight { get => Height / BlockSize + 1; }
        protected GridValue[,] _grid;
        private int[,] _blockRemainingCount;
        private bool[,] _blockVisited;
        protected bool[,] _visited;
        private int _remaining;

        private MinHeap<GridNode> _pq;

        public float NeighborRadius;

        public ProgressiveDijkstra(GridValue[,] grid, float neighborRadius = 2, int blockSize = 110)
        {
            NeighborRadius = neighborRadius;
            BlockSize = blockSize;

            Height = grid.GetLength(0);
            Width = grid.GetLength(1);

            _pq = new MinHeap<GridNode>();
            _grid = grid;
            _visited = new bool[Height, Width];
            _remaining = Height * Width;
            _blockRemainingCount = new int[BlockHeight, BlockWidth];
            _blockVisited = new bool[BlockHeight, BlockWidth];

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (_grid[i, j] == null) throw new ArgumentNullException("All grid values must be set");
                    _blockRemainingCount[i / BlockSize, j / BlockSize]++;
                }
            }
        }

        public void Lock(int i, int j)
        {
            GridNode node = new GridNode(i, j, _grid[i, j]);
            if (_isVisited(node)) return;
            _iterate(node);
        }

        public GridValue Get(int i, int j)
        {
            return _grid[i, j];
        }

        public bool IterateMulti(int numIterations)
        {
            bool isDone = false;
            for (int i = 0; i < numIterations; i++)
            {
                isDone = Iterate();
                if (isDone) return isDone;
            }
            return isDone;
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

        public bool GetBlockReady(out int minI, out int minJ, out int maxI, out int maxJ)
        {
            for (int bi = 0; bi < BlockHeight; bi++)
            {
                for (int bj = 0; bj < BlockWidth; bj++)
                {
                    if (_blockVisited[bi, bj]) continue;
                    if (_blockRemainingCount[bi, bj] > 0) continue;
                    minI = bi * BlockSize;
                    maxI = Math.Min((bi + 1) * BlockSize, Height);
                    minJ = bj * BlockSize;
                    maxJ = Math.Min((bj + 1) * BlockSize, Width);
                    _blockVisited[bi, bj] = true;
                    return true;
                }
            }
            minI = -1;
            minJ = -1;
            maxI = -1;
            maxJ = -1;
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
            int low = Mathf.FloorToInt(-NeighborRadius);
            int high = Mathf.CeilToInt(NeighborRadius);
            for (int di = low; di <= high; di++)
            {
                for (int dj = low; dj <= high; dj++)
                {
                    if (di == 0 && dj == 0) continue;
                    int i = node.I + di;
                    int j = node.J + dj;
                    if (!_inBounds(i, j)) continue;
                    float distance = Mathf.Sqrt(di * di + dj * dj);
                    if (distance > NeighborRadius) continue;
                    GridValue otherValue = _grid[i, j];
                    GridValue nextValue = otherValue.NextValue(node.Value, distance);
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
            _blockRemainingCount[node.I / BlockSize, node.J / BlockSize]--;
        }
    }
}
