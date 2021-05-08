using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace Threeyes.EventPlayer
{
    /// <summary>
    /// Work with Param
    /// </summary>
    /// <typeparam name="TEP">Type of EP</typeparam>
    /// <typeparam name="TUnityEvent">UnityEvent with Param</typeparam>
    /// <typeparam name="TParam"></typeparam>
    public class EventPlayerWithParamBase<TEP, TUnityEvent, TParam> : EventPlayer, IEventPlayerWithParam
        where TEP : EventPlayerWithParamBase<TEP, TUnityEvent, TParam>
        where TUnityEvent : UnityEvent<TParam>
    {
        #region Property & Field

        //——UnityEvent——
        public TUnityEvent onPlayWithParam = default(TUnityEvent);
        public bool IsPlayWithParam { get { return isPlayWithParam; } set { isPlayWithParam = value; } }
        public virtual TParam Value { get { return value; } set { this.value = value; } }
        public virtual string ValueToString { get { return Value.ToString(); } }//The value info Display in Inspector
        public bool IsDetectMatch { get { return isDetectMatch; } set { isDetectMatch = value; } }
        public TParam TargetValue { get { return targetValue; } set { targetValue = value; } }

        //[Header("Param Setting")]
        [SerializeField]
        [Tooltip("Use current Value when you call Play. Set to true if this EP is the Invoker")]
        protected bool isPlayWithParam = false;
        [SerializeField]
        [Tooltip("Current value")]
        protected TParam value = default(TParam);
        [SerializeField]
        [Tooltip("Check if the receive value matchs the targetValue before Play")]
        protected bool isDetectMatch = false;
        [SerializeField]
        [Tooltip("Target value to Play")]
        protected TParam targetValue = default(TParam);

        #endregion

        #region Public Method

        public override void Play(bool isPlay)
        {
            PlayWithParam(isPlay, IsPlayWithParam, Value);
        }
        public void PlayWithParam(bool isPlay, bool isPlayWithParam)
        {
            PlayWithParam(isPlay, true);
        }
        /// <summary>
        /// Execute the related play event with param (Parallel to Play(bool))
        /// PS:该公开方法不能声明在基类，否则会与(Bool)EventPlayer冲突
        /// </summary>
        /// <param name="value"></param>
        public virtual void Play(TParam value)
        {
            PlayWithParam(true, true, value);
        }

        public void PlayWithParam(bool isPlay, bool isPlayWithParam, TParam value = default(TParam))
        {
            //PS:All the public play-relate method will call this method
            if (!IsActive)
                return;

            if (isPlay && !IsReverse || !isPlay && IsReverse)//Actual Play
            {
                if (IsPlayOnce && isPlayed || !CanPlay)
                    return;

                if (isPlayWithParam)
                {
                    if (!(IsDetectMatch && !CompareValue(value, TargetValue)))
                        PlayWithParamFunc(value);
                }
                else
                    PlayFunc();
            }
            else if (!isPlay && !IsReverse || isPlay && IsReverse)//Actual Stop
            {
                if (!CanStop)
                    return;

                StopFunc();
            }
        }

        /// <summary>
        /// Manual detect the value and play (ignore the isDetectMatch value)
        /// </summary>
        /// <param name="value"></param>
        public void DetectPlay(TParam value)
        {
            bool cacheState = IsDetectMatch;
            IsDetectMatch = true;
            Play(value);
            IsDetectMatch = cacheState;
        }
        #endregion

        #region Inner Method

        protected virtual bool CompareValue(TParam value1, TParam value2)
        {
            //Todo:新增一个值类型的子类，根据枚举值CompareType，可以判断 大于、大于等于、小于、不等于 等事件
            return value1.Equals(value2);
        }

        protected virtual void PlayWithParamFunc(TParam value)
        {
            UnityAction<TEP> actionRelate =
                (ep) =>
                {
                    if (ep != null)
                        ep.PlayWithParam(true, true, value);
                };

            InvokeFunc(
                () =>
                {
                    Value = value;
                    onPlayWithParam.Invoke(value);
                    SetStateFunc(true, EventPlayer_State.PlayedwithParam);
                },
                actionRelate,
                actionRelate,
                () =>
                {
                    if (IsLogOnPlay) Debug.Log(name + " Play with Param: " + ValueToString);
                });
        }


        #endregion

        #region Editor Method
#if UNITY_EDITOR

        public override void SetHierarchyGUIProperty(StringBuilder sB)
        {
            base.SetHierarchyGUIProperty(sB);
            sbCache.Length = 0;

            if (IsPlayWithParam)
                sbCache.Append("◆");
            sbCache.Append(ValueToString);

            if (IsDetectMatch)
                sbCache.Append("==").Append(TargetValue);

            AddSplit(sB, sbCache);
        }

        public override void SetInspectorGUIUnityEventProperty(GUIPropertyGroup group)
        {
            base.SetInspectorGUIUnityEventProperty(group);
            group.listProperty.Add(new GUIProperty("onPlayWithParam", "OnPlayWithParam"));
        }

        public override void SetInspectorGUISubProperty(GUIPropertyGroup group)
        {
            base.SetInspectorGUISubProperty(group);
            group.title = "Param Setting";
            group.listProperty.Add(new GUIProperty("isPlayWithParam", "PlayWithParam", "Invoke Play with current Value."));
            group.listProperty.Add(new GUIProperty("value", "Value"));
            group.listProperty.Add(new GUIProperty("isDetectMatch", "DetectMatch", "Play only when the receive value matchs the targetValue"));
            group.listProperty.Add(new GUIProperty("targetValue", "TargetValue", "Target value to Play", IsDetectMatch));
        }
#endif
        #endregion
    }

}