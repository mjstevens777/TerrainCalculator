using System;
using System.Collections.Generic;
using ColossalFramework;
using ICities;
using UnityEngine;

namespace TerrainCalculator.Grid
{
    public class TerrainSetter
    {
        public int NumBlocksI;
        public int NumBlocksJ;

        private TerrainBlock[,] _blocks;
        private int _gridSize;
        private int _blockSize;

        public TerrainSetter(int gridSize = 1081, int blockSize = 110)
        {
            _gridSize = gridSize;
            _blockSize = blockSize;
            NumBlocksI = Mathf.CeilToInt((float)_gridSize / (float)_blockSize);
            NumBlocksJ = NumBlocksI;
            _blocks = new TerrainBlock[NumBlocksI, NumBlocksJ];
            for (int i = 0; i < NumBlocksI; i++)
            {
                int minI = i * _blockSize;
                int maxI = Math.Min(_gridSize, (i + 1) * _blockSize);
                for (int j = 0; j < NumBlocksJ; j++)
                {
                    int minJ = j * _blockSize;
                    int maxJ = Math.Min(_gridSize, (j + 1) * _blockSize);
                    _blocks[i, j] = new TerrainBlock(
                        minI: minI, minJ: minJ,
                        maxI: maxI, maxJ: maxJ);

                }
            }
        }

        public void Reset()
        {
            foreach (var block in _allBlocks()) block.Reset();
        }

        public void Set(int i, int j, float value)
        {
            int blockI, blockJ;
            _blockIdx(i, j, out blockI, out blockJ);
            TerrainBlock block = _blocks[blockI, blockJ];
            block.Set(i, j, value);
        }

        public int PublishBlocks(ITerrain terrain, int maxCount = 1)
        {
            int publishedCount = 0;
            foreach(var block in ReadyBlocks())
            {
                block.Publish(terrain);
                publishedCount++;
                if (publishedCount == maxCount) return publishedCount;
            }
            return publishedCount;
        }

        public IEnumerable<TerrainBlock> ReadyBlocks()
        {
            foreach (var block in _allBlocks())
            {
                if (!block.IsReady) continue;
                yield return block;
            }
        }

        public void SetAll(float value)
        {
            foreach (var block in _allBlocks()) block.SetAll(value);
        }

        private IEnumerable<TerrainBlock> _allBlocks()
        {
            for (int i = 0; i < NumBlocksI; i++)
            {
                for (int j = 0; j < NumBlocksJ; j++)
                {
                    yield return _blocks[i, j];
                }
            }
        }

        private void _blockIdx(int i, int j, out int blockI, out int blockJ)
        {
            blockI = i / _blockSize;
            blockJ = j / _blockSize;
        }
    }

    public class TerrainBlock
    {
        public int MinI, MinJ, MaxI, MaxJ;
        public int Height { get => MaxI - MinI; }
        public int Width { get => MaxJ - MinJ; }
        public bool IsReady { get => _remainingCount == 0 && !IsPublished; }
        public bool IsPublished { get; private set; }

        private bool[,] _updated;
        private float[,] _values;
        private int _remainingCount;


        public TerrainBlock(int minI, int minJ, int maxI, int maxJ)
        {
            MinI = minI;
            MinJ = minJ;
            MaxI = maxI;
            MaxJ = maxJ;
            Reset();
        }

        public void Reset()
        {
            _values = new float[Height, Width];
            _updated = new bool[Height, Width];
            _remainingCount = Height * Width;
            IsPublished = false;
        }

        public void Set(int i, int j, float value)
        {
            if (IsPublished)
            {
                Debug.Log($"Value set after publishing at {i} {j} = {value}");
            }
            int di = i - MinI;
            int dj = j - MinJ;
            _values[di, dj] = value;
            if (_updated[di, dj]) return;
            _updated[di, dj] = true;
            _remainingCount--;

        }

        public void SetAll(float value)
        {
            for (int di = 0; di < Height; di++)
            {
                for (int dj = 0; dj < Width; dj++)
                {
                    _values[di, dj] = value;
                    _updated[di, dj] = true;
                }
            }
            _remainingCount = 0;
        }

        public void Publish(ITerrain terrain)
        {
            IsPublished = true;
            if (terrain == null) return;
            int gridSize = terrain.heightMapResolution + 1;
            var tm = Singleton<TerrainManager>.instance;
            if (tm == null)
            {
                Debug.Log($"Could not find TerrainManager");
                return;
            }
            for (int di = 0; di < Height; di++)
            {
                for (int dj = 0; dj < Width; dj++)
                {
                    int i = MinI + di;
                    int j = MinJ + dj;
                    ushort value = terrain.HeightToRaw(_values[di, dj]);
                    tm.RawHeights[i * gridSize + j] = value;
                }
            }
            TerrainModify.UpdateArea(
                minX: MinJ - 2, minZ: MinI - 2, maxX: MaxJ + 2, maxZ: MaxI + 2,
                heights: true, surface: false, zones: false);
        }
    }
}
