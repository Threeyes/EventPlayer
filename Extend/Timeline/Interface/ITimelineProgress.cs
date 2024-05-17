using System;
using UnityEngine;
using UnityEngine.Events;
using Threeyes.Core;
#if Threeyes_Timeline
using UnityEngine.Playables;
#endif
namespace Threeyes.EventPlayer
{

    /// <summary>
    /// 处理Timeline中Clip的事件
    /// </summary>
    public interface ITimelineProgress
    {
        void OnClipUpdate(PlayableInfo playableInfo);//Execute on every state

        //Editor
        void OnClipReset(PlayableInfo playableInfo);//Execute on desire state
        void OnClipComplete(PlayableInfo playableInfo);//Execute on desire state
    }

    /// <summary>
    /// Execute Order:
    /// OnPlayableCreate
    /// OnGraphStart
    /// OnBehaviourPlay
    /// ProcessFrame
    /// OnBehaviourPause
    /// OnGraphStop
    /// OnPlayableDestroy
    /// </summary>
    [Flags]
    public enum ClipState : int
    {
        None = 0,
        PlayableCreate = 1 << 0,//Play inside the Clip, before ProcessFrame
        GraphStart = 1 << 1,
        BehaviourPlay = 1 << 2,
        ProcessFrame = 1 << 3,
        BehaviourPause = 1 << 4,
        GraphStop = 1 << 5,
        PlayableDestroy = 1 << 6
    }

    public enum ClipLoopType
    {
        Restart = 0,
        PingPong = 1
    }

    [System.Serializable]
    public class PlayableInfo : DataObjectBase
    {
        //#Clip Setting 
        [Header("Config")]
        public GameObject binding;
        public ClipLoopType loopType = ClipLoopType.Restart;
        public double clipLength = -1;
        public bool isFlip;
        public AnimationCurve curvePercent = AnimationCurve.Linear(0, 0, 1, 1);
        public int subClipCount = 1;// { get { return clipLength > 0 ? Mathd.CeilToInt(duration / clipLength) : 1; } }//默认为1

        //#Editor Setting
        public bool isResetOnJump = false;
        public bool isCompleteOnJump = false;
        public bool isResetOnPlay = false;
        public bool isEditorPreparePlay = false;//Editor is about to play

        //#RunTime
        [Header("Run Time")]
        public ClipState clipState = ClipState.BehaviourPlay;
#if Threeyes_Timeline
        public Playable playable;
        public FrameData? info;//FrameData only valid on Play/Process/Pause

        //Mix Data
        public int indexInTrack;
        public float inputWeight;

#endif

        public double time;//Current Time
        public double duration;//Total Time
        public float deltaTime;
        public float percent { get { return CalculatePercent(time, duration); } }//Total percent 

        public int subClipIndex = -1;//PS: When Clip Length>0, it will split as multi sub-Clip
        public double subTime { get { return clipLength > 0 ? Mathd.Repeat(time, clipLength) : time; } }
        public double subDuration { get { return clipLength > 0 ? clipLength : duration; } }
        public float subPercent { get { return clipLength > 0 ? CalculatePercent(subTime, subDuration) : percent; } }

        /// <summary>
        /// 手动计算并克隆一个PlayableInfo（不应该影响原有值）），常用于重置
        /// </summary>
        /// <param name="time">desire time</param>
        /// <returns></returns>
        public PlayableInfo ClonePlayableInfo(double time)
        {
            PlayableInfo playableInfo = ReflectionTool.DeepCopy(this);
            playableInfo.time = time;
            return playableInfo;
        }
#if Threeyes_Timeline
        public void UpdateData(Playable playable, FrameData? info = null)
        {
            this.playable = playable;

            //PS:该数据可能会被销毁，因此需要及时保存其值
            time = playable.GetTime();
            duration = playable.GetDuration();

            this.info = info;
            if (info.HasValue)
                deltaTime = info.Value.deltaTime;
        }

#endif

        #region Utility

        float cachePercentResult;
        float CalculatePercent(double time, double duration)
        {
            if (duration > 0)
            {
                if (isFlip)//Reverse the process
                    time = duration - time;

                float curPercent = (float)(time / duration);
                curPercent = curvePercent.Evaluate(curPercent);//Remap

                cachePercentResult = Mathf.Clamp01(curPercent);//ToUpdate：后期增加一个UnClamp的选项
                if (loopType == ClipLoopType.PingPong && subClipIndex != -1 && IsOdd(subClipIndex))
                {
                    cachePercentResult = 1 - cachePercentResult;   //Reverse the process
                }
            }
            else
            {
                cachePercentResult = 0;
            }
            return cachePercentResult;
        }

        /// <summary>
        /// 奇数
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static bool IsOdd(int n)
        {
            return (n % 2 == 1) ? true : false;
        }

        #endregion

    }


    [System.Serializable]
    public class PlayableInfoEvent : UnityEvent<PlayableInfo>
    {
    }
}
