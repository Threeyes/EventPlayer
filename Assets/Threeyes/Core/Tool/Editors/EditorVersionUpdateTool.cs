using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor.Events;
#endif
namespace Threeyes.Editor
{
    public static class EditorDummy//PS:将至少一个类放在UNITY_EDITOR外，避免打包时找不到该命名空间
    { }

#if UNITY_EDITOR
    /// <summary>
    /// 将旧版本脚本或旧命名字段升级的工具方法
    /// </summary>
    public static class EditorVersionUpdateTool
    {
        /// <summary>
        /// 针对普通字段（bool/field）
        /// </summary>
        /// <typeparam name="TUnityObject"></typeparam>
        /// <param name="target"></param>
        /// <param name="oldObj"></param>
        /// <param name="newObj"></param>
        /// <returns></returns>
        public static bool TransferField<TUnityObject>(Object target, ref TUnityObject oldObj, ref TUnityObject newObj)
        {
            if (!oldObj.Equals(newObj))//双重判断，避免意外增加listOld的元素导致listData被覆盖
            {
                newObj = oldObj;
                //oldObj = null;
                UnityEditor.EditorUtility.SetDirty(target);// mark as dirty, so the change will be save into scene file
                return true;
            }
            return false;
        }
        /// <summary>
        /// 针对 场景引用或预制物
        /// </summary>
        public static bool TransferObject<TUnityObject>(Object target, ref TUnityObject oldObj, ref TUnityObject newObj)
     where TUnityObject : Object
        {
            if (oldObj && !newObj)//双重判断，避免意外增加listOld的元素导致listData被覆盖
            {
                newObj = oldObj;
                oldObj = null;
                UnityEditor.EditorUtility.SetDirty(target);// mark as dirty, so the change will be save into scene file
                return true;
            }
            return false;
        }

        public static bool TransferUnityEvent<TUnityEvent>(Object target, ref TUnityEvent oldObj, ref TUnityEvent newObj)
           where TUnityEvent : UnityEventBase
        {
            if (oldObj != null && newObj != null)
                if (oldObj.GetPersistentEventCount() > 0 && newObj.GetPersistentEventCount() == 0)//仅旧有数据，且新无数据时才会进行转移
                {
                    newObj = ReflectionTool.DeepCopy(oldObj);
                    //删掉oldObj所有持久化
                    while (oldObj.GetPersistentEventCount() > 0)
                        UnityEventTools.RemovePersistentListener(oldObj, 0);

                    UnityEditor.EditorUtility.SetDirty(target);// mark as dirty, so the change will be saved
                                                               //Debug.LogError(target.name);
                    return true;
                }
            return false;
        }
        /// <summary>
        /// 转移List
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="target"></param>
        /// <param name="listOld"></param>
        /// <param name="listNew"></param>
        public static bool TransferList<TData>(Object target, ref List<TData> listOld, ref List<TData> listNew)
        {
            if (listOld.Count > 0 && listNew.Count == 0)//双重判断，避免意外增加listOld的元素导致listData被覆盖
            {
                listNew = listOld.SimpleClone();
                listOld.Clear();
                UnityEditor.EditorUtility.SetDirty(target);// mark as dirty, so the change will be save into scene file
                return true;
            }
            return false;
        }
    }
#endif
}
