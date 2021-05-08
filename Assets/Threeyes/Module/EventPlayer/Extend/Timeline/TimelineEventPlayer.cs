using UnityEngine;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Threeyes.EventPlayer
{
    public class TimelineEventPlayer : EventPlayerWithParamBase<TimelineEventPlayer, PlayableInfoEvent, PlayableInfo>, ITimelineProgress
    {
        #region Property & Field

        public override string ValueToString { get { return ""; } }//Don't Conver class data into string

        //[Header("Timeline Setting")]
        [Tooltip("Current Clip process percent")]
        public FloatEvent onProcessFrame;

        #endregion

        #region Inner Method

        public virtual void OnClipUpdate(PlayableInfo playableInfo)
        {
            Play(playableInfo);
        }

        protected override void PlayWithParamFunc(PlayableInfo value)
        {
            onProcessFrame.Invoke(value.percent);
            base.PlayWithParamFunc(value);
        }

        #endregion

        #region Editor Method
#if UNITY_EDITOR

        static string instName = "TimelineEP ";
        [MenuItem( strExtendMenuGroup + "TimelineEventPlayer", false, intExtendMenuOrder + 0)]
        public static void CreateTimelineEventPlayer()
        {
            EditorTool.CreateGameObject<TimelineEventPlayer>(instName);
        }
        [MenuItem( strExtendMenuGroup + "TimelineEventPlayer Child", false, intExtendMenuOrder + 1)]
        public static void CreateTimelineEventPlayerChild()
        {
            EditorTool.CreateGameObjectAsChild<TimelineEventPlayer>(instName);
        }

        public override void SetHierarchyGUIType(StringBuilder sB)
        {
            sB.Append("TL");
        }

        public override void SetHierarchyGUIProperty(StringBuilder sB)
        {
            base.SetHierarchyGUIProperty(sB);
            sbCache.Length = 0;

            sbCache.Append((Value.time * 100).ToString("00:00")).Append("/").Append((Value.duration * 100).ToString("00:00"));
            sbCache.Append("(").Append((Value.percent * 100).ToString("#0.00")).Append("%").Append(")");

            AddSplit(sB, sbCache);
        }

        public override void SetInspectorGUIUnityEventProperty(GUIPropertyGroup group)
        {
            base.SetInspectorGUIUnityEventProperty(group);
            group.listProperty.Add(new GUIProperty("onProcessFrame", "OnProcessFrame"));
        }

        public override void SetInspectorGUICommonTextArea(StringBuilder sB)
        {
            base.SetInspectorGUICommonTextArea(sB);

#if true // Threeyes_Timeline
#else
            EditorDrawerTool.AppendWarningText(sB, "You need to open the Setting Window and active Timeline support!");
            sB.Append("\r\n");
#endif
        }
#endif
        #endregion
    }
}
