using System;
using System.Collections;
using System.Collections.Generic;
using Devkit.Modularis.References;
using UnityEngine;

public class SplineInteractor : MonoBehaviour
{
    [SerializeField] private GestureIdReference currentEventReference;
    [SerializeField] private ObjectReference splineSpawnTransform;
    private GameObject currentSplineSpawnAnchor;
    [SerializeField] private float distanceBetweenKnots;
    [SerializeField] private GameObject splinePrefab;
    [SerializeField] private Gesture.GestureID gestureThatEnabled;
    
    private Vector3 lastKnotPosition;
    private RuntimeKnotGenerator _runtimeKnotGenerator;
    // Start is called before the first frame update
    void OnEnable()
    {
        splineSpawnTransform.RegisterCallback(OnSplineTransformChanged);
        currentEventReference.RegisterCallback(OnGestureChanged);
    }

    private void OnGestureChanged()
    {
        //Do stuff possibly?
    }

    private void OnDestroy()
    {
        splineSpawnTransform.UnregisterCallback(OnSplineTransformChanged);
    }

    private void OnSplineTransformChanged()
    {
        currentSplineSpawnAnchor = (GameObject)splineSpawnTransform.Value;
    }
    // Update is called once per frame
    void Update()
    {
        if (currentEventReference == gestureThatEnabled)
        {
            lastKnotPosition = currentSplineSpawnAnchor.transform.position;

            var splineGo = Instantiate(splinePrefab,lastKnotPosition,Quaternion.identity);
            _runtimeKnotGenerator = splineGo.GetComponent<RuntimeKnotGenerator>();
        }
        
        if (currentEventReference == gestureThatEnabled & Vector3.Distance(lastKnotPosition, currentSplineSpawnAnchor.transform.position) > distanceBetweenKnots)
        {
            lastKnotPosition = currentSplineSpawnAnchor.transform.position;
            _runtimeKnotGenerator.AddPointToEnd(lastKnotPosition);
        }
    }
}
