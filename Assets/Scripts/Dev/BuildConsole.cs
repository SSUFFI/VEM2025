using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

public class BuildConsole : MonoBehaviour
{
    [DllImport("kernel32.dll")]
    private static extern bool AllocConsole();

    [DllImport("kernel32.dll")]
    private static extern bool FreeConsole();

    void Awake()
    {
#if !UNITY_EDITOR && UNITY_STANDALONE_WIN

        AllocConsole();


        Application.logMessageReceived += LogToConsole;


        try
        {
            var standardOutput = Console.OpenStandardOutput();
            StreamWriter writer = new StreamWriter(standardOutput);
            writer.AutoFlush = true;
            Console.SetOut(writer);

            Console.WriteLine("=== Console Initialized ===");
        }
        catch (Exception e)
        {
            Debug.Log("Console init failed: " + e.Message);
        }
#endif
    }

    private void LogToConsole(string logString, string stackTrace, LogType type)
    {
        Console.WriteLine($"[{type}] {logString}");
        if (type == LogType.Exception || type == LogType.Error)
            Console.WriteLine(stackTrace);
    }

    void OnDestroy()
    {
#if !UNITY_EDITOR && UNITY_STANDALONE_WIN
        Application.logMessageReceived -= LogToConsole;
        FreeConsole();
#endif
    }
}
