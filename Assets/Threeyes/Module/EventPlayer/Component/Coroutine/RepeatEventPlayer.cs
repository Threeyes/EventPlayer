using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using Threeyes.Coroutine;
using UnityEngine.Playables;
#if UNITY_EDITOR
using Threeyes.Editor;
#endif

namespace Threeyes.EventPlayer
{
    /// Repeat Invoke Play Event
    ///
    /// Main Idea:
    /// - If only one of these property is set to less than or equal to 0, it will be calculate by the other property（The formula：DeltaTime × Count = Duration）
    /// - If  only DeltaTime is avaliable, it will never Stop
    /// - Check the following form( '?' means that property is not valiable):
    ///      DeltaTime | Count | Duration | (Result)                              | Editor UI
    ///        --------------------------------------------------------------
    ///         |              |             |                |                                            | [a×b:c]
    ///        --------------------------------------------------------------
    ///         |      ?       |             |                | (Calculate this property)  | [(Auto Calculate)×b:c]
    ///        --------------------------------------------------------------
    ///         |              |      ?      |                | (Calculate this property)  | [a×(Auto Calculate):c ]
    ///        --------------------------------------------------------------
    ///         |              |              |       ?       | (Calculate this property)  | [a×b:(Auto Calculate)]
    ///        --------------------------------------------------------------
    ///         |               |      ?      |       ?       | (Infinite Invoke)                | [a×∞]
    ///        --------------------------------------------------------------
    ///         |      ?       |      ?      |               | (×Not avaliable!)               | [invalid!]
    ///        --------------------------------------------------------------
    ///         |      ?       |              |       ?       | (×Not avaliable!)              | 
    ///        --------------------------------------------------------------
    ///         |      ?       |      ?       |      ?       | (×Not avaliable!)              |
    ///        --------------------------------------------------------------

    /// /// <summary>
    /// Repeat Invoke Play Event
    /// </summary>

    public class RepeatEventPlayer : CoroutineEventPlayerBase
    {
        #region Property & Field

        public bool IsPlayOnRepeatStart { get { return isPlayOnRepeatStart; } set { isPlayOnRepeatStart = value; } }
        public float DeltaTime { get { return replayDeltaTime; } set { replayDeltaTime = value; } }
        public int ReplayCount { get { return replayCount; } set { replayCount = value; } }
        public float Duration { get { return defaultDuration; } set { defaultDuration = value; } }

        //[Header("Repeat Setting")]
        [Tooltip("Play the event once Invoke RepeatPlay")]
        [SerializeField]
        protected bool isPlayOnRepeatStart = true;
        [SerializeField]
        [Tooltip("replay deltaTime.  if less than or equal to 0, it will auto calculate the average deltaTime")]
        protected float replayDeltaTime = 1;
        [SerializeField]
        [Tooltip("Replay count, after completed it will invoke Stop, only work when larger than 0")]
        protected int replayCount = 1;
        [SerializeField]
        [Tooltip("Total repeat time, if less than or equal to 0, it will never stop")]
        protected float defaultDuration = 1;

        #endregion

        #region Public Method

        public void RepeatPlay(float deltaTime, int replayCount, float duration)
        {
            TryStopCoroutine();
            cacheEnum = CoroutineManager.StartCoroutineEx(IERepeatPlay(deltaTime, replayCount, duration));
        }

        #endregion

        #region Inner Method

        protected override void PlayFunc()
        {
            RepeatPlay(DeltaTime, ReplayCount, Duration);
        }

        protected override void StopFunc()
        {
            IsCoroutineRunning = false;
            base.StopFunc();
        }

