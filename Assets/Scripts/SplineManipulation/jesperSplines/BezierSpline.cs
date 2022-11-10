using System;
using UnityEngine;

namespace AnimationSystem.Splines
{
    public class BezierSpline : MonoBehaviour
	{
		public event Action SplineChangedEvent; 

		public int SplinePointCount
		{
			get
			{
				return _allSplinePoints != null ? _allSplinePoints.Length : 0;
			}
		}

		public int CurveCount
		{
			get
			{
				return _allSplinePoints != null ? (_allSplinePoints.Length - 1) / 3 : 0;
			}
		}

		public bool Loop
		{
			get
			{
				return _loop;
			}
			set
			{
				_loop = value;

				if (Application.isPlaying && _loop && _allSplinePoints != null && _allSplinePoints.Length > 0)
				{
					SetSplinePoint(0, _allSplinePoints[0]);
				}
			}
		}

		[SerializeField] private bool _useAlternativeControlPointGenerator = false;

		private Vector3[] _anchorPoints;
		private Vector3[] _allSplinePoints;
		private bool _loop = false;
		private Vector3 _lastChangedPosition;
		private Quaternion _lastChangedRotation;

		private void LateUpdate()
		{
			if (this.transform.position != _lastChangedPosition || this.transform.rotation != _lastChangedRotation)
			{
				GenerateSpline(_anchorPoints);

				_lastChangedPosition = this.transform.position;
				_lastChangedRotation = this.transform.rotation;
			}
		}

		public void GenerateSpline(Vector3[] anchorPoints)
		{
			if (anchorPoints.Length < 2)
            {
				Debug.LogError("BezierSpline: Cannot generate a spline with less than 2 anchor points", this);
				return;
            }

			_anchorPoints = anchorPoints;

			if (_useAlternativeControlPointGenerator)
			{
                GenerateSplinePointsAlternative();
            }
			else
			{
                GenerateSplinePoints(); 
            }

            TransformSplineToLocalSpace();

			SplineChangedEvent?.Invoke();
		}

		public Vector3[] GetAnchorPoints()
        {
			return _anchorPoints;
		}

		public Vector3 GetSplinePoint(int index)
		{
			return _allSplinePoints[index];
		}

		public void SetSplinePoint(int index, Vector3 position)
		{
			if (index % 3 == 0)
			{
				var delta = position - _allSplinePoints[index];
				if (_loop)
				{
					if (index == 0)
					{
						_allSplinePoints[1] += delta;
						_allSplinePoints[_allSplinePoints.Length - 2] += delta;
						_allSplinePoints[_allSplinePoints.Length - 1] = position;
					}
					else if (index == _allSplinePoints.Length - 1)
					{
						_allSplinePoints[0] = position;
						_allSplinePoints[1] += delta;
						_allSplinePoints[index - 1] += delta;
					}
					else
					{
						_allSplinePoints[index - 1] += delta;
						_allSplinePoints[index + 1] += delta;
					}
				}
				else
				{
					if (index > 0)
					{
						_allSplinePoints[index - 1] += delta;
					}

					if (index + 1 < _allSplinePoints.Length)
					{
						_allSplinePoints[index + 1] += delta;
					}
				}
			}

			_allSplinePoints[index] = position;
			EnforceMode(index);
		}

		public BezierPoint GetBezierPoint(float t)
		{
			return new BezierPoint(GetPosition(t), GetDirection(t));
		}

		public Vector3 GetPosition(float t)
		{
			var i = 0;
			if (t >= 1f)
			{
				t = 1f;
				i = _allSplinePoints.Length - 4;
			}
			else
			{
				t = Mathf.Clamp01(t) * CurveCount;
				i = (int)t;
				t -= i;
				i *= 3;
			}

			return GetWorldPosition(Bezier.GetPoint(_allSplinePoints[i], _allSplinePoints[i + 1], _allSplinePoints[i + 2], _allSplinePoints[i + 3], t));
		}

		public Vector3 GetVelocity(float t)
		{
			var i = 0;
			if (t >= 1f)
			{
				t = 1f;
				i = _allSplinePoints.Length - 4;
			}
			else
			{
				t = Mathf.Clamp01(t) * CurveCount;
				i = (int)t;
				t -= i;
				i *= 3;
			}

			return GetWorldPosition(Bezier.GetFirstDerivative(_allSplinePoints[i], _allSplinePoints[i + 1], _allSplinePoints[i + 2], _allSplinePoints[i + 3], t)) - this.transform.position;
		}

		public Vector3 GetDirection(float t)
		{
			return GetVelocity(t).normalized;
		}

