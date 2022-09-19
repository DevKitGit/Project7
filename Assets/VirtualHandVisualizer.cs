using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using System;
using System.Linq;
using Unity.VisualScripting;

public class VirtualHandVisualizer : MonoBehaviour
{
    
    [SerializeField] private TextAsset _textAsset;
    [SerializeField] private bool animated;

    private ArticulatedHandPose _staticGesture;
    public void UpdateStaticAnimation(Gesture gesture)
    {
    }
    
    private static bool ParseCsvGestureAnimation(TextAsset textAsset)
    {
        var jointIDs = GestureRecorder._jointIDs;
        var handedness = Handedness.None;
        var referenceJoint = TrackedHandJoint.None;
        var poses = new MixedRealityPose[jointIDs.Length];
        var data = textAsset.text.Split("\n");
        var descriptor = data[0].Split(";");
        var value = data[1].Split(";");
        for (int i = 0; i < jointIDs.Length; i++)
        {
            var jointName = jointIDs[i].ToString();
            var mixedRealityPose = new MixedRealityPose();
            var posHolder = new Vector3();
            var Quaturnionholder = new Quaternion();
            foreach (var t in descriptor)
            {
                if (t == $"{jointName}PosX")
                {
                    posHolder.x = float.Parse(value[i]);
                }
                if (t == $"{jointName}PosY")
                {
                    posHolder.y = float.Parse(value[i]);
                }
                if (t == $"{jointName}PosZ")
                {
                    posHolder.z = float.Parse(value[i]);
                }
                if (t == $"{jointName}RotX")
                {
                    Quaturnionholder.x = float.Parse(value[i]);
                }
                if (t == $"{jointName}RotY")
                {
                    Quaturnionholder.y = float.Parse(value[i]);
                }
                if (t == $"{jointName}RotZ")
                {
                    Quaturnionholder.z = float.Parse(value[i]);
                }
                if (t == $"{jointName}RotW")
                {
                    Quaturnionholder.w = float.Parse(value[i]);
                }
            }
            
            
        }
        return false;
    }
    // Start is called before the first frame update
    void Start()
    {
        ParseCsvGestureAnimation(_textAsset);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
