using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MixedReality.Toolkit;
using Unity.Mathematics;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Splines;

public class RuntimeKnotGenerator : MonoBehaviour
{
    [SerializeField] private GameObject _knotPrefab;
    [SerializeField] private Transform childList;
    
    private List<GameObject> _knotGameObjects = new(50);
    
    private SplineExtrude _splineExtrude;

    private bool splineDirtyThisFrame;
    
    private GameObject KnotBeingMoved;
    [SerializeField,ReadOnly] private bool isKnotBeingMoved;

    // Start is called before the first frame update
    void Start()
    {
        _splineExtrude = GetComponent<SplineExtrude>();
        SplineUtility
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
            }
        }
        if (changed)
        {
            _splineExtrude.Spline.SetTangentMode(TangentMode.AutoSmooth);
        }
        
    }

    public void OnKnotMoved(Vector3 newKnotPos)
    {
        for (var i = 0; i < _knotGameObjects.Count; i++)
        {
            if (_knotGameObjects[i] == null)
            {
                _splineExtrude.Spline.RemoveAt(i);
                _knotGameObjects.RemoveAt(i);
                i--;
                _splineExtrude.Rebuild();
            }
            else
            {
                _splineExtrude.Spline.SetKnotNoNotify(i, new BezierKnot(transform.InverseTransformPoint(_knotGameObjects[i].transform.position)));
            }
        }
        _splineExtrude.Rebuild();
    }

    private Vector3 initialPosition;

    public void OnSpawnerTranslateBegun(Vector3 position)
    {
        initialPosition = position;
    }
    public void AddPointToEnd(Vector3 position)
    {
        var bezierKnot = new BezierKnot(transform.InverseTransformPoint(position));
        _splineExtrude.Spline.Add(bezierKnot,TangentMode.AutoSmooth);
        var knot = Instantiate(_knotPrefab,position,Quaternion.identity,childList);
        UnityEventTools.AddPersistentListener(knot.GetComponent<GenericInteractable>()._onTranslateEnd, OnKnotMoved);
        
        //knot.GetComponent<GenericInteractable>()._onTranslateEnd.A(OnKnotMoved);
        _knotGameObjects.Add(knot);
        _splineExtrude.Rebuild();
    }
    private void GenerateInitialKnot()
    {
        _splineExtrude.Spline.Add(new BezierKnot(float3.zero),TangentMode.AutoSmooth);
        var initialKnot = Instantiate(_knotPrefab,transform.TransformPoint(0,0,0),Quaternion.Euler(Vector3.forward),childList);
        initialKnot.GetComponent<GenericInteractable>().SetDestroyable(false);
        gameObject.layer = LayerMask.NameToLayer("Default");
        _knotGameObjects.Add(initialKnot);
        _splineExtrude.Rebuild();
    }
    
}
