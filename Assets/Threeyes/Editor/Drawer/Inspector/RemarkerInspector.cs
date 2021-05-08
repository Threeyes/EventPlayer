#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CanEditMultipleObjects]
[CustomEditor(typeof(Remarker), true)]//editorForChildClasses

public class RemarkerInspector : Editor
{
    private Remarker _target;

    void OnEnable()
    {
        _target = (Remarker)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Label("Tips");
        _target.tips = EditorDrawerTool.DrawTextArea(_target.tips);
    }

}

#endif