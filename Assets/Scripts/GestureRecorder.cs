using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
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

    [SerializeField] private GestureRecorderEvents _gestureRecorderEvents;

    private Gesture currentGesture;
    
    private bool DoneRecording => Time.frameCount - _frameCountOffset == currentGesture.amountOfSampleFrames - 1;

    public TrackedHandJoint ReferenceJoint { get; set; } = TrackedHandJoint.IndexTip;
    
    private Vector3 jointPositionOffset = Vector3.zero;
    private Handedness recordingHand = Handedness.None;
    private bool isRecording = false, leftRecording = false, rightRecording = false;

    private LoggingManager _loggingManager;
    private int _frameCountOffset;
    private Stopwatch _timeOffset;
    private MixedRealityPose[] jointPoses;

    private string root =
        $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\P7GestureData";
    public static readonly TrackedHandJoint[] _jointIDs =
    {
        TrackedHandJoint.Wrist,
        TrackedHandJoint.Palm,
        TrackedHandJoint.ThumbMetacarpalJoint,
        TrackedHandJoint.ThumbProximalJoint,
        TrackedHandJoint.ThumbDistalJoint,
        TrackedHandJoint.ThumbTip,
        TrackedHandJoint.IndexMetacarpal,
        TrackedHandJoint.IndexKnuckle,
        TrackedHandJoint.IndexMiddleJoint,
        TrackedHandJoint.IndexDistalJoint,
        TrackedHandJoint.IndexTip,
        TrackedHandJoint.MiddleMetacarpal,
        TrackedHandJoint.MiddleKnuckle,
        TrackedHandJoint.MiddleMiddleJoint,
        TrackedHandJoint.MiddleDistalJoint,
        TrackedHandJoint.MiddleTip,
        TrackedHandJoint.RingMetacarpal,
        TrackedHandJoint.RingKnuckle,
        TrackedHandJoint.RingMiddleJoint,
        TrackedHandJoint.RingDistalJoint,
        TrackedHandJoint.RingTip,
        TrackedHandJoint.PinkyMetacarpal,
        TrackedHandJoint.PinkyKnuckle,
        TrackedHandJoint.PinkyMiddleJoint,
        TrackedHandJoint.PinkyDistalJoint,
        TrackedHandJoint.PinkyTip
    };

    public bool DebugMode;

    private void Start()
    {

        _loggingManager = FindObjectOfType<LoggingManager>();
        _loggingManager.SetEmail("NA");
        
        jointPoses = new MixedRealityPose[_jointIDs.Length];
        if (_gestureRecorderEvents != null)
        {
            _gestureRecorderEvents.OnGestureSelected += GestureSelected;
            _gestureRecorderEvents.OnStartRecordingUserGesture += StartRecordingUserGesture;
            
        }
    }

    private void OnApplicationQuit()
    {
        if (DebugMode)
        {
            var directoryInfo = new DirectoryInfo($"{root}\\Debug");
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
            _loggingManager.SetSavePath($"{root}\\Debug");
        }
        else
        {
            var currentLargestSubDirectoryNumber = new DirectoryInfo($"{root}").GetDirectories().Where(e => int.TryParse(e.Name,out _)).OrderByDescending(e => e.Name).ToArray();
            DirectoryInfo dirinfo;
            if (currentLargestSubDirectoryNumber.Length == 0)
            {
                 dirinfo = new DirectoryInfo($"{root}\\{0}");
            }
            else
            {
                dirinfo = new DirectoryInfo($"{root}\\{int.Parse(currentLargestSubDirectoryNumber[0].Name)+1}");
            }
            if (!dirinfo.Exists)
            {
                dirinfo.Create();
            }
            _loggingManager.SetSavePath(dirinfo.FullName);
        }
        _loggingManager.SaveAllLogs(true);
    }

    private void OnDestroy()
    {
        if (_gestureRecorderEvents != null)
        {
            _gestureRecorderEvents.OnGestureSelected -= GestureSelected;
            _gestureRecorderEvents.OnStartRecordingUserGesture -= StartRecordingUserGesture;
        }
    }

    private void GestureSelected(object o)
    {
        currentGesture = (Gesture)o;
    }
    
    public void StartRecordingUserGesture()
    {
        HandJointUtils.TryGetJointPose(ReferenceJoint, currentGesture.handedness, out MixedRealityPose joint);
        jointPositionOffset = joint.Position;
        recordingHand = currentGesture.handedness;
        _frameCountOffset = Time.frameCount;
        _timeOffset = Stopwatch.StartNew();
        
        if (_loggingManager.HasLog($"gesture{currentGesture.id.ToString()}"))
        {
            _loggingManager.DeleteLog($"gesture{currentGesture.id.ToString()}");
        }
        _loggingManager.CreateLog($"gesture{currentGesture.id.ToString()}");
        isRecording = true;
    }
    public void StopRecording()
    {
        _timeOffset.Reset();
        isRecording = false;
        _gestureRecorderEvents.DoneRecordingUserGesture();
    }

    private void Update()
    {
        if (!isRecording) return;
        if (DoneRecording)
        {
            StopRecording();
            return;
        }
        RecordFrame();
    }
    
        

    public void RecordFrame()
    {
        MeasureJointPoses();
        var log = GenerateLog();
        if (log == null)
        {
            return;
        }
        _loggingManager.Log($"gesture{currentGesture.id.ToString()}", log);
    }

    
    private void MeasureJointPoses()
    {
        var globalSpaceJointPoses = new MixedRealityPose[ArticulatedHandPose.JointCount];
        
        for (int i = 0; i < ArticulatedHandPose.JointCount; ++i)
        {
            HandJointUtils.TryGetJointPose((TrackedHandJoint)i, recordingHand, out globalSpaceJointPoses[i]);
        }
        var pose = new ArticulatedHandPose();
        pose.ParseFromJointPoses(globalSpaceJointPoses, recordingHand, Quaternion.identity, jointPositionOffset); //can also be replaced with identity?
        
        for (int i = 0; i < _jointIDs.Length; ++i)
        {
            jointPoses[i] = MixedRealityPose.ZeroIdentity;
            jointPoses[i] = pose.GetLocalJointPose(_jointIDs[i], recordingHand);
        }
    }
    
    public Dictionary<string, object> GenerateLog()
    {
        var output = new Dictionary<string, object>();
        output.Add("Handedness", recordingHand.ToString());
        output.Add("Timestamp", _timeOffset.ElapsedMilliseconds.ToString());
        output.Add("Framecount", Time.frameCount - _frameCountOffset);
        output.Add("SessionID", "NA");
        output.Add("Email", "NA");
        for (var i = 0; i < jointPoses.Length; ++i)
        {                                                      
            var itemString = _jointIDs[i].ToString();
            
            output.Add($"{itemString}PosX",jointPoses[i].Position.x);
            output.Add($"{itemString}PosY",jointPoses[i].Position.y);
            output.Add($"{itemString}PosZ",jointPoses[i].Position.z);
            
            output.Add($"{itemString}RotX",jointPoses[i].Rotation.x);
            output.Add($"{itemString}RotY",jointPoses[i].Rotation.y);
            output.Add($"{itemString}RotZ",jointPoses[i].Rotation.z);
            output.Add($"{itemString}RotW",jointPoses[i].Rotation.w);

        }
        return output;
    }
}