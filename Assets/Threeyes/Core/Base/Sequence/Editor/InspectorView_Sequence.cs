#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace Threeyes.Editor
{
    /// <summary>
    /// inherit to Update Hierarchy
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SequenceAbstract), true)]//editorForChildClasses
    public class InspectorView_Sequence : InspectorViewSyncWithHierarchyBase
    {
        static bool isFoldOutUnityEvent = true;

        private SequenceAbstract targetComp;

        public override void OnInspectorGUIFunc()
        {
            targetComp = target as SequenceAbstract;
            if (!targetComp)
                return;

            base.OnInspectorGUIFunc();
            DrawUnityEventContent();
        }

        GUIPropertyGroup gPPUnityEvent = new GUIPropertyGroup();
        protected virtual void DrawUnityEventContent()
        {
            gPPUnityEvent.Clear();
            targetComp.SetInspectorGUIUnityEventProperty(gPPUnityEvent);
            isFoldOutUnityEvent = DrawFoldOut(gPPUnityEvent, isFoldOutUnityEvent);
        }

        protected virtual void FoldOut_SequenceContent()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onBeforeSet"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onSet"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onReset"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onSetInvalid"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onFirst"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onFinish"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onComplete"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onCanSetPrevious"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onCanSetNext"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onSetPageNumberText"));
        }
    }
}
#endif
