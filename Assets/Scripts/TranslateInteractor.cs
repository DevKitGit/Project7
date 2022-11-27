using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Linq;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

public class TranslateInteractor : MonoBehaviour
{
    [SerializeField] private ObjectEventReference _translateAnchor;
    [SerializeField] private GestureIdReference _currentGesture;
    [SerializeField] private InteractableReference _currentSelection;
    [SerializeField] private LayerMask _interactableLayerMask;
    [SerializeField] private float _radius;
    [SerializeField] private GameObject widgetPrefab;
    [SerializeField] private LineRenderer directionRenderer;
    
    private GameObject spawnedWidget;
    private bool _gestureActive;
    private bool _trackingActive;
    private Transform _anchorTransform;
    private SphereCollider _nearInteractionCollider;
    private Collider[] _colliders = new Collider[3];
    private bool _nearInteraction;
    private bool _farInteractionWidgetSpawned;
    
    Vector3 directionVector;
    private void Update()
    {
        if (!_trackingActive || !translationActive) { return; }
        if (_nearInteraction)
        {
            
        }
        else
        {
            if (!_farInteractionWidgetSpawned){ return;}
            directionVector = Vector3.ClampMagnitude(_anchorTransform.position - spawnedWidget.transform.position, 1f);
            _config.Process(true,ref directionVector);
            
            directionRenderer.SetPosition(1,spawnedWidget.transform.position + directionVector);
            interactableTransform.Translate(directionVector * Time.deltaTime * translateSpeed);
        }
    }

    private bool IsNearInteraction()
    {
        int count = Physics.OverlapSphereNonAlloc(_anchorTransform.position, _radius, _colliders, _interactableLayerMask,QueryTriggerInteraction.Collide);
        bool isNearby = false;
        for (int i = 0; i < count; i++)
        {
            if (_colliders[i].gameObject != _currentSelection.Value.gameObject) continue;
            isNearby = true;
            break;
        }
        return isNearby;
    }

    #region EventRegistration
    private void OnEnable()
    {
        _translateAnchor.RegisterListener(OnAnchorCreate, OnAnchorDestroy);
        _currentGesture.RegisterCallback(OnGestureChanged);
    }
    private void OnDisable()
    {
        _translateAnchor.UnregisterListener(OnAnchorCreate, OnAnchorDestroy);
        _currentGesture.UnregisterCallback(OnGestureChanged);
    }
    #endregion
    
    #region EventCallbacks

    private Transform interactableTransform;
    private TranslationConfiguration _config;
    private bool translationActive;
    private Vector3 widgetSpawnDirection;
    [SerializeField] private float translateSpeed = 0.1f;

    private void OnGestureChanged()
    {
        _gestureActive = _currentGesture == Gesture.GestureID.Translate;
        
        if (!_gestureActive)
        {
            if (_farInteractionWidgetSpawned)
            {
                ToggleFarInteractionWidget(false);
            }
            return;
        }
        if (_currentSelection.Value == null)
        {
            return;
        }

        translationActive = _currentSelection.Value.StartTranslation(out _config, out interactableTransform);
       
        _nearInteraction = IsNearInteraction();
        if (_nearInteraction) { return; }
        ToggleFarInteractionWidget(true);
    }

    private void ToggleFarInteractionWidget(bool toggle)
    {
        _farInteractionWidgetSpawned = toggle;
        directionRenderer.enabled = toggle;
        if (spawnedWidget != null)
        {
            Destroy(spawnedWidget);
        }
        if (toggle)
        {
            
            var position = _anchorTransform.position;
            widgetSpawnDirection = CameraCache.Main.transform.rotation.eulerAngles;
            widgetSpawnDirection.z = 0;
            widgetSpawnDirection.x = 0;
            spawnedWidget = Instantiate(widgetPrefab, position, Quaternion.Euler(widgetSpawnDirection));
            directionRenderer.SetPosition(0,position);
            directionRenderer.SetPosition(1,position);
        }
    }

    private void OnAnchorCreate(Object obj)
    {
        _anchorTransform = ((GameObject)obj).transform;
        _trackingActive = true;
    }
    private void OnAnchorDestroy(Object obj)
    {
        _anchorTransform = null;
        _trackingActive = false;
    }
    #endregion
}