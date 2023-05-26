#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace Threeyes.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Remarker), true)]//editorForChildClasses
    public class InspectorView_Remarker : UnityEditor.Editor
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
            _target.tips = EditorDrawerTool.DrawTextArea(_target, _target.tips);

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                //Debug.Log("GUI.changed");
            }

        }
    }
}

#endif