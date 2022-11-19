using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Physics;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Object = UnityEngine.Object;

[RequireComponent(typeof(LineRenderer))]
public class SelectionInteractor : MonoBehaviour
{
    [SerializeField] private ObjectEventReference _originPoint;
    [SerializeField] private GestureIdReference _gestureStartedEvent;
    [SerializeField] private GestureIdReference _gestureStoppedEvent;
    [SerializeField,UnityEngine.Min(1f)] private float MaxRadius = 1f;
    [SerializeField,UnityEngine.Min(1f)] private float MaxDistance = 1f;
    [SerializeField,UnityEngine.Min(1f)] private float ConeAngle = 1f;
    [SerializeField] private LayerMask selectableLayerMask;
    [SerializeField,UnityEngine.Min(3)] private int lineResolution = 100;
    [SerializeField,Range(0.01f,1f)] private float _bezierCurveDistance;
    //Origin Point Transform
    private Transform _OP;
    private LineRenderer _lineRenderer;
    private Gesture.GestureID currentGesture;
    private GenericInteractable currHoverInter = null;
    private GenericInteractable currentlySelectedInteractable = null;

    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 2;
        _lineRenderer.enabled = false;
        
    }
    private Vector3 _p0, _p1, _p2;
    private float _t = 0f;
    private Vector3[] _linePositions;

    private void LateUpdate()
    {
        if (_hoverEnabled)
        {
            LookForNewHoverTarget();
            GenerateBezierLineRenderer();
        }
    }
    

    public void GenerateBezierLineRenderer()
    {
        //First Bezier point
        _p0 = _OP.position;
        _p2 = currHoverInter.transform.position;
        //Middle Bezier point using Right Triangle math, i need to find the distance along the ray direction
        //that a given target is placed away from the user, in order to normalize for it for easier configurability.
        //a = c * sin(A)
        //b = sqrt(c^2 - a^2)
        var A = Vector3.Angle(_OP.forward, (_p2 - _p0).normalized);
        var c = Vector3.Distance(_p0, _p2);
        var a = c * math.sin(A);
        var b = math.sqrt(math.pow(c, 2) - math.pow(a, 2));

        _p1 = Vector3.MoveTowards(_p0,_p0 + _OP.forward * b,_bezierCurveDistance);       

        for (int i = 0; i < lineResolution; i++)
        {
            //B(t) = (1-t)^2*P0 + 2*(1-t)*t*P1 + t^2P2, 0 < t < 1
            _t = (float) i / lineResolution;
            _linePositions[i] = math.pow(1 - _t, 2) * _p0 + 2 * (1 - _t) * _t * _p1 + math.pow(_t, 2) * _p2;
        }
        _lineRenderer.SetPositions(_linePositions);
    }
    
    private void OnValidate()
    {
        _linePositions = new Vector3[lineResolution];
        _lineRenderer.positionCount = lineResolution;
    }

    private GenericInteractable _newHoverTargetInteractable;
    private RaycastHit _newHoverTargetRaycast;
    public void LookForNewHoverTarget()
    {
        var hoverTarget = ConeCastWithChecks(_OP.position, _OP.forward, MaxRadius, MaxDistance, ConeAngle, selectableLayerMask, .2f, 0.3f, 0.2f, 0.3f, out _newHoverTargetRaycast);
        if (!hoverTarget) { return;}
        if (_newHoverTargetRaycast.transform.TryGetComponent(out _newHoverTargetInteractable)) { return; }
        if (!_newHoverTargetInteractable.HoverStart()) { return; }
        currHoverInter = _newHoverTargetInteractable;
    }

    private void OnEnable()
    {
        _originPoint.RegisterListener(OnOriginCreate, OnOriginDestroy);
        _gestureStartedEvent.RegisterCallback(OnGestureStarted);
        _gestureStoppedEvent.RegisterCallback(OnGestureStopped);
    }



    private void OnDisable()
    {
        _originPoint.UnregisterListener(OnOriginCreate, OnOriginDestroy);
        _gestureStartedEvent.UnregisterCallback(OnGestureStarted);
        _gestureStoppedEvent.UnregisterCallback(OnGestureStopped);


    }
    private void OnGestureStarted()
    {
        _hoverEnabled = currentGesture == Gesture.GestureID.Hover;
    }
    private void OnGestureStopped()
    {
        _hoverEnabled = false;
    }

    private void OnOriginCreate(Object obj) => _OP = ((GameObject)obj).transform;
    private void OnOriginDestroy(Object obj) => _OP = null;
    private RaycastHit[] sphereCastHits;
    private RaycastHit currHit;
    private int sphereCastMaxHitCount = 10;
    private bool _hoverEnabled;
    private const int sphereCastLimit = 100;
    private GenericInteractable _coneCastGenericInteractable;
    /// <summary>
    /// Casts a sphere along a ray and checks if the hitpoint is within the angle of the cone and returns the best target determined by the weights provided.
    /// </summary>
    /// <param name="raycastHit"></param>
    /// <param name="origin">The vertex of the cone and the at the start of the sweep.</param>
    /// <param name="direction">The direction into which to sweep the sphere..</param>
    /// <param name="maxRadius">The radius of the sweep.</param>
    /// <param name="maxDistance">The max length of the cast.</param>
    /// <param name="coneAngle">The angle used to define the cone.</param>
    /// <param name="layerMask">A Layer mask that is used to selectively ignore colliders when casting a capsule.</param>
    /// <param name="distanceWeight">The importance of distance between the hitpoint and the origin in selecting the best target.</param>
    /// <param name="angleWeight">The importance of angle between the hitpoint and the origin in selecting the best target.</param>
    /// <param name="distanceToCenterWeight">The importance of distance between the hitpoint and the center of the object in selecting the best target.</param>
    /// <param name="angleToCenterWeight">The importance of angle between the hitpoint and the center of the object in selecting the best target.</param>
    /// <returns>The RaycastHit of the best object.</returns>
    public bool ConeCastWithChecks(Vector3 origin, Vector3 direction, float maxRadius, float maxDistance, float coneAngle, LayerMask layerMask, float distanceWeight, float angleWeight, float distanceToCenterWeight, float angleToCenterWeight,out RaycastHit raycastHit)
    {
        if (sphereCastHits == null || sphereCastHits.Length < sphereCastMaxHitCount)
        {
            sphereCastHits = new RaycastHit[sphereCastMaxHitCount];
        }

        var hitCount = Physics.SphereCastNonAlloc(origin - (direction * maxRadius), maxRadius, direction, sphereCastHits, maxDistance, layerMask, QueryTriggerInteraction.Collide);

        // Algorithm: double the max hit count if there are too many results, up to a certain limit
        if (hitCount >= sphereCastMaxHitCount && sphereCastMaxHitCount < sphereCastLimit)
        {
            // There might be more hits we didn't get, grow the array and try again next time
            // Note that this frame, the results might be imprecise.
            sphereCastMaxHitCount = Math.Min(sphereCastLimit, sphereCastMaxHitCount * 2);
        }

        raycastHit = new RaycastHit();
        float score = float.MaxValue;

        for (int i = 0; i < hitCount; i++)
        {
            currHit = sphereCastHits[i];
            if (!currHit.transform.TryGetComponent(out _coneCastGenericInteractable)) { continue; }
            if (!_coneCastGenericInteractable.Hoverable()) { continue; }

            Vector3 hitPoint = currHit.point;
            Vector3 directionToHit = hitPoint - origin;
            float angleToHit = Vector3.Angle(direction, directionToHit);
            var position = currHit.collider.transform.position;
            Vector3 hitDistance = position - hitPoint;
            Vector3 directionToCenter = position - origin;
            float angleToCenter = Vector3.Angle(direction, directionToCenter);

            // Additional work to see if there is a better point slightly further ahead on the direction line. This is only allowed if the collider isn't a mesh collider.
            if (currHit.collider.GetType() != typeof(MeshCollider))
            {
                Vector3 pointFurtherAlongGazePath = (maxRadius * 0.5f * direction.normalized) + FindNearestPointOnLine(origin, direction, hitPoint);
                Vector3 closestPointToPointFurtherAlongGazePath = currHit.collider.ClosestPoint(pointFurtherAlongGazePath);
                Vector3 directionToSecondaryPoint = closestPointToPointFurtherAlongGazePath - origin;
                float angleToSecondaryPoint = Vector3.Angle(direction, directionToSecondaryPoint);

                if (angleToSecondaryPoint < angleToHit)
                {
                    hitPoint = closestPointToPointFurtherAlongGazePath;
                    directionToHit = directionToSecondaryPoint;
                    angleToHit = angleToSecondaryPoint;
                    hitDistance = currHit.collider.transform.position - hitPoint;
                }
            }

            if (!(angleToHit < coneAngle)) continue;
            float distanceScore = distanceWeight == 0 ? 0.0f : (distanceWeight * directionToHit.magnitude);
            float angleScore = angleWeight == 0 ? 0.0f : (angleWeight * angleToHit);
            float centerScore = distanceToCenterWeight == 0 ? 0.0f : (distanceToCenterWeight * hitDistance.magnitude);
            float centerAngleScore = angleToCenterWeight == 0 ? 0.0f : (angleToCenterWeight * angleToCenter);
            float newScore = distanceScore + angleScore + centerScore + centerAngleScore;

            if (!(newScore < score)) continue;
            score = newScore;
            raycastHit = currHit;
        }
        return raycastHit.transform != null;
    }

    private static Vector3 FindNearestPointOnLine(Vector3 origin, Vector3 direction, Vector3 point)
    {
        direction.Normalize();
        float dotP = Vector3.Dot(point - origin, direction);
        return origin + direction * dotP;
    }
}