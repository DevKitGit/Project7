using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentGestureSelection : MonoBehaviour
{
    [SerializeField] private GestureRecorderEvents gestureRecorderEvent;
    [SerializeField] private MeshRenderer backgroundQuad;
    [SerializeField] private MeshRenderer currentSelectionQuad;

    [SerializeField, Range(0f,0.1f)] private float _borderPercentage;
    private float _border;
    [SerializeField, Range(0f,0.3f)] private float _spacingPercentage;
    private float _spacing;
    
    private float _width;
    private float _height;
    private bool _initialized;
    private int _maxValue, _currentValue;

    void OnEnable()
    {
        if (gestureRecorderEvent == null) return;
        gestureRecorderEvent.OnGestureSelected += UpdateCurrentGestureIndicator;
    }
    void OnDisable()
    {
        if (gestureRecorderEvent == null) return;
        gestureRecorderEvent.OnGestureSelected -= UpdateCurrentGestureIndicator;
    }
    private void UpdateCurrentGestureIndicator(object obj)
    {
        if (obj is not Gesture gesture) return;
        _maxValue = Math.Min(gestureRecorderEvent.gestures.Count,1);
        _currentValue = gestureRecorderEvent.gestures.IndexOf(gesture);
        if (_initialized) return;
        _initialized = true;
        //setup initial config
        
    }
    /*
     private Vector2 GetCenterPointOfIndex(int i)
    {
        return new Vector2((1 + 2 i)/(2 M), _height / 2);
    }
    private float CalculateSingleWidth()
    {
        return _width - 2 * BorderPercentage - (_maxValue - 1) * SpacingPercentage;
    }*/


    private void ResizeSelectionHighlight()
    {
        
    }
}
