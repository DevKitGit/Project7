using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using System;
public class VirtualHandVisualizer : MonoBehaviour
{
    private ArticulatedHandPose[] articulatedHandPoseAnimation;
    private bool newAnimationReceived = false;

    public void UpdateAnimation(Gesture gesture)
    {
        articulatedHandPoseAnimation = ParseCSVGestureAnimation(gesture.file);
        newAnimationReceived = true;
    }

    private ArticulatedHandPose[] ParseCSVGestureAnimation(TextAsset textAsset)
    {
        string[] data = textAsset.text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
        return null;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
