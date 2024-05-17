//#define InspectorDebug
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using UnityEngine.Events;
using System;
using System.Linq;
using Threeyes.Core.Editor;
using Threeyes.Core;

namespace Threeyes.EventPlayer.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(EventPlayer), true)]//editorForChildClasses
    public class InspectorView_EventPlayer : InspectorViewSyncWithHierarchyBase
    {

        readonly string strMultiEditingWarning = "Try not to Modify Events on multi Select, because they may have different value!";

        private EventPlayer targetComp;

        static bool isFoldOutUnityEvent = true;
        static bool isFoldOutBasicSetting = true;
        static bool isFoldOutGroupSetting = true;
        static bool isFoldOutSubSetting = true;
        //默认为False，避免占据过多空间
        static bool isFoldOutIDSetting =
#if InspectorDebug
            true;
#else
            false;
#endif

        static bool isFoldOutDebugSetting =
#if InspectorDebug
            true;
#else
            false;
#endif

        public override void OnInspectorGUIFunc()
        {
            targetComp = target as EventPlayer;
            if (!targetComp)//避免部分自定义EP可能因为被停用导致转换失败
                return;

            DrawButton(targetComp);//Draw Buttons on Top side

            if (targetComp.IsCustomInspector)
            {
                //根据设定的顺序进行排序
                Dictionary<string, int> dic = new Dictionary<string, int>()
                {
                    { "UnityEvent",targetComp.InspectorUnityEventContentOrder },
                    { "Basic",targetComp.InspectorBasicContentOrder },
                    { "Group",targetComp.InspectorGroupContentOrder },
                    { "ID",targetComp.InspectorIDContentOrder },
                    { "Debug",targetComp.InspectorDebugContentOrder },
                    { "Sub",targetComp.InspectorSubContentOrder },
                };
                dic = dic.OrderBy(kp => kp.Value).ToDictionary(p => p.Key, o => o.Value); ;

                foreach (var keyPair in dic)
                {
                    if (keyPair.Value == 0)//0不绘制
                        continue;

                    if (keyPair.Key == "UnityEvent")
                        DrawUnityEventContent();
                    else if (keyPair.Key == "Basic")
                        DrawBasicContent();
                    else if (keyPair.Key == "Group")
                        DrawGroupContent();
                    else if (keyPair.Key == "ID")
                        DrawIDContent();
                    else if (keyPair.Key == "Debug")
                        DrawDebugContent();
                    else if (keyPair.Key == "Sub")
                        DrawSubContent();
                }
            }
            else
            {
                base.OnInspectorGUIFunc();//PS:如果这里和自定义模块同时启用，会导致重复显示属性而不更新
            }

            DrawButton(targetComp);//Draw Buttons on Bottom side
            DrawCommonTips();//提示应该放最后，避免用户更改设置后，导致菜单长度变化而丢失焦点

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                //Debug.Log("GUI.changed");
            }
        }

        public void DrawButton<T>(T ep) where T : EventPlayer
        {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            string playText = "Play";
            string stopText = "Stop";

            //Param
            var epWI = ep as IEventPlayerWithParam;
            if (epWI != null)
            {
                if (epWI.IsPlayWithParam)
                    playText = "Play" + "(" + epWI.ValueToString + ")";
                if (epWI.IsStopWithParam)
                    stopText = "Stop" + "(" + epWI.ValueToString + ")";
            }
            if (GUILayout.Button(playText))
            {
                InvokeSelection(ep, (e) => e.Play());
            }

            if (GUILayout.Button("Toggle"))
            {
                InvokeSelection(ep, (e) => e.TogglePlay());
            }

            if (GUILayout.Button("Stop"))
            {
                InvokeSelection(ep, (e) => e.Stop());
            }
            GUILayout.EndHorizontal();

            //Play By ID
            GUI.enabled = ep.TargetID.NotNullOrEmpty() && ep.IsInvokeByID;
            if (!IsMultiSelected)//不支持多选
            {
                if (GUILayout.Button("Play by ID" + "(\"" + ep.TargetID + "\")"))
                {
                    ep.PlayByID();
                }
            }
            GUI.enabled = true;
            GUILayout.EndVertical();
        }

        GUIPropertyGroup gPPUnityEvent = new GUIPropertyGroup();    
        protected virtual void DrawUnityEventContent()
        {
            gPPUnityEvent.Clear();
            targetComp.SetInspectorGUIUnityEventProperty(gPPUnityEvent);
            if (IsMultiSelected)
            {
                EditorDrawerTool.AppendWarningText(gPPUnityEvent.sbTipsTop, strMultiEditingWarning);//提示不建议多选编辑
                //gPPUnityEvent.listProperty.Clear();
            }
            isFoldOutUnityEvent = DrawFoldOut(gPPUnityEvent, isFoldOutUnityEvent);
        }
        protected virtual void DrawBasicContent()
        {
            //——Basic Setting——
            isFoldOutBasicSetting = EditorDrawerTool.DrawFoldOut(isFoldOutBasicSetting, new GUIContent("Basic Setting"),
            () =>
            {
                //Active
                GUILayout.BeginVertical(GUI.skin.box);
                if (DrawSwitchButton_SupportMultiSelect(new GUIContent("Active", "Is this EP avaliable"), (e) => e.IsActive, (e, b) => e.IsActive = b))
                {
                    GUILayout.BeginHorizontal();
                    DrawSwitchButton_SupportMultiSelect(new GUIContent("CanPlay"), (e) => e.CanPlay, (e, b) => e.CanPlay = b);
                    DrawSwitchButton_SupportMultiSelect(new GUIContent("CanStop"), (e) => e.CanStop, (e, b) => e.CanStop = b);
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();

                GUILayout.BeginHorizontal();
                DrawSwitchButton_SupportMultiSelect(new GUIContent("PlayOnAwake", "Invoke Play method on Awake"), (e) => e.IsPlayOnAwake, (e, b) => e.IsPlayOnAwake = b);
                DrawSwitchButton_SupportMultiSelect(new GUIContent("PlayOnce", "The Play Method can only be Invoked once"), (e) => e.IsPlayOnce, (e, b) => e.IsPlayOnce = b);
                DrawSwitchButton_SupportMultiSelect(new GUIContent("Reverse", "Reverse the Play/Stop behaviour"), (e) => e.IsReverse, (e, b) => e.IsReverse = b);
                GUILayout.EndHorizontal();

                DrawPropertyField("listListener", new GUIContent("Listener"));//Draw Listener
            });
        }
        protected virtual void DrawGroupContent()
        {   //——Group Setting——
            isFoldOutGroupSetting = EditorDrawerTool.DrawFoldOut(isFoldOutGroupSetting, new GUIContent("Group Setting"),
            () =>
            {
                GUILayout.BeginVertical(GUI.skin.box);
                if (DrawSwitchButton_SupportMultiSelect(new GUIContent("Group", "Manage Child's EP"), (e) => e.IsGroup, (e, b) => e.IsGroup = b))
                {
                    GUILayout.BeginHorizontal();
                    DrawSwitchButton_SupportMultiSelect(new GUIContent("IncludeHide", "Invoke child's EP even if the GameObject is hiding in Hierarchy"), (e) => e.IsIncludeHide, (e, b) => e.IsIncludeHide = b);
                    DrawSwitchButton_SupportMultiSelect(new GUIContent("Recursive", "True: set all childs. False: set childs in first layer"), (e) => e.IsRecursive, (e, b) => e.IsRecursive = b);
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            });
        }
        protected virtual void DrawIDContent()
        {
            //——ID Setting——
            isFoldOutIDSetting = EditorDrawerTool.DrawFoldOut(isFoldOutIDSetting, new GUIContent("ID Setting"),
           () =>
            {
                DrawPropertyField("id", new GUIContent("Self ID: "));
                GUILayout.BeginHorizontal();
                DrawSwitchButton_SupportMultiSelect(new GUIContent("InvokeByTargetID", "Invoke other EPs by TargetID"), (e) => e.IsInvokeByID, (e, b) => e.IsInvokeByID = b);
                DrawEnumPopup_SupportMultiSelect(new GUIContent(""), (e) => e.TargetIDLocation, (e, en) => e.TargetIDLocation = (TargetIDLocationType)en);//不显示标题，节省位置
                GUILayout.EndHorizontal();
                DrawPropertyField("targetId", new GUIContent("Target ID: "));
            });
        }
        protected virtual void DrawDebugContent()
        {
            //——Debug Setting——
            isFoldOutDebugSetting = EditorDrawerTool.DrawFoldOut(isFoldOutDebugSetting, new GUIContent("Debug Setting"),
            () =>
            {
                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                DrawSwitchButton_SupportMultiSelect(new GUIContent("LogOnPlay"), (e) => e.IsLogOnPlay, (e, b) => e.IsLogOnPlay = b);
                DrawSwitchButton_SupportMultiSelect(new GUIContent("LogOnStop"), (e) => e.IsLogOnStop, (e, b) => e.IsLogOnStop = b);
                GUILayout.EndHorizontal();
                EditorDrawerTool.DrawDisplayButton(new GUIContent("Cur State: " + targetComp.State.ToString()));
                GUILayout.EndVertical();
            });
        }

        GUIPropertyGroup gPPSub = new GUIPropertyGroup();
        /// <summary>
        /// 子类信息
        /// </summary>
        private void DrawSubContent()
        {
            //ToUpdate：应该是返回List，以便自定义多个模块
            gPPSub.Clear();
            targetComp.SetInspectorGUISubProperty(gPPSub);
            if (gPPSub.HasContent)//EventPlayer没有内容，不显示
                isFoldOutSubSetting = DrawFoldOut(gPPSub, isFoldOutSubSetting);
        }

        /// <summary>
        /// 通用提示
        /// </summary>
        StringBuilder sbCommonTips = new StringBuilder();
        private void DrawCommonTips()
        {
            sbCommonTips.Length = 0;
            targetComp.SetInspectorGUICommonTextArea(sbCommonTips);
            if (sbCommonTips.Length > 0)
                EditorDrawerTool.DrawTextArea(sbCommonTips.ToString());
        }

        #region GUI Method

        System.Enum DrawEnumPopup_SupportMultiSelect(GUIContent gUIContent, CustomFunc<EventPlayer, Enum> getter, UnityAction<EventPlayer, Enum> setter)
        {
            return EditorDrawerTool.DrawEnumPopup_SupportMultiSelect(targets, gUIContent, targetComp, getter, setter);
        }


        bool DrawSwitchButton_SupportMultiSelect(GUIContent gUIContent, CustomFunc<EventPlayer, bool> getter, UnityAction<EventPlayer, bool> setter)
        {
            return EditorDrawerTool.DrawSwitchButton_SupportMultiSelect(targets, gUIContent, targetComp, getter, setter);
        }


        /// <summary>
        /// 针对所选任意数量的组件进行统一调用方法，常用于按键
        /// 原因：默认Inspector的target只针对最后一个Comp
        /// </summary>
        /// <param name="comp"></param>
        /// <param name="action"></param>
        void InvokeSelection<TComp>(TComp comp, UnityAction<TComp> action) where TComp : Component
        {
            EditorDrawerTool.InvokeSelection(targets, action);
        }

        #endregion

    }

}
#endif
