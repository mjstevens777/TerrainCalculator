using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TerrainCalculator.Network
{
    public class Path : List<Node>
    {

        Network _network;
        const int NumSegments = 30;

        public Path(Network network) : base()
        {
            _network = network;
        }

        private bool _isDirty;
        public bool IsDirty
        {
            get
            {
                if (_isDirty) return true;
                foreach (Node node in this)
                {
                    if (node.IsDirty) return true;
                }
                return false;
            }
            set => _isDirty = value;
        }

        public Vector2 Interpolate2d(float t)
        {
            int i = Mathf.FloorToInt(t);
            t = t - i;
            Node left = getNodeInBounds(i);
            Node right = getNodeInBounds(i + 1);

            if (t == 0) { return left.Pos; }
            if (right == null) { throw new IndexOutOfRangeException("Path interpolation out of bounds"); }

            float t2 = t * t;
            float t3 = t2 * t;
            return (
                (2f * t3 - 3f * t2 + 1f) * left.Pos +
                (t3 - 2f * t2 + t) * left.Grad +
                (-2f * t3 + 3f * t2) * right.Pos +
                (t3 - t2) * right.Grad
            );
        }

        public void SetDirections()
        {
            foreach(int i in Enumerable.Range(0, Count))
            {
                SetDirection(i);
            }
        }

        private void SetDirection(int index)
        {
            Node node = getNodeInBounds(index);
            Node left = getNodeInBounds(index - 1);
            Node right = getNodeInBounds(index + 1);
            if (left == null || right == null)
            {
                node.Grad.Set(0f, 0f);
                return;
            }
            node.Grad = (right.Pos - left.Pos) / 2f;
        }

        protected virtual Node getNodeInBounds(int index)
        {
            if (index >= 0 && index < Count)
            {
                return this[index];
            }
            else
            {
                return null;
            }
        }
    }
}
