using System;
using System.Collections.Generic;
using ColossalFramework;
using ICities;
using TerrainCalculator.Grid;
using TerrainCalculator.UnityUI;
using UnityEngine;

namespace TerrainCalculator.Grid
{
    public class GridBuilder : MonoBehaviour
    {
        public ICities.ITerrain Terrain;
        public IResource Resource;
        private List<Segment> _segments;
        ProgressiveDijkstra<ZValue> _algorithm;
        private bool _wasDirty;

        public void Update()
        {
            var graphBuilder = GetComponent<GraphBuilder>();
            if (graphBuilder.Segments != _segments)
            {
                _segments = graphBuilder.Segments;
                _wasDirty = true;
                _algorithm = null;
            }
            else if (_wasDirty)
            {
                _wasDirty = false;
                if (_segments.Count > 0)
                {
                    _startCalculation();
                }
            }
            else if (_isCalculating)
            {
                _updateCalculation();
            }
        }

        private bool _isCalculating { get => _algorithm != null; }

        private void _startCalculation()
        {
            Debug.Log("Starting terrain calculation");
            

            int gridSize = 1081;
            float gridSpacing = Terrain.cellSize;

            ZValue[,] grid = new ZValue[gridSize, gridSize];
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    float landSlope = 4; // TODO: Look up from grid
                    float landSlopeRad = Mathf.Deg2Rad * landSlope;
                    grid[i, j] = new ZValue(
                        landSlope: Mathf.Tan(landSlopeRad) * gridSpacing,
                        riverSlope: 0,
                        elevation: 1024);
                }
            }
            _algorithm = new ProgressiveDijkstra<ZValue>(grid, neighborRadius: 1);

            Debug.Log("Drawing segments");
            foreach (var segment in _segments)
            {
                segment.RemapToTerrain((ITerrain)Terrain);
                foreach (var node in segment.Draw())
                {
                    int j = (int)node.Pos.x;
                    int i = (int)node.Pos.y;
                    if (i < 0 || j < 0) continue;
                    if (i >= 1081 || j >= 1081) continue;
                    float riverRad = node.RiverSlope * Mathf.Deg2Rad;

                    grid[i, j] = new ZValue(
                        landSlope: grid[i, j].LandSlope,
                        riverSlope: Mathf.Tan(riverRad) * gridSpacing,
                        elevation: node.Elevation);
                    _algorithm.Lock(i, j);
                }
            }
        }

        private void _updateCalculation()
        {
            Debug.Log($"Running terrain algorithm");
            bool algorithmDone = _algorithm.IterateMulti(10000);
            int minI, minJ, maxI, maxJ;
            bool blockFound = _algorithm.GetBlockReady(out minI, out minJ, out maxI, out maxJ);
            if (blockFound)
            {
                var tm = Singleton<TerrainManager>.instance;
                Debug.Log($"Updating area {minI} {minJ} {maxI} {maxJ}");
                Debug.Log($"Elevation at {minI} {minJ} = {_algorithm.Get(minI, minJ).Elevation}");
                for (int i = minI; i < maxI; i++)
                {
                    for (int j = minJ; j < maxJ; j++)
                    {
                        ZValue value = _algorithm.Get(i, j);
                        tm.RawHeights[i * 1081 + j] = Terrain.HeightToRaw(value.Elevation);
                    }
                }
                
                TerrainModify.UpdateArea(
                    minX: minJ - 2, minZ: minI - 2, maxX: maxJ + 2, maxZ: maxI + 2,
                    heights: true, surface: false, zones: false);
            }
            if (algorithmDone && !blockFound)
            {
                _abortCalculation();
            }
        }

        private void _abortCalculation()
        {
            Debug.Log("Finishing terrain calculation");
            _algorithm = null;
        }
    }
}