        // Original solution inspired by https://github.com/yasirkula/UnityBezierSolution/blob/master/Plugins/BezierSolution
        private void GenerateSplinePoints()
		{
			var n = _anchorPoints.Length - 1;
			if (n == 1)
			{
				// Special case, Bezier curve should be a straight line
				var firstPoint = _anchorPoints[0];
				var secondPoint = _anchorPoints[1];
				var firstControlPoint = (4f * firstPoint + secondPoint) / 3f;
				var secondControlPoint = (2f * firstPoint + secondPoint) / 3f;

				_allSplinePoints = new Vector3[] {
					firstPoint,
					firstControlPoint,
					secondControlPoint,
					secondPoint
				};

				return;
			}

			var splinePointCount = 3 * _anchorPoints.Length - 2;
			_allSplinePoints = new Vector3[splinePointCount];

            // Calculate first Bezier control points
            var controlPoints = CalculateControlPoints();

			for (var i = 0; i < n; i++)
			{
				var anchorIndex = i * 3;
				_allSplinePoints[anchorIndex] = _anchorPoints[i];
				_allSplinePoints[anchorIndex + 3] = _anchorPoints[i + 1];

				// First control point
				if (i == 0)
				{
					_allSplinePoints[2] = controlPoints[0];
					_allSplinePoints[1] = 0.5f * (_allSplinePoints[0] + controlPoints[0]);
				}
				else
				{
					_allSplinePoints[anchorIndex + 2] = controlPoints[i];
					_allSplinePoints[anchorIndex + 1] = 2f * _allSplinePoints[anchorIndex] - controlPoints[i];
				}

                // Second control point
                if (_loop || i < n - 1)
                {
                    _allSplinePoints[anchorIndex + 4] = 2f * _allSplinePoints[anchorIndex + 3] - controlPoints[i + 1];
                    _allSplinePoints[anchorIndex + 5] = 2f * _allSplinePoints[anchorIndex + 3] - _allSplinePoints[anchorIndex + 4];
                }
			}

			if (_loop)
			{
				var controlPointDistance = Vector3.Distance(_allSplinePoints[2], _allSplinePoints[0]);
				var direction = Vector3.Normalize(_anchorPoints[n] - _anchorPoints[1]);

                _allSplinePoints[2] = -(_allSplinePoints[0] + direction * controlPointDistance);
                _allSplinePoints[1] = 0.5f * (_allSplinePoints[0] + _allSplinePoints[2]);

                _allSplinePoints[_allSplinePoints.Length - 1] = _allSplinePoints[0];
			}

			EnforceMirroredControlPoints();
        }

        private Vector3[] CalculateControlPoints()
        {
            // Right hand side vector
            var rhs = CalculateRHS();

            var rhsLength = rhs.Length;
            var controlPoints = new Vector3[rhsLength]; // Solution vector
            var tmp = new float[rhsLength]; // Temporary buffer

            var b = 2f;
            controlPoints[0] = rhs[0] / b;

            for (var i = 1; i < rhsLength; i++) // Decomposition and forward substitution
            {
                var val = 1f / b;
                tmp[i] = val;
                b = (i < rhsLength - 1 ? 4f : 3.5f) - val;

                controlPoints[i] = (rhs[i] - controlPoints[i - 1]) / b;
            }

            for (var i = 1; i < rhsLength; i++) // Backsubstitution
            {
                controlPoints[rhsLength - i - 1] -= tmp[rhsLength - i] * controlPoints[rhsLength - i];
            }

            return controlPoints;
        }

        private Vector3[] CalculateRHS()
        {
            var n = _anchorPoints.Length - 1;
            var rhs = new Vector3[_loop ? n + 1 : n];

            for (var i = 1; i < n - 1; i++)
            {
                rhs[i] = 4 * _anchorPoints[i] + 2 * _anchorPoints[i + 1];
            }

            rhs[0] = _anchorPoints[0] + 2 * _anchorPoints[1];

            if (_loop)
            {
                rhs[n - 1] = 4 * _anchorPoints[n - 1] + 2 * _anchorPoints[n];
                rhs[n] = (8 * _anchorPoints[n] + _anchorPoints[0]) * 0.5f;
            }
            else
            {
                rhs[n - 1] = (8 * _anchorPoints[n - 1] + _anchorPoints[n]) * 0.5f;
            }

            return rhs;
        }

