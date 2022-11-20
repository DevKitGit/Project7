using System;
using System.Collections;
using System.Collections.Generic;
using Devkit.Modularis.References;
using UnityEngine;

public class SplineInteractor : MonoBehaviour
{
    [SerializeField] private GestureIdReference currentEventReference;
    [SerializeField] private ObjectReference splineSpawnTransform;
    [SerializeField] private ObjectReference currentlySelectedObject;
    private bool selectionActive;
    private GameObject currentSplineSpawnAnchor;
    [SerializeField] private float distanceBetweenKnots;
    [SerializeField] private GameObject splinePrefab;
    [SerializeField] private Gesture.GestureID gestureThatEnabled;
    private bool GestureActive,trackingActive,SplineSpawned;
    private Vector3 lastKnotPosition;
    private RuntimeKnotGenerator _runtimeKnotGenerator;
    // Start is called before the first frame update
    void OnEnable()
    {
        splineSpawnTransform.RegisterCallback(OnSplineTransformChanged);
        currentEventReference.RegisterCallback(OnGestureChanged);
        currentlySelectedObject.RegisterCallback(OnSelectionChanged);
       
    }

    private void OnSelectionChanged()
    {
        selectionActive = currentlySelectedObject != null;
    }

    private void OnGestureChanged()
    {
        GestureActive = currentEventReference.Value == gestureThatEnabled;
        SplineSpawned = false;
    }

    private void OnDestroy()
    {
        splineSpawnTransform.UnregisterCallback(OnSplineTransformChanged);
    }

    private void OnSplineTransformChanged()
    {
        currentSplineSpawnAnchor = (GameObject)splineSpawnTransform.Value;
        trackingActive = currentSplineSpawnAnchor != null;
    }
    // Update is called once per frame
    void Update()
    {
        if (!trackingActive) { return; }
        if (!GestureActive) { return; }
        if (currentSplineSpawnAnchor.transform.position == Vector3.zero) { return; }

        if (!SplineSpawned)
        {
            SplineSpawned = true;
            lastKnotPosition = currentSplineSpawnAnchor.transform.position;

            var splineGo = Instantiate(splinePrefab,lastKnotPosition,Quaternion.identity);
            _runtimeKnotGenerator = splineGo.GetComponent<RuntimeKnotGenerator>();
        }
        if (Vector3.Distance(lastKnotPosition, currentSplineSpawnAnchor.transform.position) > distanceBetweenKnots)
        {
            lastKnotPosition = currentSplineSpawnAnchor.transform.position;
            _runtimeKnotGenerator.AddPointToEnd(lastKnotPosition);
        }
    }
}
