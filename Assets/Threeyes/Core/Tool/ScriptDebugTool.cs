using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ScriptDebugTool
{
    //Log How many time spends
    static System.Diagnostics.Stopwatch timer;
    public static void RecordStartTime()
    {
#if UNITY_EDITOR
        timer = new System.Diagnostics.Stopwatch();
        timer.Start();
#endif
    }

    public static void RecordStopTime()
    {
#if UNITY_EDITOR
        timer.Stop();
#endif
    }
    public static void LogUsedTime()
    {
#if UNITY_EDITOR
        Debug.Log("Time elapsed: " + timer.Elapsed.Milliseconds);
#endif
    }
}
