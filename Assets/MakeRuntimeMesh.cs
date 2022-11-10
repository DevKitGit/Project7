using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeRuntimeMesh : MonoBehaviour
{
    private void Reset()
    {
        if (GetComponent<MeshFilter>().sharedMesh == null)
        {
            GetComponent<MeshFilter>().sharedMesh = new Mesh();
        }
    }

    private void Start()
    {
        if (GetComponent<MeshFilter>().sharedMesh == null)
        {
            GetComponent<MeshFilter>().sharedMesh = new Mesh();
        }
        
    }
}
