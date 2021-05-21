using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TerrainCalculator.Network
{
    public class Edge : QuikGraph.Edge<Node>
    {

        public bool IsLake { get => Path.IsLake; }
        public Path Path;
        public Vector2 SourceGrad;
        public Vector2 TargetGrad;

        public const int NumSegments = 30;
        public List<Vector2> InterpPoints;
        public List<Vector2> InterpLefts;
        public CumulativeLerp InterpTs;
        public float Distance;

        public Edge(Node source, Node target, Path path, Vector2 sourceGrad, Vector2 targetGrad)
            : base(source, target)
        {
            SourceGrad = sourceGrad;
            TargetGrad = targetGrad;
            Path = path;

            InterpPoints = new List<Vector2>();
            InterpLefts = new List<Vector2>();
            InterpTs = new CumulativeLerp();
            _compute();
        }

        public Vector3[] BuildVertices()
        {
            var vertices = new Vector3[4 * (NumSegments + 1)];
            for (int i = 0; i <= NumSegments; i++)
            {
                Vector3 position = _getPos(i);
                Vector3 left = _getLeft(i);
                Vector3 up = _getUp(i);

                List<Vector3> deltas = new List<Vector3>();
                deltas.Add(left);
                deltas.Add(up);
                // Repeat twice to get hard edge in the shader
                deltas.Add(up);
                deltas.Add(-left);

                for (int j = 0; j < 4; j++)
                {
                    vertices[i * 4 + j] = position + deltas[j];
                }
            }
            return vertices;
        }

        public int[] BuildTriangles()
        {
            // 4-56-7
            // |/||/|
            // 0-12-3
            int[] triangles = new int[NumSegments * 4 * 3];
            for (int row = 0; row < NumSegments; row++)
            {
                int j = row * 4;
                int k = row * 4 * 3;
                // Tri
                triangles[k + 0] = j + 0;
                triangles[k + 1] = j + 4;
                triangles[k + 2] = j + 5;
                // Tri
                triangles[k + 3] = j + 0;
                triangles[k + 4] = j + 5;
                triangles[k + 5] = j + 1;
                // Tri
                triangles[k + 6] = j + 2;
                triangles[k + 7] = j + 6;
                triangles[k + 8] = j + 7;
                // Tri
                triangles[k + 9] = j + 2;
                triangles[k + 10] = j + 7;
                triangles[k + 11] = j + 3;
            }
            return triangles;
        }

        public float InterpNodeValue(Lerp t, Node.Key key)
        {
            float startValue = Source.ImplicitValues[key].Value;
            float endValue = Target.ImplicitValues[key].Value;
            return t.Interp(startValue, endValue);
        }

        private void _compute()
        {
            InterpPoints.Clear();
            InterpLefts.Clear();
            InterpTs.Clear();

            Vector2 prev = Source.Pos;
            for (int i = 0; i <= NumSegments; i++)
            {
                float t = ((float)i / (float)NumSegments);
                Vector2 curr = _interpolate2d(t);
                InterpPoints.Add(curr);
                InterpLefts.Add(_interpolateLeft2d(t));
                float deltaDist = (curr - prev).magnitude;
                InterpTs.Add(deltaDist);
                prev = curr;
            }
            Distance = InterpTs.Total;
        }

        private Vector2 _interpolate2d(float t)
        {
            float t2 = t * t;
            float t3 = t2 * t;
            return (
                (2f * t3 - 3f * t2 + 1f) * Source.Pos +
                (t3 - 2f * t2 + t) * SourceGrad +
                (-2f * t3 + 3f * t2) * Target.Pos +
                (t3 - t2) * TargetGrad
            );
        }

        private Vector2 _interpolateLeft2d(float t)
        {
            float t2 = t * t;
            Vector2 forward = (
                (6f * t2 - 6f * t) * Source.Pos +
                (3 * t2 - 4 * t + 1) * SourceGrad +
                (-6f * t2 + 6f * t) * Target.Pos +
                (3 * t2 - 2 * t) * TargetGrad
            );
            forward.Normalize();
            return new Vector2(-forward.y, forward.x);
        }

        private Vector3 _getPos(int i)
        {
            Lerp t = InterpTs[i];
            Vector2 pos2 = InterpPoints[i];
            float elevation = InterpNodeValue(t, Node.Key.Elevation);
            return new Vector3(pos2.x, elevation, pos2.y);
        }

        private Vector3 _getLeft(int i)
        {
            Lerp t = InterpTs[i];
            Vector2 left2 = InterpLefts[i];
            float width = InterpNodeValue(t, Node.Key.ShoreWidth);
            if (!IsLake)
            {
                width += InterpNodeValue(t, Node.Key.RiverWidth);
            }
            float radius = width / 2;
            float elevation = InterpNodeValue(t, Node.Key.Elevation);
            return radius * new Vector3(left2.x, 0, left2.y);
        }

        private Vector3 _getUp(int i)
        {
            Lerp t = InterpTs[i];
            float depth = InterpNodeValue(t, Node.Key.ShoreDepth);
            return depth * Vector3.up;
        }
    }
}
