#if Threeyes_Timeline
using System;
using System.Collections.Generic;
using Threeyes.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Threeyes.EventPlayer
{
    /// <summary>
    /// Bug:Unity2018 won't auto refresh if you change the config
    /// </summary>
    public class EventPlayerBehaviour : BehaviourBase<GameObject>
    {
        public static bool isDebugLog =
        //true;
        false;

        public PlayableInfo playableInfo = new PlayableInfo();

        //Clip Setting 
        public EventPlayer eventPlayer;
        public ClipLoopType loopType = ClipLoopType.Restart;
        public double clipLength = -1;
        public bool isFlip = false;//Reverse the process
        public AnimationCurve curvePercent = AnimationCurve.Linear(0, 0, 1, 1);//percent remapping

        public ClipState resetOn = ClipState.None;//When to call Reset

        //#Editor Setting
        public bool isResetOnJump = false;
        public bool isCompleteOnJump = false;
        public bool isResetOnPlay = false;

        //#Run Time
        List<int> listLoopFramePlayed = new List<int>();//Cache the loop index
        int loopIndex = -1;//The index of the current loop

        /// <summary>
        /// Get Call when it start play insde Clip (even begin in middle)
        /// PS: ProcessFrame execute right after OnBehaviourPlay
        /// </summary>
        /// <param name="playable"></param>
        /// <param name="info"></param>
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            OnBehaviourPlayFunc(playable, info);
        }

        void OnBehaviourPlayFunc(Playable playable, FrameData info)
        {
            if (isDebugLog)
                Debug.LogError("OnBehaviourPlay");

            if (eventPlayer)
                eventPlayer.Play();

            //Reset inner data
            listLoopFramePlayed.Clear();
            loopIndex = -1;
            playableInfo.subClipIndex = loopIndex;

            InitConfig(playable, info);
            UpdateTimelineInfo(ClipState.BehaviourPlay, playable, info);
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (isDebugLog)
                Debug.LogError("ProcessFrame");

            if (!eventPlayer)
                return;

#if UNITY_EDITOR

#if !UNITY_2019_2_OR_NEWER
            //PS:Unity2019及以下，当检测到更改Clip或EventPlayerClip，不会自动重新调用OnBehaviourPlay，只会调用ProcessFrame
            InitConfig(playable, info);
#endif

            //PS:In Editor mode, the OnBehaviourPlay will not get call on first frame, so we have to manual call it on the first frame
            //(https://forum.unity.com/threads/release-event-player-visual-play-and-organize-unityevent.536984/#post-3605916)
            //(https://forum.unity.com/threads/code-entering-exiting-a-playable-behaviour-on-the-graph.690454/#post-4618309  seant_unity:If you need guaranteed triggers, signals/markers are the correct solution. Clips are intended to do deterministic evaluation.)
            if (!Application.isPlaying)
            {
                double time = playable.GetGraph().GetRootPlayable(0).GetTime();
                if (time == 0)
                {
                    OnBehaviourPlayFunc(playable, info);
                    if (isDebugLog)
                        Debug.LogError("ProcessFrame_OnBehaviourPlay on first frame");
                }
            }
#endif

            //If reach next loop start and is playing(in case user darg the process backward and trigger this event), inovke Play method again
            if (clipLength > 0 && info.evaluationType == FrameData.EvaluationType.Playback)
            {
                //Runtime
                loopIndex = Mathf.CeilToInt((float)(playable.GetTime() / clipLength)) - 1;//[0, N]
                if (loopIndex > 0 && !listLoopFramePlayed.Contains(loopIndex))
                {
                    eventPlayer.Play();
                    listLoopFramePlayed.Add(loopIndex);
                }
                playableInfo.subClipIndex = loopIndex;
            }

            UpdateTimelineInfo(ClipState.ProcessFrame, playable, info);
        }

        /// <summary>
        /// Get call when:
        /// 1. PlayableDirector.Pause is call inside this clip
        /// 2. Reach the end frame of this clip
        /// PS:
        /// 1.ProcessFrame don't get invoke in this frame
        /// 2.If you don't wan't to invoke it on Pause, just set the speed to 0 to pause the timeline
        /// </summary>
        /// <param name="playable"></param>
        /// <param name="info"></param>
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            //Prevent get call on the first frame https://forum.unity.com/threads/release-event-player-visual-play-and-organize-unityevent.536984/#post-3605916
            double rootTime = playable.GetRootTime();
            if (rootTime > 0)
            {
                UpdateTimelineInfo(ClipState.BehaviourPause, playable, info);
                if (isDebugLog)
                    Debug.LogError("OnBehaviourPause");

                /// PS: If PlayableDirector is set to loop/hold & it's the last frame of clip & Throught out the Trach, it won't get call (because it's always active

                //Stop after UpdateTimeLineInfo
                if (eventPlayer)
                    eventPlayer.Stop();
            }

            //Reset inner data
            listLoopFramePlayed.Clear();
        }

        public override void OnPlayableCreate(Playable playable)
        {
            if (isDebugLog)
                Debug.LogError("OnPlayableCreate");

            base.OnPlayableCreate(playable);
            UpdateTimelineInfo(ClipState.PlayableCreate, playable);
        }
        public override void OnGraphStart(Playable playable)
        {
            if (isDebugLog)
                Debug.LogError("OnGraphStart");

            base.OnGraphStart(playable);
            UpdateTimelineInfo(ClipState.GraphStart, playable);


#if UNITY_EDITOR //Editor Preview
            if (!Application.isPlaying)
            {
                if (isResetOnPlay)
                {
                    StartPreview(playableInfo);//开始预览模式，在Editor.Play时会自动重置
                }
                else
                {
                    StopPreview(playableInfo);//退出预览模式
                }
            }
#endif
        }

        /// called when the PlayableGraph that owns this PlayableBehaviour stops.
        /// OnGraphStop is called when Unity stops playing the owning graph, and is guaranteed to always and only be called if OnGraphStart has been called.
        /// If the graph has been only been manually evaluated, OnGraphStop will be called prior to OnPlayableDestroy.
        /// 
        /// Warning: 
        /// 1.Event the time has not come accross this Clip, it will still get invoked
        /// 2.Both OnBehaviourPause and OnGraphStop functions are called when either paused or stopped
        /// </summary>
        /// <param name="playable"></param>
        public override void OnGraphStop(Playable playable)
        {
            if (isDebugLog)
                Debug.LogError("OnGraphStop");

            base.OnGraphStop(playable);
            UpdateTimelineInfo(ClipState.GraphStop, playable);
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            if (isDebugLog)
                Debug.LogError("OnPlayableDestroy");

            base.OnPlayableDestroy(playable);
            UpdateTimelineInfo(ClipState.PlayableDestroy, playable);
        }


        void DebugLog(string content)
        {
            if (isDebugLog)
                Debug.Log(trackBinding.name + " " + content
               + " " + playableInfo.inputWeight
                );
        }

        #region Inner Method

        ITimelineProgress iEventPlayerTimeLineInfo;

        void InitConfig(Playable playable, FrameData info)
        {
            //Init the param that won't change on RunTime
            playableInfo.binding = trackBinding;
            playableInfo.loopType = loopType;
            playableInfo.isFlip = isFlip;
            playableInfo.clipLength = clipLength;
            playableInfo.curvePercent = curvePercent;
            playableInfo.subClipCount = clipLength > 0 ? Mathd.CeilToInt(playable.GetDuration() / clipLength) : 1;//默认为1
            playableInfo.isResetOnJump = isResetOnJump;
            playableInfo.isCompleteOnJump = isCompleteOnJump;
            playableInfo.isResetOnPlay = isResetOnPlay;
        }

        void UpdateTimelineInfo(ClipState clipState, Playable playable, FrameData? info = null)
        {
            if (UpdateData(clipState, playable, info) == null)
                return;

            iEventPlayerTimeLineInfo.OnClipUpdate(playableInfo);

            if ((resetOn & clipState) == clipState)
            {
                ResetTimelineInfo();
            }
        }

        ITimelineProgress UpdateData(ClipState clipState, Playable playable, FrameData? info = null)
        {
            iEventPlayerTimeLineInfo = eventPlayer as ITimelineProgress;
            if (iEventPlayerTimeLineInfo != null)//Incase the target is not inherit from target Interface
            {
                playableInfo.clipState = clipState;
                playableInfo.UpdateData(playable, info);
            }
            return iEventPlayerTimeLineInfo;
        }

        #endregion

        #region Invoke By EventPlayerMixerBehaviour

        /// <summary>
        /// 更新Clip在Track中的信息
        /// </summary>
        /// <param name="index"></param>
        /// <param name="weight"></param>
        public void UpdateMixData(int index, float weight)
        {
            playableInfo.indexInTrack = index;
            playableInfo.inputWeight = weight;
        }

        public void ResetTimelineInfo()
        {
            if (isResetOnJump)
            {
                //Send calcuated playableInfo, you can use it's cache config to reset data
                playableInfo.time = 0;
                playableInfo.isEditorPreparePlay = false;
                iEventPlayerTimeLineInfo.OnClipReset(playableInfo);
            }
        }

        void ResetTimelineInfoBeforePlay()
        {
            //Send calcuated playableInfo, you can use it's cache config to reset data
            playableInfo.time = 0;
            playableInfo.isEditorPreparePlay = true;
            iEventPlayerTimeLineInfo.OnClipReset(playableInfo);
        }


        public void CompleteTimelineInfo()
        {
            if (isCompleteOnJump)
            {
                playableInfo.time = playableInfo.duration;
                iEventPlayerTimeLineInfo.OnClipComplete(playableInfo);
            }
        }

        #endregion

        #region Editor Method
#if UNITY_EDITOR //Editor Preview

        UnityAction cacheActionReset;
        void StartPreview(PlayableInfo value)
        {
            iEventPlayerTimeLineInfo = eventPlayer as ITimelineProgress;
            if (iEventPlayerTimeLineInfo != null)//Incase the target is not inherit from target Interface
            {
                cacheActionReset = ResetTimelineInfoBeforePlay;
                TimelinePreviewManager.StartPreview(cacheActionReset);
            }
        }

        void StopPreview(PlayableInfo playableInfo)
        {
            TimelinePreviewManager.StopPreview(cacheActionReset);
        }

#endif
        #endregion
    }

}
#endif
