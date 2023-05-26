#if UNITY_EDITOR
using System.Linq;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Threeyes.Editor
{
    /// <summary>
    /// 编辑器通用的方法
    /// PS:不能放在Editor下，因为外部代码可能访问不了，或者是因为资源加载顺序错误导致无法访问
    /// </summary>
    public static class EditorTool
    {
        #region Repaint

        public static void RepaintSceneView()
        {
#if UNITY_EDITOR
            SceneView.RepaintAll();
#endif
        }

        public static void RepaintAllViews()
        {
#if UNITY_EDITOR
            InternalEditorUtility.RepaintAllViews();
#endif
        }

        public static void RepaintHierarchyWindow()
        {

#if UNITY_EDITOR
            EditorApplication.RepaintHierarchyWindow();
#endif
        }

        #endregion

        #region Component


        /// <summary>
        /// 强制更新某个Object，常用于Inspector组件的初始化。
        /// 用途：
        ///     ——通过AddComponent添加组件/某个组件首次被添加因此Reset被调用后，组件此时因为不在Inspector显示因此不会被初始化，UnityEvent等类实例字段会维持null，因此要调用这个方法强制更新
        ///Ref：https://forum.unity.com/threads/unity-event-is-null-right-after-addcomponent.819402/#post-5427855 #2
        /// </summary>
        public static void ForceUpdateObject(Object obj)
        {
            SerializedObject so = new SerializedObject(obj);
            so.Update();
        }
        #endregion

        #region Hierarchy
        /// <summary>
        /// Expand Gameobjects on Hierarchy window
        /// 
        /// ref: 
        /// </summary>
        /// <param name="go"></param>
        /// <param name="expand"></param>
        public static void SetExpandedRecursive(GameObject go, bool expand)
        {
            System.Type type = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
            MethodInfo methodInfo = type.GetMethod("SetExpandedRecursive");

            EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");

            EditorWindow editorWindow = EditorWindow.focusedWindow;

            methodInfo.Invoke(editorWindow, new object[] { go.GetInstanceID(), expand });
        }
        #endregion

        #region Scene

        public static T CreateGameObjectAsChild<T>(string name) where T : Component
        {
            return CreateGameObject<T>(name, Selection.activeGameObject ? Selection.activeGameObject.transform : null, isSameLayer: false);
        }

        public static T CreateGameObject<T>(string name, Transform tfParent = null, bool isSameLayer = true, bool isSelect = true) where T : Component
        {
            GameObject go = new GameObject(name);
            T com = go.AddComponent<T>();
            ForceUpdateObject(com);

            if (!tfParent)
                tfParent = Selection.activeGameObject ? Selection.activeGameObject.transform.parent : null;//Try to find the relate gameObject, will be world if the value is null

            go.transform.parent = tfParent;
            if (isSameLayer && Selection.activeGameObject)
                go.transform.SetSiblingIndex(Selection.activeGameObject.transform.GetSiblingIndex() + 1);//如果是同级，那就设置排到下面

            if (tfParent)
            {
                go.transform.localPosition = default(Vector3);
                go.transform.localRotation = default(Quaternion);
                go.transform.localScale = Vector3.one;
            }

            Undo.RegisterCreatedObjectUndo(go, "Create object");
            if (isSelect)
                Selection.activeGameObject = go;

            RepaintHierarchyWindow();
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
            Scene sceneTarget = SceneManager.GetActiveScene();
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

        #endregion

        #region Others

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

        #endregion

        #region Selection

        /// <summary> 
        /// 得到选中文件的相对路径
        /// </summary>    
        /// <returns></returns>   
        public static List<string> GetSelectionAssetPaths()
        {
            List<string> assetPaths = new List<string>();
            foreach (var guid in Selection.assetGUIDs) // 这个接口才能取到两列模式时候的文件夹  
            {
                if (string.IsNullOrEmpty(guid))
                    continue;
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (!string.IsNullOrEmpty(path))
                { assetPaths.Add(path); }
            }
            return assetPaths;
        }

        #endregion
    }
}
#endif