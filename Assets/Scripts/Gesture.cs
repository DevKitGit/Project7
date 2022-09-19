using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

[CreateAssetMenu]

public class Gesture : ScriptableObject
{
    public GestureID id;
    public long duration;
    public Handedness handedness;
    public TextAsset file;
    public enum GestureID
    {
        None = 0,
        Fist,
        IndexMiddleUp,
        IndexUp,
        IndexUpThumbOut,
        ThumbOut,
    }
}
