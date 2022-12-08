using System;
using System.Collections;
using System.Collections.Generic;
using Devkit.Modularis.References;
using TMPro;
using UnityEngine;

public class UpdateTaskText : MonoBehaviour
{
    
    [SerializeField] private TextMeshPro currentTaskText;
    [SerializeField] private StringReference currentStringReference;
    private void OnEnable()
    {
        currentStringReference.RegisterCallback(OnTaskUpdated);
        currentTaskText.SetText(currentStringReference.Value);
    }
    private void OnDisable()
    {
        currentStringReference.UnregisterCallback(OnTaskUpdated);
    }
    private void OnTaskUpdated()
    {
        currentTaskText.SetText(currentStringReference.Value);
        currentTaskText.ForceMeshUpdate();
    }
}
