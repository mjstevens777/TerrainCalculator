using System;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainCalculator
{
    public class Lerp
    {
        public float T;
        


        public Lerp(float t)
        {
            T = t;
        }

        public float Interp(float start, float end)
        {
            return start * (1 - T) + end * T;
        }

        public Vector2 Interp(Vector2 start, Vector2 end)
        {
            return start * (1 - T) + end * T;
        }

        public Vector3 Interp(Vector3 start, Vector3 end)
        {
            return start * (1 - T) + end * T;
        }
    }

    public class CumulativeLerp
    {
        public float Total { get; private set; }
        private List<float> _checkpoints = new List<float>();

        public void Add(float dist)
        {
            Total += dist;
            _checkpoints.Add(Total);
        }

        public Lerp this[int index]
        {
            get
            {
                return new Lerp(_checkpoints[index] / Total);
            }
        }

        public int Count { get => _checkpoints.Count; }

        public void Clear()
        {
            Total = 0;
            _checkpoints.Clear();
        }
    }
}
