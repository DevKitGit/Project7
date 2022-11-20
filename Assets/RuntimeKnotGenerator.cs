using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class RuntimeKnotGenerator : MonoBehaviour
{
    [SerializeField] private GameObject _knotPrefab;
    
    private List<GameObject> _knotGameObjects = new(50);
    
    private SplineExtrude _splineExtrude;

    private bool splineDirtyThisFrame;
    
    private GameObject KnotBeingMoved;
    [SerializeField,ReadOnly] private bool isKnotBeingMoved;

    // Start is called before the first frame update
    void Start()
    {
        _splineExtrude = GetComponent<SplineExtrude>();
        GenerateInitialKnot();
        _splineExtrude.Spline.SetTangentMode(TangentMode.AutoSmooth);
    }

    private void Update()
    {
        UpdateSpline();
    }

    private void UpdateSpline()
    {
        var changed = false;
        for (var i = 0; i < _knotGameObjects.Count; i++)
        {
            if (_knotGameObjects[i] == null)
            {
                _splineExtrude.Spline.RemoveAt(i);
                _knotGameObjects.RemoveAt(i);
                i--;
                changed = true;
                _splineExtrude.Rebuild();

                continue;
            }
            /*if (!DoesTransformMatchKnot(_knotGameObjects[i].transform.position,_knotGameObjects[i].transform.rotation, _splineContainer.Spline[i]))
            {
                var knot = new BezierKnot(
                    _knotGameObjects[i].transform.localPosition,
                    float3.zero,
                    float3.zero,
                    Quaternion.Euler(Vector3.forward));
                _splineContainer.Spline.RemoveAt(i);
                _splineContainer.Spline.Insert(i,knot);
                changed = true;
            }*/
        }

        _splineExtrude.Spline.SetTangentMode(TangentMode.AutoSmooth);
    }

    private bool DoesTransformMatchKnot(Vector3 p, Quaternion q, BezierKnot k)
    {
        float TOLERANCE = 0.01f;
        return Math.Abs(p.x - k.Position.x) < TOLERANCE &&
               Math.Abs(p.y - k.Position.y) < TOLERANCE &&
               Math.Abs(p.z - k.Position.z) < TOLERANCE &&
               q == k.Rotation;
    }

    public void AddPointToEnd(Vector3 position)
    {
        var bezierKnot = new BezierKnot(transform.InverseTransformPoint(position));
        _splineExtrude.Spline.Add(bezierKnot,TangentMode.AutoSmooth);
        var knot = Instantiate(_knotPrefab,position,Quaternion.identity,transform);
        _knotGameObjects.Add(knot);
        _splineExtrude.Rebuild();
    }
    private void GenerateInitialKnot()
    {
        _splineExtrude.Spline.Add(new BezierKnot(float3.zero),TangentMode.AutoSmooth);
        var initialKnot = Instantiate(_knotPrefab,transform.TransformPoint(0,0,0),Quaternion.Euler(Vector3.forward),transform);
        _knotGameObjects.Add(initialKnot);
        _splineExtrude.Rebuild();

    }
}
