using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[RequireComponent(typeof(OutlineNormalsEstimator))]
public class GenericInteractable : MonoBehaviour
{
    [SerializeField] private Transform _transform;

    #region Selection
    [Header("Selection")]
    [SerializeField] private bool _selectable;
    [SerializeField] private bool _selected;
    public bool Selectable() => _selectable && !_selected;
    public bool Selected() => _selected;

    public bool Select()
    {
        if (!Selectable())
        {
            return false;
        }
        _selected = true;
        gameObject.layer = LayerMask.NameToLayer("Selected");
        return true;
    }

    public bool Deselect()
    {
        if (_selected)
        {
            return false;
        }
        gameObject.layer = LayerMask.NameToLayer("Selectable");
        _selected = false;
        return true;
    }
    #endregion
    #region Hovering
    [Header("Hovering")]
    [SerializeField] private bool _hoverable;
    [SerializeField] private bool _hovered;
    public bool Hoverable() => _hoverable && !_hovered;
    public bool Hovered() => _hovered;

    public bool HoverStart()
    {
        if (!Hoverable())
        {
            return false;
        }
        _hovered = true;
        return true;
    }

    public bool HoverStop()
    {
        if (Hovered())
        {
            return false;
        }
        _hovered = false;
        return true;
    }
    #endregion
    #region Rotation
    [Header("Rotation")]

    [SerializeField] private bool _rotatable;
    [SerializeField] private bool3 allowRotationAlongAxis;

    private Vector3 _rotationPerSecond;
    public bool Rotatable() => _rotatable && _selected;

    public bool StartRotation()
    {
        if (Selected())
        {
            
        }

        return false;
    }

    public bool StopRotation()
    {
        return false;
    }
    #endregion

    #region Scaling
    [Header("Scaling")]
    [SerializeField] private bool _scaleable;
    
    [SerializeField, MinMax(0.2f, 3f)] private float scaleRange;
    
    public bool Scalable() => _scaleable && _selected;
    #endregion



    
    
    
}