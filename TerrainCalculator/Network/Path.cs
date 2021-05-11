using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TerrainCalculator.Network
{
    public class Path
    {

        WaterNetwork _network;
        public const int NumSegments = 30;
        public bool IsDirty { get; set; }
        public List<Node> Nodes;
        private List<Vector2> _grads;

        public Path(WaterNetwork network)
        {
            _network = network;
            Nodes = new List<Node>();
            _grads = new List<Vector2>();
        }

        public virtual List<Edge> GetEdges()
        {
            _setDirections();
            List<Edge> edges = new List<Edge>();
            foreach (int i in Enumerable.Range(0, Nodes.Count))
            {
                int left = _wrapIndex(i);
                int right = _wrapIndex(i + 1);
                if (right < 0) { break; }

                List<Vector2> interp = new List<Vector2>();
                Vector2 current = Nodes[left].Pos;
                foreach (int j in Enumerable.Range(0, NumSegments + 1))
                {
                    float t = i + (j / (float)NumSegments);
                    interp.Add(_interpolate2d(t));
                }
                // NOTE: Bidirectionality handled in network class
                Edge edge = new Edge(Nodes[left], Nodes[right], interp, false);
                edges.Add(edge);
            }
            return edges;
        }

        public List<List<Node>> GetChains(Node.Key key)
        {
            List<List<Node>> chains = new List<List<Node>>();

            List<int> setIndices = new List<int>();
            foreach (int i in Enumerable.Range(0, Nodes.Count))
            {
                Node node = Nodes[i];
                if (node.ImplicitValues[key].IsSet)
                {
                    setIndices.Add(i);
                }
            }

            foreach (int start in setIndices)
            {
                List<Node> chain = new List<Node>();
                chain.Add(Nodes[start]);
                int i = start + 1;
                while (true)
                {
                    i = _wrapIndex(i);
                    if (i < 0) break;
                    Node node = Nodes[i];
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

        public List<List<Node>> GetEndpointChains(Node.Key key)
        {
            List<List<Node>> chains = new List<List<Node>>();

            int firstSet = -1;
            int lastSet = -1;

            for(int i = 0; i < Nodes.Count; i++)
            {
                Node node = Nodes[i];
                if (node.ImplicitValues[key].IsSet)
                {
                    if (firstSet == -1) firstSet = i;
                    lastSet = i;
                }
            }

            if (firstSet == -1 || lastSet == -1) throw new Exception("No values set for chain");

            if (firstSet > 0)
            {
                List<Node> chain = Nodes.GetRange(0, firstSet + 1);
                chain.Reverse();
                chains.Add(chain);
            }
            if (lastSet < Nodes.Count - 1)
            {
                List<Node> chain = Nodes.GetRange(lastSet, Nodes.Count - lastSet);
                chains.Add(chain);
            }

            return chains;
        }

        private Vector2 _interpolate2d(float t)
        {
            int i = Mathf.FloorToInt(t);
            t = t - i;
            int left = _wrapIndex(i);
            int right = _wrapIndex(i + 1);

            if (t == 0) { return Nodes[left].Pos; }
            if (right < 0) { throw new IndexOutOfRangeException("Path interpolation out of bounds"); }

            float t2 = t * t;
            float t3 = t2 * t;
            return (
                (2f * t3 - 3f * t2 + 1f) * Nodes[left].Pos +
                (t3 - 2f * t2 + t) * _grads[left] +
                (-2f * t3 + 3f * t2) * Nodes[right].Pos +
                (t3 - t2) * _grads[right]
            );
        }

        private void _setDirections()
        {
            _grads.Clear();
            for (int index = 0; index < Nodes.Count; index++)
            {
                _grads.Add(Vector2.zero);
                _setDirection(index);
            }
        }

        private void _setDirection(int index)
        {
            int left = _wrapIndex(index - 1);
            int right = _wrapIndex(index + 1);
            if (left < 0 || right < 0)
            {
                return;
            }
            _grads[index] = (Nodes[right].Pos - Nodes[left].Pos) / 2f;
        }

        protected virtual int _wrapIndex(int index)
        {
            if (index >= 0 && index < Nodes.Count)
            {
                return index;
            }
            else
            {
                return -1;
            }
        }
    }
}
