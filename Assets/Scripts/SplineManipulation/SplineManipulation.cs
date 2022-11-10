using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Dreamteck.Splines;
using Dreamteck;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Random = Unity.Mathematics.Random;

public class SplineManipulation : MonoBehaviour
{
    //position vector
    private Vector3 pos;
    //spline prefab
    [SerializeField] private GameObject _splinePrefab;
    private SplineComputer _currentSplineComputer;
    //list for knots(spheres) that appear at each knot point
    //list for all knot points
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //gets mouse position
        pos = Input.mousePosition;
        pos.z = 10;
        //turns position into screen to world point
        
        //updates the index of lowest value
        if (Input.GetKeyDown(KeyCode.A))
        {
            AddNewSpline();
        }
        if (_currentSplineComputer == null)
        {
            return;
        }
        IndexOfLowestVal();
        Debug.Log(IndexOfLowestVal());
        if (Input.GetMouseButtonDown(0))
        {
            //when left click run this function
            CreateSplinePoint();
        }

        if (Input.GetKey(KeyCode.E))
        {
            //when e is clicked run this function
            EditNodesPosition(IndexOfLowestVal());
        }
        
        //add function to add spline points in between points

    }
    void CreateSplinePoint()
    {
        var _splinePoint = new SplinePoint
        {
            //sets the position of the current spline point to be mouse position
            //function which creates a new spline point at mouse position
            position = Camera.main.ScreenToWorldPoint(pos),
            //sets the normal
            normal = Vector3.forward,
            //sets the size of the point
            size = .1f,
            //sets the color of the spline
            color = Color.white
        };
        //sets the point within the spline computer
        _currentSplineComputer.SetPoint(_currentSplineComputer.pointCount, _splinePoint);
        //instantiates the knot(sphere) at the spline point position
        _currentSplineComputer.gameObject.GetComponent<SplineContainer>().newSplinePoint(_splinePoint);

    }
    
    int IndexOfLowestVal()
    {
        //function to find what index is closest to the mouse
        float _lowestVal = Single.MaxValue;
        var _lowestValIndex = 0;
        var splineContainer = _currentSplineComputer.GetComponent<SplineContainer>();
        //for loop which itterates over all points in spline points list
        for (int j = 0; j < splineContainer._knotList.Count; j++)
        {
            //finds the distance to the closest point
            var _currentVal = Vector3.Distance(Camera.main.ScreenToWorldPoint(pos), 
                _currentSplineComputer.GetPoint(j, SplineComputer.Space.World).position);
            //if statement which changes the index
            if (_currentVal < _lowestVal)
            {
                _lowestVal = _currentVal;
                _lowestValIndex = j;
            }
        }
        //returns the index
        return _lowestValIndex;
        
    }
    void EditNodesPosition(int index)
    {
        var splineContainer = _currentSplineComputer.GetComponent<SplineContainer>();

        //script which allows for the change of knot points
        _currentSplineComputer.SetPointPosition(index, Camera.main.ScreenToWorldPoint(pos));
        splineContainer._knotList[index].transform.position = Camera.main.ScreenToWorldPoint(pos);
    }
    void AddNewSpline()
    {
        _currentSplineComputer = Instantiate(_splinePrefab, Camera.main.ScreenToWorldPoint(pos), Quaternion.identity).GetComponent<SplineComputer>();
        _currentSplineComputer.gameObject.GetComponent<TubeGenerator>().enabled = true;
    }
    
}
