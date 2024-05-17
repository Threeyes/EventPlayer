#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Threeyes.Core.Editor;
using Threeyes.Core;

namespace Threeyes.EventPlayer
{
    /// <summary>
    /// ToDelete:统一改为CCU，能够自动检测并添加对应宏定义
    /// </summary>
    //[CreateAssetMenu(menuName = "Threeyes/Manager/EventPlayerSettingManager")]
    public class SOEventPlayerSettingManager : SOInstanceBase<SOEventPlayerSettingManager>
    {
        #region Property & Field

        //——Static——
        public static bool ShowPropertyInHierarchy { get { return Instance ? Instance.showPropertyInHierarchy : true; } set { if (Instance) Instance.showPropertyInHierarchy = value; } }
        public static SOEventPlayerSettingManager Instance
        {
            get
            {
                bool isFirstCreate = (_instance == null) && !hasInitVersion;//避免程序退出时销毁，导致需要重新设置
                SOEventPlayerSettingManager result = GetOrCreateInstance(ref _instance, defaultName, pathInResources, InitSettings);
                if (result && isFirstCreate)//首次创建：更新版本号
                {
                    result.UpdateVersion(EventPlayerVersionManager.EventPlayer_Version);
                }
                return result;
            }
        }
        static bool hasInitVersion = false;
        private static SOEventPlayerSettingManager _instance;
        static string defaultName = "EventPlayerSetting";
        static string pathInResources = "Threeyes";//该Manager在Resources下的路径，默认是Resources根目录


        public string version = "0.0.0"; //Last cache version

        //Display Setting
        public bool showPropertyInHierarchy = true;//Show info of subclass

        //Other Plugin Support Setting
        //[Header("Other Plugin Support")]
        public bool useTimeline = false;
        public bool useBezierSolution = false;
        public bool useDoTweenPro = false;
        public bool useVideoPlayer = false;
        public bool activeDoTweenProPreview = false;

        //RelatePath
        static readonly string baseExtendDir = "Base/Timeline";
        static readonly string epExtendDir = "EventPlayer/Extend";

        class DefineSymbolInfo
        {
            public bool ActiveState { get { return activeStateGetter(); } set { activeStateSetter(value); } }
            public DefineSymbol defineSymbol;
            Func<bool> activeStateGetter;
            Action<bool> activeStateSetter;

            public DefineSymbolInfo(DefineSymbol defineSymbol, Func<bool> activeStateGetter, Action<bool> activeStateSetter)
            {
                this.defineSymbol = defineSymbol;
                this.activeStateGetter = activeStateGetter;
                this.activeStateSetter = activeStateSetter;
            }
        }

        ////存储宏定义及激活状态
        List<DefineSymbolInfo> listDefineSymbolInfo
        {
            get
            {
                if (_listDefineSymbolInfo == null)
                {
                    _listDefineSymbolInfo = new List<DefineSymbolInfo>()
                    {
                    //Third-Party
                    new DefineSymbolInfo(new DefineSymbol("Threeyes_Timeline", "Timeline Event", "支持Timeline", baseExtendDir+"|"+ epExtendDir),()=>useTimeline,(v)=>useTimeline=v),
                    new DefineSymbolInfo(new DefineSymbol("Threeyes_BezierSolution", "BezierSolution Support", "支持BezierSolution", epExtendDir+"/"+"BezierSolution"),()=>useBezierSolution,(v)=>useBezierSolution=v),
                    new DefineSymbolInfo(new DefineSymbol("Threeyes_DoTweenPro", "DoTweenPro Support", "支持DoTweenPro", epExtendDir+"/"+"DoTweenPro"),()=>useDoTweenPro,(v)=>useDoTweenPro=v),

                    //BuiltIn
                    new DefineSymbolInfo(new DefineSymbol("Threeyes_VideoPlayer", "VideoPlayer Event", "支持VideoPlayer", epExtendDir+"/"+"Video"),()=>useVideoPlayer,(v)=>useVideoPlayer=v),
                    };
                }
                return _listDefineSymbolInfo;
            }
        }
        List<DefineSymbolInfo> _listDefineSymbolInfo = null;
        #endregion

        //根据PlayerSettings中的Define，初始化SO（便于项目迁移后的初始化）
        static void InitSettings(SOEventPlayerSettingManager sOEventPlayerSettingManager)
        {
            var listDefine = EditorDefineSymbolTool.GetListDefine();
            foreach (var element in sOEventPlayerSettingManager.listDefineSymbolInfo)
            {
                element.ActiveState = listDefine.Contains(element.defineSymbol.name);
            }
            EditorUtility.SetDirty(sOEventPlayerSettingManager);//！需要调用该方法保存更改

            //Debug.Log("SOEventPlayerSettingManager Init after creation!");
        }

        public void UpdateVersion(string currentVersion)
        {
            if (version != currentVersion)
            {
                //Update Setting
                Debug.Log("Update EventPlayerSetting version from " + version + " to " + currentVersion);
                version = currentVersion;
                EditorUtility.SetDirty(this);//先保存一次这个字段，否则后期的RefreshDefine会导致保存失败
                RefreshDefine();
                EditorUtility.SetDirty(this);//！需要调用该方法保存更改
            }
        }



        [ContextMenu("RefreshDefine")]
        public void RefreshDefine()
        {
            //ToUpdate:改用EditorDefineSymbolTool.ModifyDefines
            List<string> listDefineToAdd = new List<string>();
            List<string> listDefineToRemove = new List<string>();
            foreach (var element in listDefineSymbolInfo)
            {
                if (element.ActiveState == true)//激活状态
                    listDefineToAdd.Add(element.defineSymbol.name);
                else
                    listDefineToRemove.Add(element.defineSymbol.name);
            }
            EditorDefineSymbolTool.ModifyDefines(listDefineToAdd, listDefineToRemove);
        }

        void OnValidate()
        {
#if UNITY_EDITOR
            EditorApplication.RepaintHierarchyWindow();
#endif
        }
    }

    //[InitializeOnLoad]//Warning：Asset operations such as asset loading should be avoided in InitializeOnLoad methods. InitializeOnLoad methods are called before asset importing is completed and therefore the asset loading can fail resulting in a null object. To do initialization after a domain reload which requires asset operations use the AssetPostprocessor.OnPostprocessAllAssets callback. This callback supports all asset operations and has a parameter signaling if there was a domain reload.（https://docs.unity3d.com/ScriptReference/
    public static class EventPlayerVersionManager
    {
        public static readonly string EventPlayer_Version = "4.0.1"; //Plugin Version
        /////Bug:
        /////-在初始化时调用容易导致报错，改为在Instance实例创建时调用（因为[InitializeOnLoad]调用时，资源还未加载完成）
        //static EventPlayerVersionManager()
        //{
        //    if (SOEventPlayerSettingManager.Instance)
        //    {
        //        SOEventPlayerSettingManager.Instance.UpdateVersion(EventPlayer_Version);
        //    }
        //}
    }
}
#endif