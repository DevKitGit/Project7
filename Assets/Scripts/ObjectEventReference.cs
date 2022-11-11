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

    private event Action<Object> OnDisabled;

    private event Action<Object> OnEnabled;

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
    public void RegisterListenerExtended(Action<Object> onCreateMethod = null,Action<Object> onDestroyedMethod = null ,Action<Object> onEnabledMethod = null,Action<Object> onDisabledMethod = null )
    {
        if (onCreateMethod != null)
        {
            OnCreated += onCreateMethod;
        }
        if (onDestroyedMethod != null)
        {
            OnDestroyed += onDestroyedMethod;
        }
        if (onEnabledMethod != null)
        {
            OnEnabled += onEnabledMethod;
        }
        if (onDisabledMethod != null)
        {
            OnDisabled += onDisabledMethod;
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
    
    public void UnregisterListenerExtended(Action<Object> onCreateMethod = null,Action<Object> onDestroyedMethod = null ,Action<Object> onEnabledMethod = null,Action<Object> onDisabledMethod = null )
    {
        if (onCreateMethod != null)
        {
            OnCreated -= onCreateMethod;
        }
        if (onDestroyedMethod != null)
        {
            OnDestroyed -= onDestroyedMethod;
        }
        if (onEnabledMethod != null)
        {
            OnEnabled -= onEnabledMethod;
        }
        if (onDisabledMethod != null)
        {
            OnDisabled -= onDisabledMethod;
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
    public void ObjectEnabled(Object obj)
    {
        OnEnabled?.Invoke(obj);
    }
        
    public void ObjectDisabled(Object obj)
    {
        OnDisabled?.Invoke(obj);
    }
}