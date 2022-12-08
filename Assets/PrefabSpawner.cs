using System.Collections;
using System.Collections.Generic;
using Devkit.Modularis.References;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    [SerializeField] private ObjectReference _parentReference;
    [SerializeField] private ObjectReference rightHandReference;
    
    [SerializeField] private GameObject prefab;
    
    public void Spawn()
    {
        var position = CameraCache.Main.transform.position;
        var forward = CameraCache.Main.transform.forward;
        var go = _parentReference.Value as GameObject;
        var rightHand = rightHandReference.Value as GameObject;
        position = Vector3.MoveTowards(position,rightHand.transform.position, 2f);
        var spawn =Instantiate(prefab, position, prefab.transform.rotation,go.transform);
        spawn.transform.localScale /= go.transform.localScale.x;
    }
}
