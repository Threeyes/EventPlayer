#if UNITY_EDITOR
using Threeyes.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Threeyes.EventPlayer.Editor
{
    public class EventPlayerSettingWindow : EditorWindow
    {
        const string windowTitle = "EventPlayer Setting";
        static readonly Vector2 _WinSize = new Vector2(400, 400);

        SOEventPlayerSettingManager inst { get { return SOEventPlayerSettingManager.Instance; } }

        [MenuItem("Tools/Threeyes/" + windowTitle)]
        static void OpenWindow()
        {
            EventPlayerSettingWindow window = EditorWindow.GetWindow<EventPlayerSettingWindow>(true, windowTitle, true);
            window.minSize = _WinSize;
            window.maxSize = _WinSize;
            window.ShowUtility();
        }

        const string lineBreak = "└─";
        static Color colorGray = Color.white * 0.8f;
        static bool isSettingChange = false;
        void OnGUI()
        {
            EditorDrawerTool.RecordGUIColors();
            EditorGUI.BeginChangeCheck();

            //Display Setting
            GUILayout.BeginVertical(GUI.skin.box);
            EditorDrawerTool.DrawGroupTitleText("Display Setting");
            EditorDrawerTool.DrawSpace();
            DrawToggle(new GUIContent("Show property in Hierarchy Window"), () => inst.showPropertyInHierarchy,
                (b) =>
                {
                    inst.showPropertyInHierarchy = b;
                    EditorApplication.RepaintHierarchyWindow();
                }
                , ref isSettingChange);
            GUILayout.EndVertical();

            //Other Plugin Support
            GUILayout.BeginVertical(GUI.skin.box);
            EditorDrawerTool.DrawGroupTitleText("Extern Support");
            EditorDrawerTool.DrawSpace();

            //#TimeLine
            if (DrawToggle(new GUIContent("TimeLine", "Import Timeline before you active this!"), () => inst.useTimeline, (b) => inst.useTimeline = b, ref isSettingChange))
                DrawSubLines("[Unity.Timeline]", "[Unity.Timeline.Editor]");

            //#BezierSolution
            EditorDrawerTool.DrawSpace();
            if (DrawToggle(new GUIContent("BezierSolution"), () => inst.useBezierSolution, (b) => inst.useBezierSolution = b, ref isSettingChange))
                DrawSubLines("[BezierSolution.Runtime]");

            //#DoTweenPro
            EditorDrawerTool.DrawSpace();
            if (DrawToggle(new GUIContent("DoTweenPro"), () => inst.useDoTweenPro, (b) => inst.useDoTweenPro = b, ref isSettingChange))
            {
                DrawSubLines("[DOTweenPro.Scripts] (If Exists)");
                GUILayout.BeginHorizontal();
                DrawLineBreak();
                DrawToggle(new GUIContent("Preview (Still in development, Bug inside!)", "In preview mode, The Tween may not reset to origin state, if so, you can reload the scene without saving it"), () => inst.activeDoTweenProPreview, (b) => inst.activeDoTweenProPreview = b, ref isSettingChange);
                GUILayout.EndHorizontal();
            }

            //#VideoPlayer
            EditorDrawerTool.DrawSpace();
            DrawToggle(new GUIContent("VideoPlayer"), () => inst.useVideoPlayer, (b) => inst.useVideoPlayer = b, ref isSettingChange);

            EditorDrawerTool.DrawSpace();
            EditorDrawerTool.RecordGUIColors();

            if (GUILayout.Button("Apply"))
            {
                inst.RefreshDefine();
            }
            GUI.color = colorGray;
            GUILayout.Label("—Select and click Apply, then wait for compile to complete—", EditorDrawerTool.gUITitleText);//Hints
            GUILayout.Label("—Also you should link to the require asmdef—", EditorDrawerTool.gUITitleText);//Hints
            EditorDrawerTool.RestoreGUIColors();
            GUILayout.EndVertical();

            //Plugin Info
            GUILayout.BeginVertical(GUI.skin.box);
            EditorDrawerTool.DrawGroupTitleText("Info");
            GUILayout.Label("Version: " + inst.version);
            GUILayout.EndVertical();


            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(inst);//！需要调用该方法保存更改
            }

            //Debug.Log(isSettingChange);
        }

        void DrawSubLines(params string[] listText)
        {
            GUILayout.BeginHorizontal();
            DrawLineBreak();
            GUI.color = colorGray;
            GUILayout.Label(new GUIContent("Require asmdef: " + listText.ConnectToString("+")));
            EditorDrawerTool.RestoreGUIColors();
            GUILayout.EndHorizontal();
        }

        void DrawLineBreak()
        {
            GUILayout.Label(lineBreak, GUILayout.MaxWidth(16));
        }

        bool DrawToggle(GUIContent gUIContent, CustomFunc<bool> getter, UnityAction<bool> setter, ref bool isChanged)
        {
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
                isChanged |= true;//Mark as changed
            }
            return result;
        }
    }
}
#endif