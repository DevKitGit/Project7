using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnCondition : MonoBehaviour
{
    [SerializeField] private VRInputTypeReference _conditionReference;
    [SerializeField] private VRInputType requiredCondition;
    void OnEnable()
    {
        if (_conditionReference.Value != requiredCondition)
        {
            gameObject.SetActive(false);
        }
    }
}
