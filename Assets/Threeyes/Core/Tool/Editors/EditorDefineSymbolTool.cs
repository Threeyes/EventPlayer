#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
namespace Threeyes.Editor
{
    public static class EditorDefineSymbolTool
    {
        public static void ModifyDefines(List<string> listDefineToAdd, List<string> listDefineToRemove)
        {
            //Debug.LogError("Begin ModifyDefines!");
            string strCurrentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);//获取当前平台的宏定义
            HashSet<string> hashSetNewDefines = new HashSet<string>(strCurrentDefines.Split(';'));//使用HashSet存储的优势是能自动排序，保证新旧的表能够正确比较

            //保证只会存在一次
            foreach (var element in listDefineToAdd)
            {
                hashSetNewDefines.Add(element);
            }
            foreach (var element in listDefineToRemove)
            {
                hashSetNewDefines.Remove(element);
            }

            // only touch PlayerSettings if we actually modified it.
            string strNewDefines = string.Join(";", hashSetNewDefines);
            if (strNewDefines != strCurrentDefines)
            {
                //Debug.LogError("Define Change!");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, strNewDefines);
            }
        }

        /// <summary>
        /// Add a custom define
        /// </summary>
        /// <param name="define"></param>
        /// <param name="buildTargetGroup"></param>
        public static void AddDefine(string define)
        {
            var listDefine = GetListDefine();
            if (!listDefine.Contains(define))
            {
                listDefine.Add(define);
                SetDefines(listDefine);
            }
        }

        /// <summary>
        /// Remove a custom define
        /// </summary>
        /// <param name="_define"></param>
        /// <param name="_buildTargetGroup"></param>
        public static void RemoveDefine(string define)
        {
            var listDefine = GetListDefine();
            if (listDefine.Contains(define))
            {
                listDefine.Remove(define);
                SetDefines(listDefine);
            }
        }

        public static List<string> GetListDefine()
        {
            return GetArrDefine().ToList();
        }

        /// <summary>
        /// PS:使用HashSet存储的优势是能自动排序，保证新旧的表能够正确比较
        /// </summary>
        /// <returns></returns>
        public static HashSet<string> GetHashSetDefine()
        {
            return new HashSet<string>(GetArrDefine());
        }
        static string[] GetArrDefine()
        {
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            return defines.Split(';');
        }

        public static void SetDefines(List<string> definesList)
        {
            var defines = string.Join(";", definesList.ToArray());
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defines);
        }
    }
}
#endif