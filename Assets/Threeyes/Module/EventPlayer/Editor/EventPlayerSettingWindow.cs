#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Threeyes.EventPlayer
{
    public class EventPlayerSettingWindow : EditorWindow
    {
        const string windowTitle = "EventPlayer Setting";
        static readonly Vector2 _WinSize = new Vector2(400, 400);

        SOEventPlayerSettingManager inst { get { return SOEventPlayerSettingManager.Instance; } }

        [MenuItem("Window/Threeyes/" + windowTitle)]
        static void OpenWindow()
        {
            EventPlayerSettingWindow window = EditorWindow.GetWindow<EventPlayerSettingWindow>(true, windowTitle, true);
            window.minSize = _WinSize;
            window.maxSize = _WinSize;
            window.ShowUtility();
        }
        void OnGUI()
        {
            EditorGUI.BeginChangeCheck();

            //Display Setting
            GUILayout.BeginVertical(GUI.skin.box);
            EditorDrawerTool.DrawGroupTitleText("Display Setting");
            EditorDrawerTool.DrawSpace();
            DrawSwitchButton(new GUIContent("Show property in Hierarchy Window"), () => inst.showPropertyInHierarchy,
                (b) =>
                {
                    inst.showPropertyInHierarchy = b;
                    EditorApplication.RepaintHierarchyWindow();
                }
                );
            GUILayout.EndVertical();

            //Other Plugin Support
            GUILayout.BeginVertical(GUI.skin.box);
            EditorDrawerTool.DrawGroupTitleText("Extern Support");
            EditorDrawerTool.DrawSpace();
            DrawSwitchButton(new GUIContent("TimeLine"), () => inst.useTimeline, (b) => inst.useTimeline = b);
            DrawSwitchButton(new GUIContent("VideoPlayer"), () => inst.useVideoPlayer, (b) => inst.useVideoPlayer = b);

            EditorDrawerTool.RecordGUIColors();
            GUI.color = Color.white * 0.9f;
            GUILayout.Label("Select which plugin you want to use then click Apply");//Hints
            EditorDrawerTool.RestoreGUIColors();

            if (GUILayout.Button("Apply"))
            {
                inst.RefreshDefine();
            }
            GUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(inst);//！需要调用该方法保存更改
            }
        }


        bool DrawSwitchButton(GUIContent gUIContent, CustomFunc<bool> getter, UnityAction<bool> setter)
        {
            //return EditorDrawerTool.DrawSwitchButton(gUIContent, getter, setter, inst);

            //这个Toggle比较明显
            bool curValue = getter();
            bool result = EditorGUILayout.ToggleLeft(gUIContent, getter());
            if (result != curValue)
            {
                if (setter != null)
                {
                    Undo.RecordObject(inst, "Changed Property");
                    setter(result);//Change  setting
                    EditorUtility.SetDirty(inst);
                }
            }
            return result;
        }
    }
}
#endif