        int curPlayedCount = 0;
        IEnumerator IERepeatPlay(float deltaTime, int replayCount, float duration)
        {
            //Trim the follow situation：[?,?,O] [?,O,?] [?,?,?]
            if (!IsConfigValid)
            {
                Debug.LogError("The Config is invalid!");
                yield break;
            }

#if UNITY_EDITOR
            CoroutineUsedTime = 0;
            curPlayedCount = 0;
            if (IsLogOnPlay)
                print(name + " RepeatEventPlay!");
#endif

            //Convert [?,O,O] [O,?,O][O,O,?] +  into [O,O,O]
            ConvertToRunTimeConfig(ref deltaTime, ref replayCount, ref duration);

            if (IsConfigOverflow(deltaTime, replayCount, duration))
            {
                Debug.LogError("The Config is overflow!");
                yield break;
            }

            IsCoroutineRunning = true;
            SetStateFunc(EventPlayer_State.Played);

            //To avoid the missing invoke due to Time trouble (避免因为时间精度而少调用一次事件)
            bool isPerfectMatch = false;//It set to true, it will invoke the exact count before stop
            if (duration != float.PositiveInfinity && duration % deltaTime == 0)//(检查是否能整除)
            {
                isPerfectMatch = true;
            }

            float tempStartTime = CurGameTime; ;
            float lastInvokeTime = tempStartTime;
            bool hasPlayOnRepeatBegin = false;
            float curTime = 0;

            while (true)
            {
                curTime = CurGameTime;
                if ((IsPlayOnRepeatStart && !hasPlayOnRepeatBegin) || (curTime - lastInvokeTime >= deltaTime))
                {
                    base.PlayFunc();
                    curPlayedCount++;
                    lastInvokeTime = curTime;
                    hasPlayOnRepeatBegin = true;
                }

                //Duration time Detect
                if (duration == float.PositiveInfinity)//[O,？,?] (Infinite Invoke)
                {

                }
                else if (IsPropertyValid(duration) && (curTime - tempStartTime >= duration))//The duration time is over
                {
                    //Invoke the exact count before stop (如果在结束前次数还没调用完，那就主动调用一次(可能导致原因：帧率问题导致时间采样精度不够))
                    if (isPerfectMatch && curPlayedCount < replayCount)
                    {
                        base.PlayFunc();
                        curPlayedCount++;
                    }
                    if (curPlayedCount < replayCount)//Log if there is not invoke the exact count
                        Debug.LogWarning("Total invoke count is: " + curPlayedCount + " / " + replayCount + ", please check the config");

                    StopFunc();
                    yield break;
                }

                //RepeatCount Detect
                if (IsPropertyValid(replayCount) && curPlayedCount >= replayCount)//The replayCount is completed
                {
                    StopFunc();
                    yield break;
                }

#if UNITY_EDITOR
                CoroutineUsedTime = curTime - lastInvokeTime;
#endif

                if (HasDestoryed)//In case get destroy
                    yield break;
                yield return null;
            }
        }

        /// <summary>
        ///  Convert [?,O,O] [O,?,O][O,O,?]+ [O,？,?]into [O,O,O] (auto convert all the property to valid)
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <param name="replayCount"></param>
        /// <param name="duration">if [O,？,?], change the value to PositiveInfinity</param>
        public void ConvertToRunTimeConfig(ref float deltaTime, ref int replayCount, ref float duration)
        {
            List<bool> listValiable = new List<bool>
            {
                IsPropertyValid(deltaTime),
                IsPropertyValid(replayCount),
                IsPropertyValid(duration)
            };
            //Calculate all the not valiable property (Convert [?,O,O] [O,?,O][O,O,?] into [O,O,O])
            if (listValiable.FindAll((b) => b == false).Count == 1)//Find out there is only one not valiable property
            {
                if (!IsPropertyValid(deltaTime))
                {
                    deltaTime = duration / replayCount;
                }
                if (!IsPropertyValid(replayCount))
                {
                    replayCount = (int)(duration / deltaTime);
                }
                if (!IsPropertyValid(duration))
                {
                    duration = deltaTime * (replayCount > 0 && IsPlayOnRepeatStart ? replayCount - 1 : replayCount);//减去Start的调用
                }
            }
            //[O,？,?]
            else if (IsPropertyValid(DeltaTime) && !IsPropertyValid(replayCount) & !IsPropertyValid(duration))
            {
                duration = float.PositiveInfinity;
            }
        }

        public bool IsConfigValid
        {
            get
            {
                //Check Avaliable
                if (!IsPropertyValid(DeltaTime))
                {
                    if (!IsPropertyValid(ReplayCount) || !IsPropertyValid(Duration))
                    {
                        //[?,?, O] [?, O,?] [?,?,?]
                        return false;
                    }
                }
                return true;
            }
        }

        bool IsConfigOverflow(float deltaTime, int replayCount, float duration)
        {
            int tempCount = IsPlayOnRepeatStart ? replayCount - 1 : replayCount;//如果在开始的时候就调用，那就减去开头的调用次数
            return deltaTime * tempCount - duration > 0.01f;
        }

        bool IsPropertyValid(float value)
        {
            return value > 0;
        }

        #endregion

        #region Editor Method

#if UNITY_EDITOR

        //——MenuItem——
        static string instName = "RepeatEP ";
        [UnityEditor.MenuItem(strMenuItem_RootCoroutine + "RepeatEventPlayer", false, intCoroutineMenuOrder + 1)]
        public static void CreateRepeatEventPlayer()
        {
            EditorTool.CreateGameObjectAsChild<RepeatEventPlayer>(instName);
        }

