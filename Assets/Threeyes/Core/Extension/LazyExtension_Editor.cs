#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LazyExtension_Editor
{

    #region Inst

    /// <summary>
    /// 生成一个保存预制物引用的新物体（Editor）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name=""></param>
    /// <returns></returns>
    public static T InstantiatePrefabEditor<T>(this T obj, Vector3 position, Quaternion rotation) where T : Object
    {
        GameObject newGo = null;
#if UNITY_EDITOR
        GameObject souGo = null;
        if (obj.GetType() == typeof(GameObject))
            souGo = obj as GameObject;
        else if (obj.GetType() == typeof(Component))
            souGo = (obj as Component).gameObject;

        newGo = UnityEditor.PrefabUtility.InstantiatePrefab(souGo) as GameObject;
        newGo.transform.position = position;
        newGo.transform.rotation = rotation;

        if (obj.GetType() == typeof(GameObject))
            return newGo as T;
        return newGo.GetComponent<T>();
#else
        return Object.Instantiate(obj, position, rotation);
#endif
    }

    #endregion

}
#endif