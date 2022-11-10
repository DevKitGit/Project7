using System;
using System.Collections;
using System.Collections.Generic;
using Devkit.Modularis.Events;
using Devkit.Modularis.References;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

public class NearTranslatable : MonoBehaviour
{
    [SerializeField] private Transform transformToMove;
    [SerializeField, Min(1)] private float translatePositionSpeed = 1f;
    [SerializeField, Min(1)] private float translateRotationSpeed = 1f;
    [SerializeField] private Gesture.GestureID TranslateGesture;
    
    
    [Header("Right hand")]
    [SerializeField] private Vector3Reference rightHandPosition;
    [SerializeField] private QuaternionReference rightHandRotation;
    [SerializeField] private GestureIdReference currentRightGesture;
    
    [Header("Left hand")]
    [SerializeField] private Vector3Reference leftHandPosition;
    [SerializeField] private QuaternionReference leftHandRotation;
    [SerializeField] private GestureIdReference currentLeftGesture;
    
    [Header("Info")]
    [SerializeField, ReadOnly] private bool rightTranslateGestureActive;
    [SerializeField, ReadOnly] private bool leftTranslateGestureActive;
    [SerializeField, ReadOnly] private bool isTouchedByPlayer;

    private Vector3 currentTargetPosition;
    private Quaternion currentTargetRotation;
    private void Update()
    {
        if (!isTouchedByPlayer) return;
        if (rightTranslateGestureActive)
        {
            currentTargetPosition = rightHandPosition;
            currentTargetRotation = rightHandRotation;
        }
        else if (leftTranslateGestureActive)
        {
            currentTargetPosition = leftHandPosition;
            currentTargetRotation = leftHandRotation;
        }
        else
        {
            return;
        }
        transformToMove.SetPositionAndRotation(
            Vector3.MoveTowards(transform.position, currentTargetPosition, Time.deltaTime * translatePositionSpeed),
            Quaternion.RotateTowards(transform.rotation, currentTargetRotation, Time.deltaTime * translateRotationSpeed));
    }
    
    public void RightTranslateStart()
    {
        
        rightTranslateGestureActive = currentRightGesture.Value == TranslateGesture;
    }
    
    public void RightTranslateStop()
    {
        rightTranslateGestureActive = false;
        isTouchedByPlayer = leftTranslateGestureActive;

    }
    public void LeftTranslateStart()
    {
        leftTranslateGestureActive = currentLeftGesture.Value == TranslateGesture;
    }
    
    public void LeftTranslateStop()
    {
        leftTranslateGestureActive = false;
        isTouchedByPlayer = rightTranslateGestureActive;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isTouchedByPlayer = true;
        }
    }

    private void OnCollisionEnter(Collision other)
    {

    }
}
