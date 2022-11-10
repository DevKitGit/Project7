using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class ObjectEvent : MonoBehaviour
{
    [SerializeField] private ObjectEventReference objectEventReference;
    [SerializeField] private bool _onCreate;
    [SerializeField] private bool _onDestroy;
    
    void Start()
    {
        if (_onCreate)
        {
            objectEventReference.ObjectCreated(gameObject);
        }
    }
    
    private void OnDestroy()
    {
        if (_onDestroy)
        {
            objectEventReference.ObjectDestroyed(gameObject);
        }
    }
}
