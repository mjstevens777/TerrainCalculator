using System;
using System.Collections.Generic;
using ColossalFramework;
using ICities;
using TerrainCalculator.Grid;
using TerrainCalculator.UnityUI;
using UnityEngine;

namespace TerrainCalculator.Grid
{
    public class GridBuilder
    {
        private IManagers _managers;
        private ITerrain _terrain { get => _managers.terrain; }
        private int _gridSize { get => _terrain.heightMapResolution + 1; }
        private float _gridSpacing { get => _terrain.cellSize; }
        private IResource _resource { get => _managers.resource; }
        private GraphBuilder _graphBuilder;
        private List<List<Segment>> _segments;
        private ProgressiveDijkstra<GridValue> _algorithm;
        private TerrainSetter _terrainSetter;

        public GridBuilder(IManagers managers)
        {
            _managers = managers;
            _terrainSetter = new TerrainSetter();
            _abortCalculation();
            _abortSetToZero();
        }

        public void Tick()
        {
            _findGraphBuilder();
            if (_graphBuilder == null) return;
            bool graphDirty = _graphBuilder.Segments != _segments;
            bool resourceDirty = ResourceExtension.IsDirty;
            if (_isSettingToZero)
            {
                _updateSetToZero();
            }
            else if (!_graphBuilder.IsStable || resourceDirty)
            {
                _startSetToZero();

            }
            else if (graphDirty)
            {
                _startCalculation();
            }
            else if (_isCalculating)
            {
                _updateCalculation();
            }
        }

        private void _findGraphBuilder()
        {
            if (_graphBuilder != null) return;
            var go = GameObject.Find(LoadingExtension.ObjectName);
            if (go == null)
            {
                Debug.Log("Could not find GameObject TerrainCalculator");
                return;
            }
            _graphBuilder = go.GetComponent<GraphBuilder>();
            if (_graphBuilder == null)
            {
                Debug.Log("Could not find Component GraphBuilder");
                return;
            }
            Debug.Log("Found GraphBuilder");
        }

        private bool _isSettingToZero { get => _isSetToZeroStarted && !_isSetToZeroCompleted; }
        private bool _isSetToZeroStarted;
        private bool _isSetToZeroCompleted;


        private void _startSetToZero()
        {
            Debug.Log("Start setting to zero");
            _terrainSetter.Reset();
            _terrainSetter.SetAll(40);
            _isSetToZeroStarted = true;
            _isSetToZeroCompleted = false;
            ResourceExtension.IsDirty = false;
        }

        private void _abortSetToZero()
        {
            _isSetToZeroStarted = false;
            _isSetToZeroCompleted = false;
        }

        private void _updateSetToZero()
        {
            Debug.Log("Setting to zero");
            int numPublished = _terrainSetter.PublishBlocks(_terrain, _terrainSetter.NumBlocksJ);
            if (numPublished == 0) _isSetToZeroCompleted = true;
        }

        private bool _isCalculating { get => _algorithm != null; }

        private void _startCalculation()
        {
            Debug.Log("Starting terrain calculation");
            _segments = _graphBuilder.Segments;
            ResourceExtension.IsDirty = false;
            _abortCalculation();
            _abortSetToZero();
            if (_segments.Count == 0) return;

            //var tm = Singleton<TerrainManager>.instance;
            //tm.RenderTopographyInfo = true;

            GridValue[,] grid = new GridValue[_gridSize, _gridSize];
            float[,] slopeGrid = _getSlopeGrid();
            for (int i = 0; i < _gridSize; i++)
            {
                for (int j = 0; j < _gridSize; j++)
                {
                    float landSlope = _getSlopeFromGrid(slopeGrid, i, j);
                    float landSlopeRad = Mathf.Deg2Rad * landSlope;
                    grid[i, j] = new GridValue(
                        landSlope: Mathf.Tan(landSlopeRad) * _gridSpacing,
                        elevation: 1024,
                        shoreDistance: 2);
                }
            }
            _algorithm = new ProgressiveDijkstra<GridValue>(grid, neighborRadius: 3);

            Debug.Log("Drawing segments");
            foreach (var pathSegments in _segments)
            {
                _drawPath(grid, pathSegments);
            }
        }

