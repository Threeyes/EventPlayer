#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
/// <summary>
/// 将更改同步到Hierarchy上
/// </summary>
public class InspectorSyncWithHierarchyBase : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUI.BeginChangeCheck();

        OnInspectorGUIFunc();

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            EditorApplication.RepaintHierarchyWindow();
        }
    }

    public virtual void OnInspectorGUIFunc()
    {
        base.OnInspectorGUI();
    }
}
#endif