        // Original solution inspired by https://github.com/yasirkula/UnityBezierSolution/blob/master/Plugins/BezierSolution
        private void GenerateSplinePointsAlternative()
		{
			// This method doesn't work well with only 2 anchor poins
			if (_anchorPoints.Length <= 2)
			{
				GenerateSplinePoints();
				return;
			}

			var anchorPointCount = _anchorPoints.Length;
			var splinePointCount = 3 * anchorPointCount - 2;
            _allSplinePoints = new Vector3[splinePointCount];

            for (var i = 0; i < anchorPointCount; i++)
			{
				var pMinus1 = Vector3.zero;
				var p1 = Vector3.zero;
				var p2 = Vector3.zero;

				if (i == 0)
				{
					if (_loop)
					{
						pMinus1 = _anchorPoints[anchorPointCount - 1];
					}
					else
					{
						pMinus1 = _anchorPoints[0];
					}
				}
				else
				{
					pMinus1 = _anchorPoints[i - 1];
				}

				if (_loop)
				{
					p1 = _anchorPoints[(i + 1) % anchorPointCount];
					p2 = _anchorPoints[(i + 2) % anchorPointCount];
				}
				else
				{
					if (i < anchorPointCount - 2)
					{
						p1 = _anchorPoints[i + 1];
						p2 = _anchorPoints[i + 2];
					}
					else if (i == anchorPointCount - 2)
					{
						p1 = _anchorPoints[i + 1];
						p2 = _anchorPoints[i + 1];
					}
					else
					{
						p1 = _anchorPoints[i];
						p2 = _anchorPoints[i];
					}
				}

				var anchorIndex = i * 3;
				_allSplinePoints[anchorIndex] = _anchorPoints[i];

				if (i == 0)
				{
                    _allSplinePoints[2] = _allSplinePoints[0] + (p1 - pMinus1) / 6f;
                    _allSplinePoints[1] = 0.5f * (_allSplinePoints[0] + _allSplinePoints[2]);
				}
				else if (i < anchorPointCount - 1)
                {
                    _allSplinePoints[anchorIndex + 2] = _allSplinePoints[anchorIndex] + (p1 - pMinus1) / 6f;
                    _allSplinePoints[anchorIndex + 1] = 2f * _allSplinePoints[anchorIndex] - _allSplinePoints[anchorIndex + 2];
				}

				if (i < anchorPointCount - 2)
				{
					_allSplinePoints[anchorIndex + 4] = p1 - (p2 - _allSplinePoints[anchorIndex]) / 6f;
					_allSplinePoints[anchorIndex + 5] = 2f * _allSplinePoints[anchorIndex + 3] - _allSplinePoints[anchorIndex + 4];
				}
				else if (_loop)
				{
					_allSplinePoints[1] = p1 - (p2 - _allSplinePoints[anchorIndex]) / 6f;
					_allSplinePoints[2] = 2f * _allSplinePoints[0] - _allSplinePoints[1];
				}
			}

			EnforceMirroredControlPoints();
        }

		private void EnforceMirroredControlPoints()
		{
			for (var i = 2; i < _anchorPoints.Length; i++)
			{
				EnforceMode((i - 1) * 3);
			}

			if (_loop)
			{
				EnforceMode(0);
			}
		}

		private void EnforceMode(int anchorIndex)
		{
			var modeIndex = (anchorIndex + 1) / 3;
			if (!_loop && (modeIndex == 0 || modeIndex == _anchorPoints.Length))
			{
				return;
			}

			var middleIndex = modeIndex * 3;
			var fixedIndex = 0;
			var enforcedIndex = 0;

			if (anchorIndex <= middleIndex)
			{
				fixedIndex = middleIndex - 1;
				if (fixedIndex < 0)
				{
					fixedIndex = _allSplinePoints.Length - 2;
				}

				enforcedIndex = middleIndex + 1;
				if (enforcedIndex >= _allSplinePoints.Length)
				{
					enforcedIndex = 1;
				}
			}
			else
			{
				fixedIndex = middleIndex + 1;
				if (fixedIndex >= _allSplinePoints.Length)
				{
					fixedIndex = 1;
				}

				enforcedIndex = middleIndex - 1;
				if (enforcedIndex < 0)
				{
					enforcedIndex = _allSplinePoints.Length - 2;
				}
			}

			var middle = _allSplinePoints[middleIndex];
			var enforcedTangent = middle - _allSplinePoints[fixedIndex];

			_allSplinePoints[enforcedIndex] = middle + enforcedTangent;
		}

		private Vector3 GetLocalPosition(Vector3 worldPosition)
        {
			//return worldPosition;
			return this.transform.InverseTransformPoint(worldPosition);
		}

		private Vector3 GetWorldPosition(Vector3 localPosition)
		{
			//return localPosition;
			return this.transform.TransformPoint(localPosition);
		}

		private void TransformSplineToLocalSpace()
		{
			for (var i = 0; i < _allSplinePoints.Length; i++)
			{
				_allSplinePoints[i] = GetLocalPosition(_allSplinePoints[i]);
			}
		}
	}
}