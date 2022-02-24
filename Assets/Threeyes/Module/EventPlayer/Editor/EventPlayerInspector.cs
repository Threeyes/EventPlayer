﻿#if UNITY_EDITOR

#define InspectorDebug

namespace Threeyes.EventPlayer
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using System.Text;
    using UnityEngine.Events;
    using System;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(EventPlayer), true)]//editorForChildClasses
    public class EventPlayerInspector : InspectorSyncWithHierarchyBase
    {
        bool IsMultiSelected
        {
            get { return serializedObject.isEditingMultipleObjects; }
        }
        readonly string strMultiEditingWarning = "Try not to Modify Events on multi Select, because they may have different value!";

        private EventPlayer targetEP;

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
            targetEP = target as EventPlayer;
            if (!targetEP)//避免部分自定义EP可能因为被停用导致转换失败
                return;

            DrawCommonTips();
            DrawButton(targetEP);//Draw Buttons on Top side

            if (targetEP.IsCustomInspector)
            {
                DrawUnityEventContent();
                DrawBasicContent();
                DrawSubContent();
            }
            else
            {
                base.OnInspectorGUIFunc();//PS:如果这里和自定义模块同时启用，会导致重复显示属性而不更新
            }

            DrawButton(targetEP);//Draw Buttons on Bottom side
        }

        public void DrawButton<T>(T ep) where T : EventPlayer
        {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            string playText = "Play";
            //Param
            IEventPlayerWithParam epWI = ep as IEventPlayerWithParam;
            if (epWI != null)
            {
                if (epWI.IsPlayWithParam)
                    playText = "Play" + "(" + epWI.ValueToString + ")";
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
                    ep.PlaybyID();
                }
            }
            GUI.enabled = true;
            GUILayout.EndVertical();
        }

        /// <summary>
        /// 通用提示
        /// </summary>
        StringBuilder sbCommonTips = new StringBuilder();
        private void DrawCommonTips()
        {
            sbCommonTips.Length = 0;
            targetEP.SetInspectorGUICommonTextArea(sbCommonTips);
            EditorDrawerTool.DrawTextArea(sbCommonTips, true);
        }

        GUIPropertyGroup gPPUnityEvent = new GUIPropertyGroup();
        protected virtual void DrawUnityEventContent()
        {
            gPPUnityEvent.Clear();
            targetEP.SetInspectorGUIUnityEventProperty(gPPUnityEvent);
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
            });

            //——Group Setting——
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

            //——Debug Setting——
            isFoldOutDebugSetting = EditorDrawerTool.DrawFoldOut(isFoldOutDebugSetting, new GUIContent("Debug Setting"),
            () =>
            {
                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                DrawSwitchButton_SupportMultiSelect(new GUIContent("LogOnPlay"), (e) => e.IsLogOnPlay, (e, b) => e.IsLogOnPlay = b);
                DrawSwitchButton_SupportMultiSelect(new GUIContent("LogOnStop"), (e) => e.IsLogOnStop, (e, b) => e.IsLogOnStop = b);
                GUILayout.EndHorizontal();
                EditorDrawerTool.DrawDisplayButton(new GUIContent("Cur State: " + targetEP.State.ToString()));
                GUILayout.EndVertical();
            });
        }

        GUIPropertyGroup gPPSub = new GUIPropertyGroup();
        /// <summary>
        /// 子类信息
        /// </summary>
        private void DrawSubContent()
        {
            gPPSub.Clear();
            targetEP.SetInspectorGUISubProperty(gPPSub);
            if (gPPSub.HasContent)//EventPlayer没有内容，不显示
                isFoldOutSubSetting = DrawFoldOut(gPPSub, isFoldOutSubSetting);
        }

        #region GUI Method

        void DrawPropertyField(string propertyPath, GUIContent gUIContent)
        {
            EditorDrawerTool.DrawPropertyField(serializedObject, propertyPath, gUIContent);//显示事件
        }

        protected bool DrawFoldOut(GUIPropertyGroup gUIPropertyGroup, bool isFoldout)
        {
            return EditorDrawerTool.DrawFoldOut(serializedObject, gUIPropertyGroup, isFoldout);
        }

        System.Enum DrawEnumPopup_SupportMultiSelect(GUIContent gUIContent, CustomFunc<EventPlayer, Enum> getter, UnityAction<EventPlayer, Enum> setter)
        {
            return EditorDrawerTool.DrawEnumPopup_SupportMultiSelect(targets, gUIContent, targetEP, getter, setter);
        }


        bool DrawSwitchButton_SupportMultiSelect(GUIContent gUIContent, CustomFunc<EventPlayer, bool> getter, UnityAction<EventPlayer, bool> setter)
        {
            return EditorDrawerTool.DrawSwitchButton_SupportMultiSelect(targets, gUIContent, targetEP, getter, setter);
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