using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public static class RuntimeTool
{

    static Dictionary<UnityAction, float> dicCacheCurFameAction = new Dictionary<UnityAction, float>();
    static Dictionary<UnityAction, float> dicCacheNextFameAction = new Dictionary<UnityAction, float>();

    /// <summary>
    /// Make sure the method execute once in cur frame (eg: heavy method like Resources.UnloadUnusedAssets )
    /// </summary>
    /// <param name="action"></param>
    public static async void ExecuteOnceInCurFrameAsync(UnityAction action)
    {
        try
        {
            int curFrameCount = Time.frameCount;
            if (dicCacheCurFameAction.ContainsKey(action) && dicCacheCurFameAction[action] == curFrameCount)
            {
                //Debug.Log("ExecuteOnceInOneFrame already contain in frame " + curFrameCount);
                return;
            }
            else
            {
                if (!dicCacheCurFameAction.ContainsKey(action))
                    dicCacheCurFameAction.Add(action, curFrameCount);
                action.Execute();
            }

            await Task.Yield();
            dicCacheCurFameAction.Remove(action);//Remove in next frame
        }
        catch (Exception e)
        {
            Debug.LogError("ExecuteOnceInCurFrameAsync error:\r\n" + e);
        }
    }

    /// <summary>
    /// Make sure the method execute once in next frame 
    /// (ToTest)
    /// </summary>
    /// <param name="action"></param>
    public static async void ExecuteOnceInNextFrameAsync(UnityAction action)
    {
        try
        {
            int curFrameCount = Time.frameCount;
            if (dicCacheNextFameAction.ContainsKey(action) && dicCacheNextFameAction[action] == curFrameCount)
            {
                //Debug.Log("ExecuteOnceInOneFrame already contain in frame " + curFrameCount);
                return;
            }
            else
            {
                if (!dicCacheNextFameAction.ContainsKey(action))
                    dicCacheNextFameAction.Add(action, curFrameCount);
            }

            await Task.Yield();
            action.Execute();
            dicCacheNextFameAction.Remove(action);//下一帧及时移除
        }
        catch (Exception e)
        {
            Debug.LogError("ExecuteOnceInCurFrameAsync error:\r\n" + e);
        }
    }
}
