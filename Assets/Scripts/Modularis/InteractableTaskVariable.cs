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
public class InteractableTask
{
    private GameObject taskGameobject;
    private string taskString;
    
}
