#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;

namespace Threeyes.EventPlayer
{
    /// <summary>
    /// 在切换PlayMode时，调用特定方法
    /// </summary>
    public class TimelinePreviewManager : MonoBehaviour
    {
        static readonly List<UnityAction> listAction = new List<UnityAction>();

        static readonly List<UnityAction> _TmpKeys = new List<UnityAction>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="go"></param>
        /// <param name="actionOnStop">Stop时调用的委托</param>
        public static void StartPreview(GameObject go, UnityAction actionOnStop)
        {
            bool isPreviewing = listAction.Count > 0;
            if (!isPreviewing) StartupGlobalPreview();
            AddAnimationToGlobalPreview(actionOnStop);
        }
        public static void StartPreview(UnityAction action)
        {
            bool isPreviewing = listAction.Count > 0;
            if (!isPreviewing) StartupGlobalPreview();
            AddAnimationToGlobalPreview(action);
        }
        static void AddAnimationToGlobalPreview(UnityAction action)
        {
            if (!listAction.Contains(action))
            {
                listAction.Add(action);
            }
        }
        public static void StopPreview(GameObject go)
        {
            //To Imple
        }
        public static void StopPreview(UnityAction action)
        {
            if (action == null)
                return;

            _TmpKeys.Clear();
            foreach (var cacheAction in listAction)
            {
                if (cacheAction == action)
                    _TmpKeys.Add(cacheAction);
            }
            StopPreview(_TmpKeys);
            _TmpKeys.Clear();

            if (listAction.Count == 0)
                StopAllPreviews();
            else
                InternalEditorUtility.RepaintAllViews();
        }

        static void StopPreview(List<UnityAction> listTempAction)
        {
            foreach (var action in listTempAction)
            {
                action.Execute();
            }
        }
        static void StartupGlobalPreview()
        {
            UnityEditor.EditorApplication.playModeStateChanged += StopAllPreviews;
        }

#if !(UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_5)
        public static void StopAllPreviews(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
                StopAllPreviews();
        }
#endif

        public static void StopAllPreviews()
        {
            StopPreview(listAction);
            _TmpKeys.Clear();
            listAction.Clear();

            EditorApplication.playModeStateChanged -= StopAllPreviews;
            InternalEditorUtility.RepaintAllViews();
        }
    }

}
#endif