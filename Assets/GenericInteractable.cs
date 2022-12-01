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
        if (_beingTranslated)
        {
            StopTranslation();
        }

        if (_beingRotated)
        {
            StopRotation();
        }

        if (_beingScaled)
        {
            //StopScaling();
        }
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
    [Header("Translation")]
    [SerializeField] private bool _beingTranslated;
    [SerializeField] private bool _translateable = true;


    public bool Translateable() => _translateable && _selected;
    public bool BeingTranslated => _beingTranslated;
    [SerializeField] public TranslationConfiguration translationConfiguration;

    
    public bool StartTranslation(ref TranslationConfiguration configuration, ref Transform interactableTransform)
    {
        if (!Translateable())
        {
            return false;
        }
        configuration = translationConfiguration;
        interactableTransform = _transform;
        _beingTranslated = true;
        return true;
    }

    public bool VerifyOwnership(ref Transform interactorHeldTransform) => interactorHeldTransform == _transform;

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
    
    private Vector3 _rotationPerSecond;
    public bool Rotatable() => _rotatable && _selected;

    [SerializeField] public RotationConfiguration rotationConfiguration;

    public bool StartRotation(ref RotationConfiguration configuration, ref Transform interactableTransform)
    {
        if (!Rotatable())
        {
            return false;
        }
        configuration = rotationConfiguration;
        interactableTransform = _transform;
        _beingRotated = true;
        return true;
    }

    public bool StopRotation()
    {
        if (!_beingRotated)
        {
            return false;
        }
        _beingRotated = false;
        return true;
    }

    #endregion
    #region Scaling
    [Header("Scaling")]
    [SerializeField] private bool _beingScaled;
    [SerializeField] private bool _scalable = true;
    public bool Scalable() => _scalable && _selected;
    public bool BeingScaled => _beingScaled;
    [SerializeField] public ScaleConfiguration scaleConfiguration;
    public bool StartScaling(ref ScaleConfiguration configuration, ref Transform interactableTransform)
    {
        if (!Scalable())
        {
            return false;
        }
        configuration = scaleConfiguration;
        interactableTransform = _transform;
        _beingScaled = true;
        return true;
    }
    
    public bool StopScaling()
    {
        if (!_beingScaled)
        {
            return false;
        }
        _beingScaled = false;
        return true;
    }
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
[Serializable]
public struct RotationConfiguration
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
    public RotationConfiguration(bool3 axisConstraint)
    {
        this.axisConstraint = axisConstraint;
        widgetSize = 0.01f;
        allowFarInteraction = true;
    }
    
    public RotationConfiguration(bool3 axisConstraint, float widgetSize)
    {
        this.axisConstraint = axisConstraint;
        this.widgetSize = widgetSize;
        allowFarInteraction = true;
    }
    
    public static RotationConfiguration Default { get; } = new();
}
[Serializable]
public struct ScaleConfiguration
{
    [SerializeField] private Vector3 minScale;
    [SerializeField] private Vector3 maxScale;
    public void Process(ref Vector3 position)
    {
        position.x = math.clamp(position.x, minScale.x, maxScale.x);
        position.y = math.clamp(position.y, minScale.y, maxScale.y);
        position.z = math.clamp(position.z, minScale.z, maxScale.z);
    }
    public ScaleConfiguration(Vector3 minScale, Vector3 maxScale)
    {
        this.minScale = minScale;
        this.maxScale = maxScale;
    }
    public static ScaleConfiguration Default { get; } = new();
}