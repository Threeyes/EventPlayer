using UnityEngine;
using System.Text;
using Threeyes.Core.Editor;
using Threeyes.Core;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Threeyes.EventPlayer
{
    public class EventPlayer_PlayableInfo : EventPlayerWithParamBase<EventPlayer_PlayableInfo, PlayableInfoEvent, PlayableInfo>, ITimelineProgress
    {
        #region Property & Field

        public override string ValueToString { get { return ""; } }//PS: Don't Conver class data into string

        [Tooltip("Current Clip process percent")]
        public FloatEvent onProcessFrame;

        #endregion

        #region Inner Method

        public virtual void OnClipUpdate(PlayableInfo playableInfo)
        {
            PlayWithParam(playableInfo);
        }

        public void OnClipReset(PlayableInfo playableInfo)
        {
            NotifyListener<TLEPListener>((epl) => epl.OnReset(playableInfo));
        }

        public void OnClipComplete(PlayableInfo playableInfo)
        {
            NotifyListener<TLEPListener>((epl) => epl.OnComplete(playableInfo));
        }

        protected override void PlayWithParamFunc(PlayableInfo value)
        {
            onProcessFrame.Invoke(value.subPercent);
            base.PlayWithParamFunc(value);
        }

        #endregion

        #region Editor Method
#if UNITY_EDITOR

        //！！MenuItem！！
        static string instName = "PIEP ";
        [MenuItem(strMenuItem_Root_Extend + "PlayableInfo", false, intExtendMenuOrder + 0)]
        public static void CreateTimelineEventPlayer()
        {
            EditorTool.CreateGameObjectAsChild<EventPlayer_PlayableInfo>(instName);
        }

        //！！Hierarchy GUI！！
        public override string ShortTypeName { get { return "TL"; } }
        public override void SetHierarchyGUIProperty(StringBuilder sB)
        {
            base.SetHierarchyGUIProperty(sB);

            if (Value == null)
                return;

            sbCache.Length = 0;
            sbCache.Append((Value.time * 60).DateTimeFormat()).Append("/").Append((Value.duration * 60).DateTimeFormat());//A Trick to show millisecond
            sbCache.Append("(").Append((Value.percent * 100).ToString("#0.00")).Append("%").Append(")");

            AddSplit(sB, sbCache);
        }

        //！！Inspector GUI！！
        public override void SetInspectorGUIUnityEventProperty(GUIPropertyGroup group)
        {
            base.SetInspectorGUIUnityEventProperty(group);
            group.listProperty.Add(new GUIProperty(nameof(onProcessFrame)));
        }
        public override void SetInspectorGUICommonTextArea(StringBuilder sB)
        {
            base.SetInspectorGUICommonTextArea(sB);

#if Threeyes_Timeline
#else
            sB.AppendWarningRichText("You need to open the Setting Window and active Timeline support!");
            sB.Append("\r\n");
#endif
        }

#endif
        #endregion
    }
}
