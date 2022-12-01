using System;
using System.Collections;
using System.Collections.Generic;
using Devkit.Modularis.Events;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    [SerializeField] private List<GameEvent> tasks;
    [SerializeField,ReadOnly] private int currentIndex = 0;

    private void Start()
    {
        tasks[currentIndex].RegisterListener(OnNextEvent);
    }

    private void OnNextEvent()
    {
        tasks[currentIndex].UnregisterListener(OnNextEvent);
        currentIndex++;
        tasks[currentIndex].RegisterListener(OnNextEvent);
        
    }
}
