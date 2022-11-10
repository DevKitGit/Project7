using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using Unity.Mathematics;
using UnityEngine;

public class SplineContainer : MonoBehaviour
{
    public List<GameObject> _knotList = new List<GameObject>();
    [SerializeField] private GameObject _knotPoints;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void newSplinePoint(SplinePoint _splinePoint)
    {
        var newKnot = Instantiate(_knotPoints, _splinePoint.position, quaternion.identity);
        //adds the knot to a list of knots
        _knotList.Add(newKnot);
    }
}