        private void _drawPath(GridValue[,] grid, List<Segment> segments)
        {
            if (segments.Count == 0) return;

            foreach (var segment in segments)
            {
                segment.RemapToTerrain((ITerrain)_terrain);
                foreach (var node in segment.Draw())
                {
                    int j = (int)node.Pos.x;
                    int i = (int)node.Pos.y;
                    if (i < 0 || j < 0) continue;
                    if (i >= 1081 || j >= 1081) continue;
                    float riverRad = node.RiverSlope * Mathf.Deg2Rad;

                    grid[i, j] = new GridValue(
                        landSlope: grid[i, j].LandSlope,
                        riverSlope: Mathf.Tan(riverRad) * _gridSpacing,
                        riverWidth: node.RiverWidth / _gridSpacing,
                        shoreWidth: node.ShoreWidth / _gridSpacing,
                        shoreDepth: node.ShoreDepth,
                        elevation: node.Elevation,
                        shoreDistance: segment.IsLake ? 1 : 0);
                    _algorithm.Lock(i, j);
                }
            }

            if (segments[0].IsLake) _fillLake(grid, segments);
        }

        private void _fillLake(GridValue[,] grid, List<Segment> segments)
        {
            Polygon polygon = new Polygon(segments);
            int[,] coords = polygon.Compute();
            SegmentNode node = segments[0].Start;
            for (int row = 0; row < coords.GetLength(0); row++)
            {
                int i = coords[row, 0];
                int j = coords[row, 1];
                if (i < 0 || j < 0) continue;
                if (i >= _gridSize || j >= _gridSize) continue;

                float riverRad = node.RiverSlope * Mathf.Deg2Rad;
                grid[i, j] = new GridValue(
                    landSlope: grid[i, j].LandSlope,
                    riverSlope: Mathf.Tan(riverRad) * _gridSpacing,
                    riverWidth: node.RiverWidth / _gridSpacing,
                    shoreWidth: node.ShoreWidth / _gridSpacing,
                    shoreDepth: node.ShoreDepth,
                    elevation: node.Elevation,
                    shoreDistance: 1);
                _algorithm.Lock(i, j);
            }
        }

        private void _updateCalculation()
        {
            Debug.Log($"Running terrain algorithm");
            bool algorithmDone = _algorithm.IterateMulti(50000);

            while (true)
            {
                int i, j;
                GridValue value;
                bool foundNode = _algorithm.GetReady(out i, out j, out value);
                if (!foundNode) break;
                _terrainSetter.Set(i, j, value.FinalElevation);
            }

            int blocksPublished = _terrainSetter.PublishBlocks(_terrain);
            if (algorithmDone && blocksPublished == 0)
            {
                _abortCalculation();
            }
        }

        private void _abortCalculation()
        {
            Debug.Log("Finishing terrain calculation");
            _algorithm = null;
            _terrainSetter.Reset();
        }

        private float[,] _getSlopeGrid()
        {
            int size = _resource.gridResolution;
            float[,] slope = new float[size, size];
            float oilSlope = 1;
            float baseSlope = 4;
            float fertileSlope = 10;
            float oreSlope = 30;
            for (int z = 0; z < size; z++)
            {
                for (int x = 0; x < size; x++)
                {
                    float fertility = (float)_resource.GetResource(x, z, NaturalResource.Fertility) / 255f;
                    float oil = (float)_resource.GetResource(x, z, NaturalResource.Oil) / 255f;
                    float ore = (float)_resource.GetResource(x, z, NaturalResource.Ore) / 255f;
                    slope[z, x] = (
                        baseSlope +
                        (oilSlope - baseSlope) * oil +
                        (fertileSlope - baseSlope) * fertility +
                        (oreSlope - fertileSlope) * ore);
                }
            }
            return slope;
        }

        private float _getSlopeFromGrid(float[,] grid, int i, int j)
        {
            float x, z;
            _terrain.HeightMapCoordToPosition(j, i, out x, out z);
            int resX, resZ;
            _resource.WorldToGridPosition(new Vector3(x, 0, z), out resX, out resZ);
            resX = Math.Min(Math.Max(resX, 0), grid.GetLength(1) - 1);
            resZ = Math.Min(Math.Max(resZ, 0), grid.GetLength(0) - 1);
            return grid[resZ, resX];
        }
    }
}