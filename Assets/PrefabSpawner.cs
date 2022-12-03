using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    public void Spawn()
    {
        var position = CameraCache.Main.transform.position;
        var forward = CameraCache.Main.transform.forward;
        position = Vector3.MoveTowards(position, new Vector3(forward.x, position.y, forward.z).normalized, 1f);
        Instantiate(prefab, position, prefab.transform.rotation);
    }
}
