using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TerrainCalculator.Network
{
    public class Path : List<Node>
    {

        WaterNetwork _network;
        public const int NumSegments = 30;

        public Path(WaterNetwork network) : base()
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

        public virtual List<Edge> GetEdges()
        {
            _setDirections();
            List<Edge> edges = new List<Edge>();
            foreach (int i in Enumerable.Range(0, Count))
            {
                Node left = _getNodeInBounds(i);
                Node right = _getNodeInBounds(i + 1);
                if (right == null) { break; }

                List<Vector2> interp = new List<Vector2>();
                Vector2 current = left.Pos;
                foreach (int j in Enumerable.Range(0, NumSegments + 1))
                {
                    float t = i + (j / (float)NumSegments);
                    interp.Add(_interpolate2d(t));
                }
                Edge edge = new Edge(left, right, interp, false);
                edges.Add(edge);
            }
            return edges;
        }

        public List<List<Node>> GetChains(Node.ImplicitKey key)
        {
            List<List<Node>> chains = new List<List<Node>>();

            List<int> setIndices = new List<int>();
            foreach (int i in Enumerable.Range(0, Count))
            {
                Node node = this[i];
                if (node.ImplicitValues[key].IsSet)
                {
                    setIndices.Add(i);
                }
            }

            foreach (int start in setIndices)
            {
                List<Node> chain = new List<Node>();
                chain.Add(this[start]);
                int i = start + 1;
                while (true)
                {
                    Node node = _getNodeInBounds(i);
                    if (node == null) break;
                    chain.Add(node);
                    if (node.ImplicitValues[key].IsSet) break;
                    i++;
                }
                if (!chain[chain.Count - 1].ImplicitValues[key].IsSet) continue;
                if (chain.Count <= 2) continue;
                chains.Add(chain);
            }

            return chains;
        }

        public List<List<Node>> GetEndpointChains(Node.ImplicitKey key)
        {
            List<List<Node>> chains = new List<List<Node>>();

            int firstSet = -1;
            int lastSet = -1;

            foreach (int i in Enumerable.Range(0, Count))
            {
                Node node = this[i];
                if (node.ImplicitValues[key].IsSet)
                {
                    if (firstSet == -1) firstSet = i;
                    lastSet = i;
                }
            }

            if (firstSet == -1 || lastSet == -1) throw new Exception("No values set for chain");

            if (firstSet > 0)
            {
                List<Node> chain = GetRange(0, firstSet + 1);
                chain.Reverse();
                chains.Add(chain);
            }
            if (lastSet < Count - 1)
            {
                List<Node> chain = GetRange(lastSet, Count - lastSet);
                chains.Add(chain);
            }

            return chains;
        }

        private Vector2 _interpolate2d(float t)
        {
            int i = Mathf.FloorToInt(t);
            t = t - i;
            Node left = _getNodeInBounds(i);
            Node right = _getNodeInBounds(i + 1);

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

        private void _setDirections()
        {
            foreach(int i in Enumerable.Range(0, Count))
            {
                _setDirection(i);
            }
        }

        private void _setDirection(int index)
        {
            Node node = _getNodeInBounds(index);
            Node left = _getNodeInBounds(index - 1);
            Node right = _getNodeInBounds(index + 1);
            if (left == null || right == null)
            {
                node.Grad.Set(0f, 0f);
                return;
            }
            node.Grad = (right.Pos - left.Pos) / 2f;
        }

        protected virtual Node _getNodeInBounds(int index)
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
