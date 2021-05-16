using System;
using TerrainCalculator.Grid;
using UnityEngine;

namespace TerrainCalculator.UnityUI
{
    public class GridBuilder : MonoBehaviour
    {


        private ProgressiveDijkstra<ZValue> algorithm;

        public bool IsDirty;
        private bool _wasDirty;
        private bool _isCalculating;

        public void Update()
        {
            if (IsDirty)
            {
                _updateFast();
                _abortCalculation();
                IsDirty = false;
                _wasDirty = true;
            } else if (_wasDirty)
            {
                _wasDirty = false;
                _isCalculating = true;
                _startCalculation();
            } else if (_isCalculating)
            {
                _isCalculating = _updateCalculation();
            }
        }

        public void _updateFast()
        {
            return;
        }

        public void _startCalculation()
        {

        }

        public bool _updateCalculation()
        {
            return false;
        }

        public void _abortCalculation()
        {

        }
    }
}
