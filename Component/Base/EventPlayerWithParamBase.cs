using System.Collections;
using System.Collections.Generic;
using System.Text;
using Threeyes.Core;
using UnityEngine;
using UnityEngine.Events;

namespace Threeyes.EventPlayer
{
    /// <summary>
    /// Invoke event with Param
    /// 
    /// Warning: 
    /// -Don't call Play(TParam value) because it will conflict with Play(bool isPlay), use PlayWithParam(TParam value) instead.
    /// </summary>
    /// <typeparam name="TEP">Type of EP</typeparam>
    /// <typeparam name="TUnityEvent">UnityEvent with Param</typeparam>
    /// <typeparam name="TParam"></typeparam>
    public class EventPlayerWithParamBase<TEP, TUnityEvent, TParam> : EventPlayer, IEventPlayerWithParam
        where TEP : EventPlayerWithParamBase<TEP, TUnityEvent, TParam>
        where TUnityEvent : UnityEvent<TParam>
    {
        #region Networking
        public UnityAction<bool> actCommandPlayWithParam;

        #endregion

        #region Property & Field

        //——UnityEvent——
        public TUnityEvent onPlayWithParam = default(TUnityEvent);
        public TUnityEvent onStopWithParam = default(TUnityEvent);

        //——Basic Setting——
        public bool IsPlayWithParam { get { return isPlayWithParam; } set { isPlayWithParam = value; } }
        public bool IsStopWithParam { get { return isStopWithParam; } set { isStopWithParam = value; } }

        public virtual TParam Value { get { return value; } set { this.value = value; } }
        public virtual string ValueToString { get { return Value != null ? Value.ToString() : ""; } }//The value info Display in Inspector
        public bool IsDetectMatch { get { return isDetectMatch; } set { isDetectMatch = value; } }
        public TParam TargetValue { get { return targetValue; } set { targetValue = value; } }

        //[Header("Param Setting")]
        [SerializeField]
        [Tooltip("Use current Value when you call Play. Set to true if this EP is the Invoker")]
        protected bool isPlayWithParam = false;
        [SerializeField]
        [Tooltip("Use current Value when you call Stop. Set to true if this EP is the Invoker")]
        protected bool isStopWithParam = false;
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
        /// <summary>
        /// Execute the related play event with param (Parallel to Play(bool))
        /// Warning:该公开方法不能声明在基类，否则会与EventPlaye_Bool冲突
        /// </summary>
        /// <param name="value"></param>
        [System.Obsolete("Will messup with subClass Play(object). Use PlayWithParam(TParam) instead.", false)]
        public virtual void Play(TParam value)
        {
            PlayWithParam(true, true, value);
        }

        public override void Play(bool isPlay)
        {
            PlayWithParam(isPlay, isPlay ? IsPlayWithParam : IsStopWithParam, Value);
        }

        public void PlayWithParam(TParam value)
        {
            PlayWithParam(true, true, value);
        }
        public void StopWithParam(TParam value)
        {
            PlayWithParam(false, true, value);
        }
        /// <summary>
        /// Play using the exist value
        /// </summary>
        /// <param name="isPlay"></param>
        /// <param name="isPlayStopWithParam"></param>
        public void PlayWithParam(bool isPlay, TParam value)
        {
            PlayWithParam(isPlay, true, value);
        }

        public void PlayWithParam(bool isPlay, bool isPlayStopWithParam, TParam value)
        {
            //PS:All the public play-relate method will call this method
            if (!IsActive)
                return;

            if (isPlay && !IsReverse || !isPlay && IsReverse)//Actual Play
            {
                if (IsPlayOnce && isPlayed || !CanPlay)
                    return;

                if (isPlayStopWithParam)
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

                if (isPlayStopWithParam)
                {
                    if (!(IsDetectMatch && !CompareValue(value, TargetValue)))
                        StopWithParamFunc(value);
                }
                else
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
            PlayWithParam(value);
            IsDetectMatch = cacheState;
        }
        #endregion

        #region Inner Method

        protected virtual bool CompareValue(TParam value1, TParam value2)
        {
            //Todo:新增一个值类型的子类，根据枚举值CompareType，可以判断 大于、大于等于、小于、不等于 等事件
            return Equals(value1, value2);
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
                    SetStateFunc(EventPlayer_State.PlayedwithParam);
                },
                actionRelate,
                actionRelate,
                () =>
                {
                    if (IsLogOnPlay) Debug.Log(name + " Play with Param: " + ValueToString);
                });

            NotifyListener<EventPlayerWithParamListener<TParam>>((epl) => epl.OnPlayWithParam(value));
        }
        protected virtual void StopWithParamFunc(TParam value)
        {
            UnityAction<TEP> actionRelate =
                (ep) =>
                {
                    if (ep != null)
                        ep.PlayWithParam(false, true, value);
                };

            InvokeFunc(
                () =>
                {
                    Value = value;
                    onStopWithParam.Invoke(value);
                    SetStateFunc(EventPlayer_State.StopedwithParam);
                },
                actionRelate,
                actionRelate,
                () =>
                {
                    if (IsLogOnStop) Debug.Log(name + " Stop with Param: " + ValueToString);
                });

            NotifyListener<EventPlayerWithParamListener<TParam>>((epl) => epl.OnStopWithParam(value));
        }



        #endregion

        #region Editor Method
#if UNITY_EDITOR

        public override void SetHierarchyGUIProperty(StringBuilder sB)
        {
            base.SetHierarchyGUIProperty(sB);
            sbCache.Length = 0;

            if (IsPlayWithParam || IsStopWithParam)
                sbCache.Append("◆");
            sbCache.Append(ValueToString);

            if (IsDetectMatch)
                sbCache.Append("==").Append(TargetValue);

            AddSplit(sB, sbCache);
        }

        public override void SetInspectorGUIUnityEventProperty(GUIPropertyGroup group)
        {
            base.SetInspectorGUIUnityEventProperty(group);
            group.listProperty.Add(new GUIProperty(nameof(onPlayWithParam)));
            group.listProperty.Add(new GUIProperty(nameof(onStopWithParam)));
        }

        public override void SetInspectorGUISubProperty(GUIPropertyGroup group)
        {
            base.SetInspectorGUISubProperty(group);
            group.title = "Param Setting";
            group.listProperty.Add(new GUIProperty(nameof(isPlayWithParam), "PlayWithParam", "Invoke Play with current Value."));
            group.listProperty.Add(new GUIProperty(nameof(value)));
            group.listProperty.Add(new GUIProperty(nameof(isDetectMatch), "DetectMatch", "Play only when the receive value matchs the targetValue"));
            group.listProperty.Add(new GUIProperty(nameof(targetValue), "TargetValue", "Target value to Play", IsDetectMatch));
        }
#endif
        #endregion
    }
}