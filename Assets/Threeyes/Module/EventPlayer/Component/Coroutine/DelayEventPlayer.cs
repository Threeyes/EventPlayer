using UnityEngine;
using System.Collections;
using System.Text;

namespace Threeyes.EventPlayer
{
#if UNITY_EDITOR
    using UnityEditor;
#endif

    /// <summary>
    /// Delay Invoke Play Event
    /// </summary>
    public class DelayEventPlayer : CoroutineEventPlayerBase
    {
        #region Property & Field

        public int DelayFrame { get { return defaultDelayFrame; } set { defaultDelayFrame = value; } }
        public float DelayTime { get { return defaultDelayTime; } set { defaultDelayTime = value; } }

        //[Header("Delay Setting")]
        [SerializeField]
        protected int defaultDelayFrame = -1;
        [SerializeField]
        protected float defaultDelayTime = 1;

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
            cacheEnum = Coroutine.CoroutineManager.StartCoroutineEx(IEDelayPlay(delayTime, delayFrame));
        }

        int coroutineUsedFrame = 0;
        IEnumerator IEDelayPlay(float delayTime, int delayFrame)
        {
            //If both property is not valid, instead of LogError, it will invoke event at once just like EventPlayer

            IsCoroutineRunning = true;
#if UNITY_EDITOR
            coroutineUsedFrame = 0;
            coroutineUsedTime = 0;
            if (IsLogOnPlay)
                print(name + " DelayEventPlay!");
#endif

            if (IsPropertyValid(delayFrame))
                for (int i = 0; i != delayFrame; i++)
                {
#if UNITY_EDITOR
                    coroutineUsedFrame++;
                    RepaintHierarchyWindow();
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
                    coroutineUsedTime += DeltaGameTime;
                    RepaintHierarchyWindow();
#endif

                    if (HasDestoryed)//In case get destroy
                        yield break;
                    yield return null;//Wait for next frame(DeltaGameTime will retrun the true value base on IsIgnoreTimeScale)
                }
            }

            IsCoroutineRunning = false;//PS:PlayFunc will RefreshEditor
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

        static string instName = "DelayEP ";

        [MenuItem(strSubCoroutineMenuItem + "DelayEventPlayer", false, intCoroutineMenuOrder + 0)]
        public static void CreateDelayEventPlayer()
        {
            EditorTool.CreateGameObject<DelayEventPlayer>(instName);
        }

        [MenuItem(strSubCoroutineMenuItem + "DelayEventPlayer Child", false, intCoroutineMenuOrder + 1)]
        public static void CreateDelayEventPlayerChild()
        {
            EditorTool.CreateGameObjectAsChild<DelayEventPlayer>(instName);
        }

        public override void SetHierarchyGUIType(StringBuilder sB)
        {
            sB.Append("D");
        }

        public override void SetHierarchyGUIProperty(StringBuilder sB)
        {
            base.SetHierarchyGUIProperty(sB);

            if (IsCoroutineRunning)
            {
                sbCache.Length = 0;

                float totalRunTime = (float)coroutineUsedFrame / 60 + coroutineUsedTime;
                sbCache.Append(GetRunningSymbol(totalRunTime));

                //Only show the valid time
                if (coroutineUsedFrame > 0)
                    sbCache.Append(coroutineUsedFrame).Append("f");
                if (coroutineUsedTime > 0)
                {
                    if (coroutineUsedFrame > 0)
                    {
                        sbCache.Append("+");
                    }
                    sbCache.Append(coroutineUsedTime.ToString("#0.00")).Append("s");
                }

                AddSplit(sB, sbCache);
            }


            //——Config——
            sbCache.Length = 0;
            if (IsIgnoreTimeScale)
                sbCache.Append("Ⓘ ");
            if (!IsPropertyValid(DelayTime) && !IsPropertyValid(DelayFrame))
            {
                EditorDrawerTool.AppendWarningText(sbCache, DelayFrame, "f", "+", DelayTime, "s");
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

        public override void SetInspectorGUISubProperty(GUIPropertyGroup group)
        {
            base.SetInspectorGUISubProperty(group);
            group.title = "Delay Setting";
            group.listProperty.Add(new GUIProperty("defaultDelayFrame", "DelayFrame"));
            group.listProperty.Add(new GUIProperty("defaultDelayTime", "DelayTime"));
        }

        public override void SetInspectorGUICommonTextArea(StringBuilder sB)
        {
            base.SetInspectorGUICommonTextArea(sB);
            if (DelayTime <= 0 && DelayFrame <= 0)
            {
                EditorDrawerTool.AppendWarningText(sB, "The total delay time is zero!");
                sB.Append("\r\n");
            }
        }
#endif
        #endregion
    }
}
