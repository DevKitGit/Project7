using System;
using System.Collections;
using System.Collections.Generic;
using Devkit.Modularis.References;
using UnityEngine;

public class SelectableLayerChecker : MonoBehaviour
{
    [SerializeField] private ObjectReference currentlySelectedGameObject;

    private void OnEnable()
    {
        currentlySelectedGameObject.RegisterCallback(OnSelectionChanged);
    }

    private void OnDisable()
    {
        currentlySelectedGameObject.UnregisterCallback(OnSelectionChanged);
    }

    private void OnSelectionChanged()
    {
        if (currentlySelectedGameObject.Value == gameObject)
        {
            gameObject.layer = LayerMask.NameToLayer("Selection");
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Default");
        }
    }
}
