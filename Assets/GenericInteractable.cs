using System;
using OVR;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(OutlineNormalsEstimator))]
public class GenericInteractable : MonoBehaviour
{
    [SerializeField] private Transform _transform;
    [SerializeField] private MeshRenderer _meshRenderer;
    #region Selection
    [Header("Selection")]
    [SerializeField,ReadOnly] private bool _selected;
    [SerializeField] private bool _selectable;
    public bool Selectable() => _selectable && !_selected;
    public bool Selected() => _selected;

    private void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    public bool StartSelect()
    {
        if (!Selectable())
        {
            return false;
        }
        _selected = true;
        gameObject.layer = LayerMask.NameToLayer("Selection");
        return true;
    }

    public bool StopSelect()
    {
        if (!_selected)
        {
            return false;
        }
        gameObject.layer = LayerMask.NameToLayer(_hovered ? "Hovered" : "Interactable");
        _selected = false;
        return true;
    }
    #endregion
    #region Hovering
    [Header("Hovering")]
    [SerializeField,ReadOnly] private bool _hovered;
    [SerializeField] private bool _hoverable;
    public bool Hoverable() => _hoverable && !_hovered;
    public bool Hovered() => _hovered;

    public bool StartHover()
    {
        if (!Hoverable())
        {
            return false;
        }

        gameObject.layer = _selected ? gameObject.layer : LayerMask.NameToLayer("Hovered");
        _hovered = true;
        return true;
    }

    public bool StopHover()
    {
        if (!Hovered())
        {
            return false;
        }
        gameObject.layer = _selected ? gameObject.layer : LayerMask.NameToLayer("Interactable");
        _hovered = false;
        return true;
    }
    #endregion
    
    #region Translation
    [Header("Rotation")]
    [SerializeField] private bool _beingTranslated;
    [SerializeField] private bool _translateable = true;


    public bool Translateable() => _translateable && _selected;

    public bool StartTranslation(out TranslationConfiguration configuration, out Transform interactableTransform)
    {
        if (!Translateable())
        {
            configuration = TranslationConfiguration.Default;
            interactableTransform = null;
            return false;
        }
        configuration = translationConfiguration;
        interactableTransform = _transform;
        _beingTranslated = true;
        return true;
    }

    [SerializeField] public TranslationConfiguration translationConfiguration;
    public bool StopTranslation()
    {
        if (!_beingTranslated)
        {
            return false;
        }
        _beingTranslated = false;
        
        return true;
    }
    #endregion
    #region Rotation
    [Header("Rotation")]
    [SerializeField] private bool _beingRotated;
    [SerializeField] private bool _rotatable;

    [SerializeField] private bool3 allowRotationAlongAxis;

    private Vector3 _rotationPerSecond;
    public bool Rotatable() => _rotatable && _selected;

    public bool StartRotation()
    {
        if (!Selected())
        {
            return false;
        }
        _beingRotated = true;
        return true;
    }

    public bool StopRotation()
    {
        if (!Selected())
        {
            return false;
        }
        return true;
    }
    #endregion
    #region Scaling
    [Header("Scaling")]
    [SerializeField,ReadOnly] private bool _beingScaled;
    [SerializeField] private bool _scaleable;
    [SerializeField, UnityEngine.Rendering.PostProcessing.MinMax(0.2f, 3f)] private float scaleRange;

    public bool Scalable() => _scaleable && _selected;
    #endregion
}

[Serializable]
public struct TranslationConfiguration
{
    [SerializeField] private bool3 axisConstraint;
    [SerializeField, Min(0.01f)] private float widgetSize;
    [SerializeField] private bool allowFarInteraction;
    public void Process(bool isFarInteraction, ref Vector3 position)
    {
        if (isFarInteraction && !allowFarInteraction)
        {
            position = Vector3.zero;
            return;
        }
        position.x = axisConstraint.x ? 0 : position.x;
        position.y = axisConstraint.y ? 0 : position.y;
        position.z = axisConstraint.z ? 0 : position.z;

    }
    public TranslationConfiguration(bool3 axisConstraint)
    {
        this.axisConstraint = axisConstraint;
        widgetSize = 0.01f;
        allowFarInteraction = true;
    }
    
    public TranslationConfiguration(bool3 axisConstraint, float widgetSize)
    {
        this.axisConstraint = axisConstraint;
        this.widgetSize = widgetSize;
        allowFarInteraction = true;
    }
    
    public static TranslationConfiguration Default { get; } = new();

}