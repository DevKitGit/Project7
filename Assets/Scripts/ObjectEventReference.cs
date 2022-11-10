using System;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

[CreateAssetMenu]
public class ObjectEventReference : ScriptableObject
{
    private event Action<Object> OnCreated;
    private event Action<Object> OnDestroyed;

    public void RegisterListener(Action<Object> onCreateMethod = null,Action<Object> onDestroyedMethod = null )
    {
        if (onCreateMethod != null)
        {
            OnCreated += onCreateMethod;
        }
        if (onDestroyedMethod != null)
        {
            OnDestroyed += onDestroyedMethod;
        }
    }
    
    public void UnregisterListener(Action<Object> onCreateMethod = null,Action<Object> onDestroyedMethod = null )
    {
        if (onCreateMethod != null)
        {
            OnCreated -= onCreateMethod;
        }
        if (onDestroyedMethod != null)
        {
            OnDestroyed -= onDestroyedMethod;
        }
    }
    
    public void ObjectCreated(Object obj)
    {
        OnCreated?.Invoke(obj);
    }
        
    public void ObjectDestroyed(Object obj)
    {
        OnDestroyed?.Invoke(obj);
    }
}