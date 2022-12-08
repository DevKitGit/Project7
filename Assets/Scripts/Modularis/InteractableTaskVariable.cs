using System;
using System.Collections;
using System.Collections.Generic;
using Devkit.Modularis.Variables;
using UnityEngine;

[CreateAssetMenu(menuName = "Core/Variables/Interactable Task Variable",fileName = "Interactable Task Variable",order = 100)]
public class InteractableTaskVariable : BaseVariable<InteractableTask>
{
   
}

[Serializable]
public struct InteractableTask
{ 
    [SerializeField] private TaskType taskType;
    [SerializeField] private string taskDescription;
}

public enum TaskType
{
    TestStarted, //The test was started
    ObjectSpawned, //The intended object was spawned
    SplineCreated, //The right sp
    SplineKnotCreated,
    ObjectPlacedOnSpline,
    AnimationDone,
    TestDone
}

public enum ErrorType
{
    
}
