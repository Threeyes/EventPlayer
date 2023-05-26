using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class LazyExtension_Scene
{
    static GameObject[] arrCacheRootGO;

    /// <summary>
    /// Returns all components of Type type in the scene
    /// </summary>
    /// <typeparam name="TClass"></typeparam>
    /// <param name="scene"></param>
    /// <param name="includeInactive"></param>
    /// <returns></returns>
    public static IEnumerable<TClass> GetComponents<TClass>(this Scene scene, bool includeInactive = false)
    {
        //通过场景遍历得到【测试100000个物体，用时0ms，可接受】
        if (scene.IsValid())
        {
            arrCacheRootGO = scene.GetRootGameObjects();
            TClass[] arrClass = new TClass[] { };
            for (int i = 0; i != arrCacheRootGO.Length; i++)//不使用foreach，能够大幅减少GC
            {
                arrClass = arrCacheRootGO[i].GetComponentsInChildren<TClass>(includeInactive);
                for (int j = 0; j != arrClass.Length; j++)
                    yield return arrClass[j];
            }
        }
    }
}