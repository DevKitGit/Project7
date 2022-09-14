using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using Unity.VisualScripting;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class GestureRecorder : MonoBehaviour
{
    /// <summary>
    /// The joint positioned at the origin at the start of the recording.
    /// </summary>
    /// <remarks>
    /// <para>If the reference joint moves between start and stop of recording then final position is used as an jointPositionOffset.</para>
    /// <para>Example: A "poke" gesture can be simulated by moving the index finger forward between start and stop,
    /// giving an jointPositionOffset that creates a poking motion when interpolated.</para>
    /// </remarks>

    public ProgramSO programSo;

    public int _currProgramIndex = 0;
    public TrackedHandJoint ReferenceJoint { get; set; } = TrackedHandJoint.IndexTip;



    private Vector3 jointPositionOffset = Vector3.zero;
    private Handedness recordingHand = Handedness.None;
    private bool isRecording = false, leftRecording = false, rightRecording = false;

    [SerializeField] private int RecordedFrames;
    private LoggingManager _loggingManager;
    private int _frameCountOffset;
    private Stopwatch _timeOffset;
    private void Start()
    {
        
        _loggingManager = FindObjectOfType<LoggingManager>();
        if (_loggingManager == null)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
        _loggingManager.SetEmail("kval15@student.aau.dk");
    }

    public void ToggleRecordNextGesture()
    {
        isRecording = !isRecording;
        if (isRecording)
        {
            RecordHandStart(programSo.program[_currProgramIndex].handedness);
        }
        else
        {
            StopRecording();
        }
    }

    public void StopRecording()
    {
        _timeOffset.Reset();
        isRecording = false;
        _loggingManager.SaveLog($"gesture{programSo.program[_currProgramIndex].id.ToString()}",clear:true);
    }
    private void RecordHandStart(Handedness handedness)
    {
        HandJointUtils.TryGetJointPose(ReferenceJoint, handedness, out MixedRealityPose joint);
        jointPositionOffset = joint.Position;
        recordingHand = handedness;
        _frameCountOffset = Time.frameCount;
        _timeOffset = Stopwatch.StartNew();
        _loggingManager.CreateLog($"gesture{programSo.program[_currProgramIndex].id.ToString()}");
    }

    private void Update()
    {
        if (isRecording)
        {
            RecordFrame();
        }
    }
        

    public void RecordFrame()
    {
        HandJointUtils.TryGetJointPose(ReferenceJoint, recordingHand, out MixedRealityPose joint);
        MixedRealityPose[] jointPoses = new MixedRealityPose[ArticulatedHandPose.JointCount];
        for (int i = 0; i < ArticulatedHandPose.JointCount; ++i)
        {
            HandJointUtils.TryGetJointPose((TrackedHandJoint)i, recordingHand, out jointPoses[i]);
        }
        ArticulatedHandPose pose = new ArticulatedHandPose();
        pose.ParseFromJointPoses(jointPoses, recordingHand, Quaternion.identity, jointPositionOffset);
        var log = GenerateLog();
        if (log == null)
        {
            return;
        }
        _loggingManager.Log($"gesture{programSo.program[_currProgramIndex].id.ToString()}", log);
    }
    public void RecordHandStop()
    {
        MixedRealityPose[] jointPoses = new MixedRealityPose[ArticulatedHandPose.JointCount];
        for (int i = 0; i < ArticulatedHandPose.JointCount; ++i)
        {
            HandJointUtils.TryGetJointPose((TrackedHandJoint)i, recordingHand, out jointPoses[i]);
        }
        ArticulatedHandPose pose = new ArticulatedHandPose();
        pose.ParseFromJointPoses(jointPoses, recordingHand, Quaternion.identity, jointPositionOffset);
        recordingHand = Handedness.None;
    }

    public Dictionary<string, object> GenerateLog()
    {
        var output = new Dictionary<string, object>();
        output.Add("handedness", recordingHand.ToString());
        output.Add("frameCount",(Time.frameCount-_frameCountOffset).ToString());
        output.Add("time", _timeOffset.ElapsedMilliseconds.ToString());
        var jointposes = new MixedRealityPose[ArticulatedHandPose.JointCount];
        var pose = new ArticulatedHandPose();
        for (var i = 0; i < ArticulatedHandPose.JointCount; ++i)
        {
            var jointPose = MixedRealityPose.ZeroIdentity;
            HandJointUtils.TryGetJointPose((TrackedHandJoint) i, recordingHand, out jointPose);
            if (jointPose == MixedRealityPose.ZeroIdentity)
            {
                return null; //if a pose wasn't detected this frame, stop looking through the rest.
            }

            jointposes[i] = jointPose;

            var itemString = ((TrackedHandJoint)(i)).ToString();
            itemString = Char.ToLowerInvariant(itemString[0]) + itemString.Substring(1);
            if (itemString.Contains("Metacarpal"))
            {
                continue;
            }
            output.Add($"{itemString}PosX",jointPose.Position.x.ToString());
            output.Add($"{itemString}PosY",jointPose.Position.y.ToString());
            output.Add($"{itemString}PosZ",jointPose.Position.z.ToString());
            
            output.Add($"{itemString}RotX",jointPose.Rotation.x.ToString());
            output.Add($"{itemString}RotY",jointPose.Rotation.y.ToString());
            output.Add($"{itemString}RotZ",jointPose.Rotation.z.ToString());
        }
        pose.ParseFromJointPoses(jointposes, recordingHand, Quaternion.identity, jointPositionOffset);
        pose.GetLocalJointPose(ReferenceJoint, recordingHand);
        
        return output;
    }
}