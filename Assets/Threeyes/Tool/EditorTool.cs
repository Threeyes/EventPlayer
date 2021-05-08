using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

/// <summary>
/// 编辑器通用的方法
/// PS:不能放在Editor下，因为外部代码可能访问不了，或者是因为资源加载顺序错误导致无法访问
/// </summary>
public static class EditorTool
{
    public static Color HLColor = /*Application.HasProLicense() ? Color.blue : */Color.cyan;

    public const string CommonMenuItemPprefix = "Tools/";

    /// <summary>
    /// Colyu菜单的前缀
    /// </summary>
    public const string MenuItemPrefix = CommonMenuItemPprefix + "_Colyu/";

    public const string AssetMenuPrefix_SO = "SO/";

    public const string AssetMenuPrefix_SO_Build = AssetMenuPrefix_SO + "Build/";

#if UNITY_EDITOR

    public static void RepaintSceneView()
    {
#if UNITY_EDITOR
        SceneView.RepaintAll();
#endif
    }

    public static void RepaintHierarchyWindow()
    {

#if UNITY_EDITOR
        EditorApplication.RepaintHierarchyWindow();
#endif
    }


    public static T CreateGameObjectAsChild<T>(string name) where T : Component
    {
        return CreateGameObject<T>(name, Selection.activeGameObject ? Selection.activeGameObject.transform : null);
    }

    public static T CreateGameObject<T>(string name, Transform tfParent = null) where T : Component
    {
        GameObject go = new GameObject(name);
        T com = go.AddComponent<T>();

        if (!tfParent)//target parent or world
            tfParent = Selection.activeGameObject ? Selection.activeGameObject.transform.parent : null;

        go.transform.parent = tfParent;
        go.transform.localPosition = default(Vector3);
        if (Selection.activeGameObject)
            go.transform.SetSiblingIndex(Selection.activeGameObject.transform.GetSiblingIndex() + 1);

        if (tfParent)
            go.transform.localPosition = default(Vector3);

        Undo.RegisterCreatedObjectUndo(go, "Create object");
        Selection.activeGameObject = go;

        return com;
    }

    /// <summary>
    /// 选择并高亮
    /// </summary>
    /// <param name="obj"></param>
    public static void SelectAndHighlight(Object obj)
    {
#if UNITY_EDITOR
        EditorGUIUtility.PingObject(obj);
        Selection.activeObject = obj;
#endif
    }



    /// <summary>
    /// 找到场景中指定名字的物体
    /// </summary>
    /// <param name="targetName"></param>
    /// <returns></returns>
    public static GameObject FindTargetInScene(string targetName)
    {
        Scene sceneTarget = EditorSceneManager.GetActiveScene();
        GameObject goTarget = null;
        foreach (GameObject go in sceneTarget.GetRootGameObjects().ToList())
        {
            //修改：循环遍历
            go.transform.ForEachChildTransform(
                (tf) =>
                {
                    if (tf.gameObject.name == targetName)
                        goTarget = tf.gameObject;
                },
                true,
                true);

            if (goTarget != null)
                break;
        }
        return goTarget;
    }

    /// <summary>
    /// PlayClip on Editor (even not playing)
    /// </summary>
    /// <param name="clip"></param>
    public static void PlayClip(AudioClip clip)
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        System.Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "PlayClip",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new System.Type[] {
                typeof(AudioClip)
            },
            null
        );
        method.Invoke(
            null,
            new object[] {
                clip
            }
        );
    } // PlayClip()

    /// <summary>
    /// 
    /// </summary>
    public static void StopAllClips()
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        System.Type audioUtilClass =
              unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "StopAllClips",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new System.Type[] { },
            null
        );
        method.Invoke(
            null,
            new object[] { }
        );
    }

#endif

}

