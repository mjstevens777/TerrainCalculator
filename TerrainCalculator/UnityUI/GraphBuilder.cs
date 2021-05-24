using System;
using System.Collections.Generic;
using TerrainCalculator.Network;
using TerrainCalculator.Grid;
using UnityEngine;

namespace TerrainCalculator.UnityUI
{

    public class GraphBuilder : MonoBehaviour
    {
        private WaterNetwork _net;

        public bool IsDirty;
        public bool IsStable;
        public List<List<Segment>> Segments;

        public void Start()
        {
            Debug.Log("GraphBuilder start");
            _net = GetComponent<State>().Net;
        }

        public void Update()
        {
            if (!IsDirty)
            {
                IsStable = true;
                return;
            }
            IsStable = false;
            IsDirty = false;
            _net.InterpolateAll();
            _copyNetwork();
        }

        private void _copyNetwork()
        {
            List<List<Segment>> newSegments = new List<List<Segment>>();
            foreach (var edges in _net.EdgesByPath)
            {
                List<Segment> pathSegments = new List<Segment>();
                foreach (var edge in edges)
                {
                    List<SegmentNode> edgeNodes = new List<SegmentNode>();
                    for (int i = 0; i < edge.InterpPoints.Count; i++)
                    {
                        Lerp t = edge.InterpTs[i];
                        SegmentNode node = new SegmentNode(
                            edge.InterpPoints[i],
                            elevation: edge.InterpNodeValue(t, Node.Key.Elevation),
                            riverSlope: edge.InterpNodeValue(t, Node.Key.RiverSlope),
                            riverWidth: edge.InterpNodeValue(t, Node.Key.RiverWidth),
                            shoreDepth: edge.InterpNodeValue(t, Node.Key.ShoreDepth),
                            shoreWidth: edge.InterpNodeValue(t, Node.Key.ShoreWidth));
                        edgeNodes.Add(node);
                    }

                    for (int i = 0; i < edgeNodes.Count - 1; i++)
                    {
                        pathSegments.Add(new Segment(edgeNodes[i], edgeNodes[i + 1], edge.IsLake));
                    }
                }
                newSegments.Add(pathSegments);
            }

            Segments = newSegments;
        }
    }
}
