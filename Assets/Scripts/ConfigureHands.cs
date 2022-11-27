using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
public class ConfigureHands : MonoBehaviour
{
    private void Start()
    {
        PointerUtils.SetHandRayPointerBehavior(PointerBehavior.AlwaysOff);
        PointerUtils.SetGazePointerBehavior(PointerBehavior.AlwaysOff);
        PointerUtils.SetHandGrabPointerBehavior(PointerBehavior.AlwaysOff);
    }
}