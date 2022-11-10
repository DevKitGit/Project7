using System.Collections.Generic;
using UnityEngine;

namespace AnimationSystem.Splines
{
    [RequireComponent(typeof(MeshFilter))]
    public class SplineVisualizer : MonoBehaviour
    {
        [SerializeField] private Mesh2D _shape2D = default;
        [SerializeField] private BezierSpline _bezierSpline = default;
        [SerializeField] [Range(2, 100)] private int _segmentCount = 15;
        [SerializeField] [Range(0.001f, 2f)] private float _scaleFactor = 1f;

        private Mesh _mesh;
        private Vector3 _lastChangedPosition;
        private Quaternion _lastChangedRotation;

        private void OnEnable()
        {
            if (_bezierSpline == null)
            {
                _bezierSpline = this.GetComponent<BezierSpline>();
            }

            _bezierSpline.SplineChangedEvent += OnSplineChangedEvent;
        }

        private void OnDisable()
        {
            _bezierSpline.SplineChangedEvent -= OnSplineChangedEvent;
        }

        private void Awake()
        {
            _mesh = new Mesh();
            _mesh.name = "SplinePathSegment";

            this.GetComponent<MeshFilter>().sharedMesh = _mesh;
        }

        private void LateUpdate()
        {
            if (this.transform.position != _lastChangedPosition || this.transform.rotation != _lastChangedRotation)
            {
                GenerateMesh();

                _lastChangedPosition = this.transform.position;
                _lastChangedRotation = this.transform.rotation;
            }
        }

        private void GenerateMesh()
        {
            _mesh.Clear();

            // Vertices
            var uSpan = _shape2D.CalculateUspan();
            var splineLength = GetApproximateLength();
            var splinePartCount = _segmentCount * _bezierSpline.CurveCount;

            var verts = new List<Vector3>();
            var normals = new List<Vector3>();
            var uvs = new List<Vector2>();
            for (var ringIndex = 0; ringIndex < splinePartCount; ringIndex++)
            {
                
                var splineProgress = ringIndex / (splinePartCount - 1f);
                var point = _bezierSpline.GetBezierPoint(splineProgress);
                var vCoordinate = splineProgress * splineLength / uSpan;

                for (var vertIndex = 0; vertIndex < _shape2D.VertexCount; vertIndex++)
                {
                    var localPoint = _shape2D.Vertices[vertIndex].Point * _scaleFactor;

                    verts.Add(point.LocalToWorldPosition(localPoint) - this.transform.position);
                    normals.Add(point.LocalToWorldVector(_shape2D.Vertices[vertIndex].Normal));

                    uvs.Add(new Vector2(_shape2D.Vertices[vertIndex].U, vCoordinate));
                }
            }

            // Triangles
            var triIndices = new List<int>();
            for (var ringIndex = 0; ringIndex < splinePartCount - 1; ringIndex++)
            {
                var rootIndex = ringIndex * _shape2D.VertexCount;
                var rootIndexNext = (ringIndex + 1) * _shape2D.VertexCount;

                for (var lineIndex = 0; lineIndex < _shape2D.LineCount; lineIndex += 2)
                {
                    var lineIndexA = _shape2D.LineIndices[lineIndex];
                    var lineIndexB = _shape2D.LineIndices[lineIndex + 1];

                    var currentA = rootIndex + lineIndexA;
                    var currentB = rootIndex + lineIndexB;
                    var nextA = rootIndexNext + lineIndexA;
                    var nextB = rootIndexNext + lineIndexB;

                    triIndices.Add(currentA);
                    triIndices.Add(nextA);
                    triIndices.Add(nextB);

                    triIndices.Add(currentA);
                    triIndices.Add(nextB);
                    triIndices.Add(currentB);
                }
            }

            _mesh.SetVertices(verts);
            _mesh.SetNormals(normals);
            _mesh.SetUVs(0, uvs);
            _mesh.SetTriangles(triIndices, 0);
        }

        // Get approximate length of a bezier curve/spline
        private float GetApproximateLength(int precision = 8)
        {
            // We're splitting the spline into <precision> number of segments
            var points = new Vector3[precision];
            for (var i = 0; i < precision; i++)
            {
                var t = i / (precision - 1f);
                points[i] = _bezierSpline.GetPosition(t);
            }

            // Summing up the segment lengths
            var distance = 0f;
            for (var i = 0; i < precision - 1; i++)
            {
                var a = points[i];
                var b = points[i + 1];

                distance += Vector3.Distance(a, b);
            }

            return distance;
        }

        private void OnSplineChangedEvent()
        {
            GenerateMesh();
        }
    }
}