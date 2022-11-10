using System;
using System.Collections;
using System.Collections.Generic;
using AnimationSystem.Splines;
using UnityEngine;

public class SplineManipulator : MonoBehaviour
{
    [SerializeField] private BezierPoint _point = default;

    private Vector3 pos;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        pos = Input.mousePosition;
        pos.z = 10;
        transform.position = Camera.main.ScreenToWorldPoint(pos);

        if (Input.GetMouseButtonDown(0))
        {
            GetBezierPoint();
        }

        if (Input.GetKey(KeyCode.E))
        {
            
        }

        if (Input.GetKey(KeyCode.A))
        {
            
        }

        if (Input.GetKey(KeyCode.N))
        {
            
        }

        Vector3 GetBezierPoint()
        {
            _point.Position = transform.position;
            return _point.Position;
        }
    }
}
