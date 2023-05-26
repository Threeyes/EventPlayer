using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 生成场景中的唯一ID
/// 
/// 适用于：
/// 1.固定层级及名称（如DP）
/// </summary>
public static class SceneUniqueTool
{
    //PS:打包后的instanceID会变化，不能用于标识物体，弃用

    //ToUpdate:应该使用Transform的Path，需要加上根物体，或者是加上index
    //通过场景中的唯一标识序列化/反序列化绑定对应物体
    public static string GetSceneID(Component component, bool logIfNull = false)
    {
        string id = "";
        if (!component)
        {
            if (logIfNull)
                Debug.LogError("组件为空！");
        }
        else
        {
            id = GetGameObjectPath(component.gameObject);
        }
        return id;
    }
    public static TComoponent FindBySceneID<TComoponent>(string sceneID, Transform tfRoot = null)
        where TComoponent : Component
    {
        if (sceneID.IsNullOrEmpty())
        {
            Debug.LogError("ID 为空！");
        }
        else if (!tfRoot)
        {
            Debug.LogError("tfRoot 为空！");
        }
        else
        {
            //ToAdd:如果tfRoot为空，那就从所有场景中查找
            return tfRoot.FindFirstComponentInChild<TComoponent>(true, true, (com) => GetSceneID(com) == sceneID);//只需要检查是否能生成同一个ID即可
        }
        return null;
    }

    static string GetGameObjectPath(GameObject obj)
    {
        string path = obj.name + "#" + obj.transform.GetSiblingIndex();

        while (obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
            path = obj.name + "/" + path;
        }
        return path;
    }
}
