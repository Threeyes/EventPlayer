using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Log various value
/// </summary>
[ExecuteInEditMode]
public class Remarker : MonoBehaviour
{
    public StringEvent onLog;

    public LogType logType = LogType.Log;
    public bool withName = false;//Print with name
    public string format;//Output format, can be null. EX:The Current number is {0}
    [HideInInspector]
    public string tips;//Draw by RemarkerInspector

    public bool isLogOnConsole = true;

    public void LogObject(object value)
    {
        LogObj(value);
    }
    public void Log(bool isTrue)
    {
        LogObj(isTrue ? "True" : "False");
    }

    public void Log(Vector2 value)
    {
        LogObj(value);
    }
    public void Log(Vector3 value)
    {
        LogObj(value);
    }
    public void Log(System.Enum value)
    {
        LogObj(value);
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

    public void Log(Color value)
    {
        LogObj(value);
    }
    public void Log(TextAsset value)
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
        if (isLogOnConsole)
        {
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

        onLog.Invoke(str);//For extern function call (eg: set text)
    }
}