        //——Hierarchy GUI——
        public override string ShortTypeName { get { return "R"; } }
        public override void SetHierarchyGUIProperty(StringBuilder sB)
        {
            base.SetHierarchyGUIProperty(sB);

            float deltaTime = DeltaTime;
            int replayCount = ReplayCount;
            float duration = Duration;

            //Show Coroutine
            if (IsCoroutineRunning)
            {
                sbCache.Length = 0;
                sbCache.Append(GetRunningSymbol(CoroutineUsedTime)).Append("#").Append(curPlayedCount + 1).Append(" ").Append(CoroutineUsedTime.ToString("#0.00")).Append("s");
                AddSplit(sB, sbCache);
            }

            //——Config——
            sbCache.Length = 0;
            if (IsIgnoreTimeScale)
                sbCache.Append("Ⓘ ");

            //Not Valid [?,?, O] [?, O,?] [?,?,?]
            if (!IsConfigValid)
            {
                if (!IsPropertyValid(DeltaTime))
                    sbCache.AppendWarningRichText(deltaTime, "s");
                else
                    sbCache.Append("deltaTime").Append("s");
                sbCache.Append("×");
                if (!IsPropertyValid(ReplayCount))
                    sbCache.AppendWarningRichText(replayCount);
                else
                    sbCache.Append(replayCount);
                sbCache.Append(":");
                if (!IsPropertyValid(Duration))
                    sbCache.AppendWarningRichText(duration, "s");
                else
                    sbCache.Append(duration).Append("s");
                AddSplit(sB, sbCache);
                return;
            }

            ConvertToRunTimeConfig(ref deltaTime, ref replayCount, ref duration);

            //[O,？,?](Infinity)
            if (duration == float.PositiveInfinity)
            {
                sbCache.Append(deltaTime).Append("s").Append("×").Append("∞");
            }
            else
            {
                string playAhead = IsPlayOnRepeatStart ? "1+" : "";
                string strDeltaTime = deltaTime + "s";
                string strRepeatCount = (IsPlayOnRepeatStart ? replayCount - 1 : replayCount).ToString("F0");//减去首次的调用
                string strDuration = duration + "s";

                //Check Raw Data and Mark which property is auto generated
                if (!IsPropertyValid(DeltaTime))
                    strDeltaTime = "<" + strDeltaTime + ">";
                if (!IsPropertyValid(ReplayCount))
                    strRepeatCount = "<" + strRepeatCount + ">";
                if (!IsPropertyValid(Duration))
                    strDuration = "<" + strDuration + ">";

                sbCache.Append(playAhead).Append(strDeltaTime).Append("×").Append(strRepeatCount).Append(":");

                //overflow warning
                bool isOverFLow = IsConfigOverflow(deltaTime, replayCount, duration);
                if (isOverFLow)
                {
                    sbCache.AppendWarningRichText(strDuration);
                }
                else
                    sbCache.Append(strDuration);
            }

            AddSplit(sB, sbCache);
        }

        //——Inspector GUI——
        public override void SetInspectorGUISubProperty(GUIPropertyGroup group)
        {
            base.SetInspectorGUISubProperty(group);
            group.title = "Repeat Setting";
            group.listProperty.Add(new GUIProperty(nameof(isPlayOnRepeatStart), "PlayOnStart"));
            group.listProperty.Add(new GUIProperty(nameof(replayDeltaTime), "ReplayDeltaTime"));
            group.listProperty.Add(new GUIProperty(nameof(replayCount), "ReplayCount"));
            group.listProperty.Add(new GUIProperty(nameof(defaultDuration), "Duration"));
        }
        public override void SetInspectorGUICommonTextArea(StringBuilder sB)
        {
            base.SetInspectorGUICommonTextArea(sB);
            //if (IsPropertyValid(DeltaTime) && IsPropertyValid(ReplayCount) && IsPropertyValid(Duration))
            {
                if (!IsConfigValid)
                {
                    sB.AppendWarningRichText("The config are not valid!");
                    sB.Append("\r\n");
                }
                else if (IsConfigOverflow(DeltaTime, ReplayCount, Duration))
                {
                    sB.AppendWarningRichText("The config are not valid (", DeltaTime, "s×", ReplayCount, ">", Duration, "s)!");
                    sB.Append("\r\n");
                }
            }
        }
#endif
        #endregion

    }
}
