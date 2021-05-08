using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Log Tips
/// </summary>
[ExecuteInEditMode]
public class Remarker : MonoBehaviour
{
    public LogType logType = LogType.Log;

    public bool withName = false;//Print with name
    public string format;//Output format, can be null. EX:The Current number is {0}
    [HideInInspector]
    public string tips;

    public void Log(bool isTrue)
    {
        LogObj(isTrue ? "True" : "False");
    }
    public void Log(float value)
    {
        LogObj(value);
    }

    public void Log(int value)
    {
        LogObj(value);
    }

    public void Log(string value)
    {
        LogObj(value);
    }
    public void Log()
    {
        LogObj(tips);
    }

    void LogObj(params object[] value)
    {
        string str = "";

        if (!string.IsNullOrEmpty(format))
        {
            try
            {
                str += string.Format(format, value);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Format Error！" + e);
                foreach (var s in value)
                {
                    str += s;
                }
            }
        }
        else
        {
            foreach (var s in value)
            {
                str += s;
            }
        }

        if (withName)
        {
            str = "\"" + gameObject.name + "\": " + str;
        }
        switch (logType)
        {
            case LogType.Log:
                Debug.Log(str); break;
            case LogType.Warning:
                Debug.LogWarning(str); break;
            case LogType.Error:
                Debug.LogError(str); break;
        }
    }
}
