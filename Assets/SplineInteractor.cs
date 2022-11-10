using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineInteractor : MonoBehaviour
{
    [SerializeField] private float distanceBetweenKnots;
    [SerializeField] private GameObject splinePrefab;
    private Vector3 lastKnotPosition;
    private bool extrudeSplineMode;

    private RuntimeKnotGenerator _runtimeKnotGenerator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            var splineGo = Instantiate(splinePrefab,transform.position,Quaternion.identity);
            _runtimeKnotGenerator = splineGo.GetComponent<RuntimeKnotGenerator>();
            extrudeSplineMode = true;
            lastKnotPosition = transform.position;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            extrudeSplineMode = false;
        }
        if (extrudeSplineMode & Vector3.Distance(lastKnotPosition, transform.position) >= distanceBetweenKnots)
        {
            lastKnotPosition = transform.position;
            _runtimeKnotGenerator.AddPointToEnd(transform.position);
        }
    }
}
