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
        protected bool[,] _visited;
        private int _remaining;
        private Queue<GridNode> _ready;

        private MinHeap<GridNode> _pq;

        private List<GridNeighbor> _gridNeighbors;

        public ProgressiveDijkstra(GridValue[,] grid, float neighborRadius = 2, int blockSize = 110)
        {
            BlockSize = blockSize;

            Height = grid.GetLength(0);
            Width = grid.GetLength(1);

            _pq = new MinHeap<GridNode>();
            _ready = new Queue<GridNode>();
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

            _gridNeighbors = GridNeighbor.GetAll(neighborRadius);
        }

        public void Lock(int i, int j)
        {
            GridNode node = new GridNode(i, j, _grid[i, j]);
            if (_visited[node.I, node.J]) return;
            _iterate(node);
        }

        public bool GetReady(out int i, out int j, out GridValue value)
        {
            if (_ready.Count == 0)
            {
                i = -1;
                j = -1;
                value = default(GridValue);
                return false;
            }
            GridNode node = _ready.Dequeue();
            i = node.I;
            j = node.J;
            value = node.Value;
            return true;
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

        private void _iterate(GridNode node)
        {
            if (_visited[node.I, node.J]) return;
            _visit(node);
            foreach (GridNode neighbor in _getNeighbors(node))
            {
                _pq.Insert(neighbor);
            }
        }

        private IEnumerable<GridNode> _getNeighbors(GridNode node)
        {
            foreach (var neighbor in _gridNeighbors)
            {
                int i = node.I + neighbor.I;
                int j = node.J + neighbor.J;
                if (!_inBounds(i, j)) continue;
                if (_visited[i, j]) continue;
                GridValue otherValue = _grid[i, j];
                GridValue nextValue = otherValue.NextValue(node.Value, neighbor.Distance);
                GridNode nextNode = new GridNode(i, j, nextValue);
                yield return nextNode;

            }
        }

        protected virtual bool _inBounds(int i, int j)
        {
            return i >= 0 &&
                   j >= 0 &&
                   i < Height &&
                   j < Width;
        }

        private void _visit(GridNode node)
        {
            if (_visited[node.I, node.J]) return;
            _grid[node.I, node.J] = node.Value;
            _visited[node.I, node.J] = true;
            _ready.Enqueue(node);
            _remaining--;
        }
    }
}
