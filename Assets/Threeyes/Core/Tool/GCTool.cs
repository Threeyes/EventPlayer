using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class GCTool
{
    public static void UnloadUnusedAssets()
    {
        RuntimeTool.ExecuteOnceInCurFrameAsync(() => Resources.UnloadUnusedAssets());
    }

    static int lastInvokeAsyncFrameCount = 0;
    /// <summary>
    /// Make sure Resources.UnloadUnusedAssets get call once on next frame 
    /// 
    /// 功能：
    /// 1.保证当前帧只能调用一次该方法
    /// 2.延迟到下一帧执行GC，便于用户在该帧完成其他操作
    /// </summary>
    public static async void UnloadUnusedAssetsAsync()
    {
        //ToUpdate:RuntimeTool增加
        int curFrameCount = Time.frameCount;
        if (lastInvokeAsyncFrameCount == curFrameCount)
            return;
        lastInvokeAsyncFrameCount = curFrameCount;

        await Task.Yield();//Wait till next frame 
        Resources.UnloadUnusedAssets();
    }
}
