#if UNITY_EDITOR
using UnityEngine;

namespace Threeyes.Editor
{
    public static class EditorPathTool
    {
        public static string GetAssetAbsPath(Object objAsset)
        {
            if (!objAsset)
                return "";
            return UnityRelateToAbsPath(UnityEditor.AssetDatabase.GetAssetPath(objAsset));
        }

        /// <summary>
        /// Unity local path to abs path
        /// </summary>
        /// <param name="unityPath"></param>
        /// <returns></returns>
        public static string UnityRelateToAbsPath(string unityPath)
        {
            string path = Application.dataPath;
            path = path.Replace("Assets", "");
            return path + unityPath;
        }
        public static string AbsToUnityRelatePath(string absPath)
        {
            //转换成同一样式
            absPath = PathTool.ConvertToSystemFormat(absPath);
            string appDataPath = PathTool.ConvertToSystemFormat(Application.dataPath);
            string relativepath = absPath;
            if (absPath.StartsWith(appDataPath))
            {
                relativepath = "Assets" + absPath.Substring(appDataPath.Length);
            }
            else
            {
                Debug.LogError("The path is not inside this Unity Project!\r\n" + absPath);
            }
            return relativepath;
        }

    }
}
#endif