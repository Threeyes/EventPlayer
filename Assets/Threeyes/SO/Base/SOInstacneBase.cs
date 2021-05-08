using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// ScriptableObject资源单例类
/// </summary>
/// <typeparam name="T"></typeparam>
public class SOInstacneBase<T> : ScriptableObject where T : SOInstacneBase<T>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_inst">用于缓存的字段</param>
    /// <param name="pathInResources">//该Manager在Resources下的路径</param>
    /// <param name="name">文件名</param>
    /// <param name="actionOnCreate">创建后的Action</param>
    /// <returns></returns>
    public static T GetOrCreateInstance(ref T _inst, string name, string pathInResources = "", UnityAction<T> actionOnCreate = null)
    {
        //#缓存字段已经有引用，返回（可能是先前调用过）
        if (_inst)
        {
            //Debug.Log("Load From Cache property");
            return _inst;
        }

        string localDirectoryPath = "Assets/Resources";//Assets
        if (pathInResources != "")
            localDirectoryPath += "/" + pathInResources;// Assets/xxx
        string localFilePath = localDirectoryPath + "/" + name + ".asset";

        //#从Resources文件夹中获取
        if (!_inst)
        {
            string filePathinResources = name;
            if (pathInResources != "")
                filePathinResources = pathInResources + "/" + name;
            _inst = ResourceLoad<T>(filePathinResources);
            if (_inst)
            {
                //Debug.Log("Load From Directory: " + localDirectoryPath + "/" + name);
                return _inst;
            }
        }

        //#在本地文件夹中创建并获取引用
        if (!_inst)
        {
#if UNITY_EDITOR

            //@Editor模式下，通过IO创建文件夹，而不用Unity内置方法AssetDatabase.CreateFolder，因为其一次只能创建一层文件夹）（可能会因为Unity没有更新而报错）（ref：https://forum.unity.com/threads/assetdatabase-create-folders-recursively.787046/）

            string rootPath = Application.dataPath.Replace("Assets", "");//项目根目录
            if (!Directory.Exists(rootPath + "/" + localDirectoryPath))
            {
                Directory.CreateDirectory(rootPath + "/" + localDirectoryPath);
                //Debug.LogWarning("Create Directory: " + localDirectoryPath);
                AssetDatabase.Refresh();
            }

            if (!AssetDatabase.IsValidFolder(localDirectoryPath))
            {
                Debug.LogError("Directory not found! Try to manual refresh the editor !");
                return null;
            }

            _inst = ScriptableObject.CreateInstance<T>();
            _inst.OnCreate();
            actionOnCreate.Execute(_inst);
            AssetDatabase.CreateAsset(_inst, localFilePath);
            AssetDatabase.SaveAssets();
            //Debug.Log("Create " + name + ".asset");
#else
                Debug.LogError("Can't Create instance in Runtime!");
                // 如果仍然没有，就从默认状态中创建一个新的
                // CreateDefaultT函数可以是从JSON文件中读取，并且在实例化完后指明_instance.hideFlags = HideFlags.HideAndDontSave
                //_instance = CreateDefaultT();
#endif
        }

        if (!_inst)
        {
            Debug.LogError("Can't create instance: \r\n" + localFilePath);
        }
        return _inst;
    }

    public static T1 ResourceLoad<T1>(string pathInResources)
    where T1 : ScriptableObject
    {
        //便于在运行时获取
        return Resources.Load<T1>(pathInResources);
    }

    /// <summary>
    /// 首次被创建
    /// </summary>
    protected virtual void OnCreate()
    {

    }

}
