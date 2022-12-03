using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    void Update()
    {
        if (CameraCache.Main != null)
        {
            transform.LookAt(CameraCache.Main.transform.position, Vector3.forward);
        }
    }
}
