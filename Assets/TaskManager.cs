using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Devkit.Modularis.References;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    [SerializeField] private StringReference _currentTaskDescription;
    [SerializeField] private VRInputTypeReference _condition;
    [SerializeField,ReadOnly] private int currentIndex = 0;
    [SerializeField] private bool _debugMode;
    [SerializeField] private List<StringReference> tasks;
    private LoggingManager _loggingManager;
    private Stopwatch _stopwatch;
    private readonly string _root = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\P7TaskData";
    

    private void Start()
    {
        _stopwatch = Stopwatch.StartNew();
        _loggingManager = FindObjectOfType<LoggingManager>();
        _loggingManager.SetEmail("NA");
        tasks[currentIndex].RegisterCallback(OnEventCompleted);
        _currentTaskDescription.Value = tasks[currentIndex];
    }

    private void OnEventCompleted()
    {
        LogEvent();
        _stopwatch.Restart();
        tasks[currentIndex].UnregisterCallback(OnEventCompleted);
        currentIndex++;
        
        if (currentIndex == tasks.Count)
        {
            Save();
            Application.Quit();
            return;
        }
        _currentTaskDescription.Value = tasks[currentIndex];
        tasks[currentIndex].RegisterCallback(OnEventCompleted);
    }

    private void LogEvent()
    {
        var output = new Dictionary<string, object>();
        output.Add("TaskID",currentIndex);
        output.Add("Condition", Enum.GetName(typeof(VRInputType), _condition.Value));
        output.Add("Task Description", tasks[currentIndex].Value);
        output.Add("TaskCompletionTimeInMillis",_stopwatch.ElapsedMilliseconds);
        output.Add("Timestamp", Time.time);
        output.Add("SessionID", "NA");
        output.Add("Email", "NA");
        output.Add("Real time", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        _loggingManager.Log("Tasks", output);
    }

    private void Save()
    {
        if (_debugMode)
        {
            var directoryInfo = new DirectoryInfo($"{_root}\\Debug");
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
            _loggingManager.SetSavePath($"{_root}\\Debug");
        }
        else
        {
            var currentLargestSubDirectoryNumber = new DirectoryInfo($"{_root}").GetDirectories().Where(e => int.TryParse(e.Name,out _)).OrderByDescending(e => e.Name).ToArray();
            DirectoryInfo dirinfo;
            if (currentLargestSubDirectoryNumber.Length == 0)
            {
                dirinfo = new DirectoryInfo($"{_root}\\{0}");
            }
            else
            {
                dirinfo = new DirectoryInfo($"{_root}\\{int.Parse(currentLargestSubDirectoryNumber[0].Name)+1}");
            }
            if (!dirinfo.Exists)
            {
                dirinfo.Create();
            }
            _loggingManager.SetSavePath(dirinfo.FullName);
        }
        _loggingManager.SaveAllLogs(true);
    }
}


