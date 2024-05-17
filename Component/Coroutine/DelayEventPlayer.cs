using UnityEngine;
using System.Collections;
using System.Text;
using Threeyes.Core.Editor;
using Threeyes.Core;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Threeyes.EventPlayer
{

    /// <summary>
    /// Delay Invoke Play Event
    /// </summary>
    [AddComponentMenu(EditorDefinition_EventPlayer.AssetMenuPrefix_Action_Coroutine + "DelayEventPlayer")]
    public class DelayEventPlayer : CoroutineEventPlayerBase
    {
        #region Property & Field

        public int DelayFrame { get { return defaultDelayFrame; } set { defaultDelayFrame = value; } }
        public float DelayTime { get { return defaultDelayTime; } set { defaultDelayTime = value; } }
        [SerializeField]
        protected int defaultDelayFrame = -1;
        [SerializeField]
        protected float defaultDelayTime = 1;

        //Editor Display
        protected int CoroutineUsedFrame { get { return coroutineUsedFrame; } set { coroutineUsedFrame = value; RepaintHierarchyWindow(); } }
        private int coroutineUsedFrame = 0;

        #endregion

        #region Public Method

        /// <summary>
        /// Stop the CountDown and Play at once
        /// </summary>
        public void InterruptAndPlayAtOnce()
        {
            TryStopCoroutine();//Stop theCoroutine at once
            base.PlayFunc();//Play at once
        }

        public void DelayFramePlay(int delayFrame)
        {
            DelayPlay(-1, delayFrame);
        }

        public void DelayTimePlay(float delayTime)
        {
            DelayPlay(delayTime, -1);
        }

        [System.Obsolete("Use DelayTimePlay instead")]
        public void DelayPlay(float delayTime)
        {
            DelayPlay(delayTime, -1);
        }

        public void DelayPlay(float delayTime, int delayFrame)
        {
            TryStopCoroutine();
            cacheEnum = CoroutineManager.StartCoroutineEx(IEDelayPlay(delayTime, delayFrame));
        }

        IEnumerator IEDelayPlay(float delayTime, int delayFrame)
        {
            //If all the properties are not valid, instead of LogError, it will invoke event at once just like EventPlayer

            IsCoroutineRunning = true;
            SetStateFunc(EventPlayer_State.Played);

#if UNITY_EDITOR
            CoroutineUsedFrame = 0;
            CoroutineUsedTime = 0;
            if (IsLogOnPlay)
                print(name + " DelayEventPlay!");
#endif

            if (IsPropertyValid(delayFrame))
                for (int i = 0; i != delayFrame; i++)
                {
#if UNITY_EDITOR
                    CoroutineUsedFrame++;
#endif
                    if (HasDestoryed)//In case get destroy
                        yield break;
                    yield return null;
                }

            if (IsPropertyValid(delayTime))
            {
                float tempStartTime = CurGameTime;
                float leftTime = delayTime;

                while (leftTime >= 0)
                {
                    leftTime -= DeltaGameTime;
#if UNITY_EDITOR
                    CoroutineUsedTime += DeltaGameTime;
#endif

                    if (HasDestoryed)//In case get destroy
                        yield break;
                    yield return null;//Wait for next frame(DeltaGameTime will retrun the true value base on IsIgnoreTimeScale)
                }
            }

            IsCoroutineRunning = false;//PS:PlayFunc will RefreshEditor
            if (HasDestoryed)//In case get destroy
                yield break;
            base.PlayFunc();

#if UNITY_EDITOR
            RepaintHierarchyWindow();
#endif
        }

        #endregion

        #region Inner Method

        protected override void PlayFunc()
        {
            DelayPlay(DelayTime, DelayFrame);
        }

        protected override void StopFunc()
        {
            IsCoroutineRunning = false;
            base.StopFunc();
        }

        bool IsPropertyValid(float value)
        {
            return value > 0;
        }

        #endregion

        #region Editor Method
#if UNITY_EDITOR

        //——MenuItem——
        static string instName = "DelayEP ";
        [MenuItem(strMenuItem_RootCoroutine + "DelayEventPlayer", false, intCoroutineMenuOrder + 0)]
        public static void CreateDelayEventPlayer()
        {
            EditorTool.CreateGameObjectAsChild<DelayEventPlayer>(instName);
        }
        [MenuItem(strMenuItem_Root_Collection + "DelayEPG Child", false, intCollectionMenuOrder + 4)]
        public static void CreateDelayEventPlayerGroupChild()
        {
            var eventPlayer = EditorTool.CreateGameObjectAsChild<DelayEventPlayer>("DelayEPG ");
            eventPlayer.IsGroup = true;
        }

        //——Hierarchy GUI——
        public override string ShortTypeName { get { return "D"; } }
        public override void SetHierarchyGUIProperty(StringBuilder sB)
        {
            base.SetHierarchyGUIProperty(sB);

            if (IsCoroutineRunning)
            {
                sbCache.Length = 0;

                float totalRunTime = (float)CoroutineUsedFrame / 60 + CoroutineUsedTime;
                sbCache.Append(GetRunningSymbol(totalRunTime));

                //Only show the valid time
                if (CoroutineUsedFrame > 0)
                    sbCache.Append(CoroutineUsedFrame).Append("f");
                if (CoroutineUsedTime > 0)
                {
                    if (CoroutineUsedFrame > 0)
                    {
                        sbCache.Append("+");
                    }
                    sbCache.Append(CoroutineUsedTime.ToString("#0.00")).Append("s");
                }

                AddSplit(sB, sbCache);
            }


            //——Config——
            sbCache.Length = 0;
            if (IsIgnoreTimeScale)
                sbCache.Append("Ⓣ ");
            if (!IsPropertyValid(DelayTime) && !IsPropertyValid(DelayFrame))
            {
                sbCache.AppendWarningRichText(DelayFrame, "f", "+", DelayTime, "s");
            }
            else
            {
                if (DelayFrame > 0)
                {
                    sbCache.Append(DelayFrame).Append("f");
                }
                if (DelayTime > 0)
                {
                    if (DelayFrame > 0)
                    {
                        sbCache.Append("+");
                    }
                    sbCache.Append(DelayTime).Append("s");
                }
            }
            AddSplit(sB, sbCache);
        }

        //——Inspector GUI——
        public override void SetInspectorGUISubProperty(GUIPropertyGroup group)
        {
            base.SetInspectorGUISubProperty(group);
            group.title = "Delay Setting";
            group.listProperty.Add(new GUIProperty(nameof(defaultDelayFrame), "DelayFrame"));
            group.listProperty.Add(new GUIProperty(nameof(defaultDelayTime), "DelayTime"));
        }
        public override void SetInspectorGUICommonTextArea(StringBuilder sB)
        {
            base.SetInspectorGUICommonTextArea(sB);
            if (DelayTime <= 0 && DelayFrame <= 0)
            {
                sB.AppendWarningRichText("The total delay time is zero!");
                sB.Append("\r\n");
            }
        }
#endif
        #endregion
    }
}
