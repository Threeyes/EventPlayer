using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using Threeyes.Core;
#if Threeyes_VideoPlayer
using UnityEngine.Video;
#endif
#if UNITY_EDITOR
using UnityEditor;
using Threeyes.Core.Editor;
#endif

namespace Threeyes.EventPlayer
{
    /// <summary>
    /// ToUpdate: Add VideoState
    /// </summary>
    public class EventPlayer_Video :
#if Threeyes_VideoPlayer
   EventPlayerForCompWithParamBase<VideoPlayer, EventPlayer_Video, EventPlayer_Video.VideoClipEvent, VideoClip>
#else
   EventPlayerForCompWithParamBaseDummy
#endif
    {
        #region Property & Field

#if Threeyes_VideoPlayer

        public bool IsPlaying { get { return Comp ? Comp.isPlaying : false; } }
        public override string ValueToString
        {
            get
            {
                if (Value)
                {
                    if (Value.originalPath != cacheVideoClipPath)
                    {
                        cacheVideoClipPath = Value.originalPath;
                        cacheValueToString = cacheVideoClipPath.GetFileNameWithoutExtension();
                        cacheValueToString = cacheValueToString.Substring(0, Mathf.Min(cacheValueToString.Length, 10)); //In case the name is tooooooo long
                    }
                    return cacheValueToString; //In case the name is tooooooo long
                }
                return "";
            }
        }
        string cacheValueToString;
        string cacheVideoClipPath;
        public override VideoClip Value
        {
            get
            {
                //Try return Value first, or else return clip from VideoPlayer (In this case, Value means override)
                return base.Value ? base.Value : (Comp && Comp.clip ? Comp.clip : null);
            }
            set
            {
                base.Value = value;
            }
        }

        public string VideoTimeInfo
        {
            get
            {
                if (Comp && Comp.clip)
                    return Comp.time.DateTimeFormat() + "/" + Comp.clip.length.DateTimeFormat();
                return ZeroDateTimeFormat + "/" + ZeroDateTimeFormat;
            }
        }
        string ZeroDateTimeFormat { get { if (string.IsNullOrEmpty(_zeroDateTimeFormat)) _zeroDateTimeFormat = ((double)0).DateTimeFormat(); return _zeroDateTimeFormat; } }
        string _zeroDateTimeFormat;
        /// <summary>
        /// Video progress
        /// </summary>
        public float VideoPercent
        {
            get
            {
                if (Comp && Comp.frameCount > 0 && Comp.frame > -1)//Frame为0代表该值无效
                {
#if UNITY_2018_2 //该版本有个问题：frame的值是[0,Comp.frameCount ]
                    return (float)Comp.frame / Comp.frameCount;
#else//新版本：frame的值是[-1,Comp.frameCount-1 ]
                    return Mathf.Abs(Comp.frame) / (Comp.frameCount - 1);
#endif
                }
                return 0;

            }
            set
            {
                if (!Comp)
                    return;
                isSeeking = true;
#if UNITY_2018_2 //该版本有个问题：frame的值是[0,Comp.frameCount ]
                Comp.frame = (long)(Comp.frameCount * value);
#else//新版本：frame的值是[-1,Comp.frameCount-1 ]
                Comp.frame = (long)((Comp.frameCount - 2) * value);//Bug：如果设置为Comp.frameCount - 1会报错
#endif
            }
        }

        /// <summary>
        /// Video Volume
        /// </summary>
        public float VolumePercent
        {
            get
            {
                switch (Comp.audioOutputMode)
                {
                    case VideoAudioOutputMode.Direct:
                        return Comp.GetDirectAudioVolume(0);
                    case VideoAudioOutputMode.AudioSource:
                        if (Comp.GetTargetAudioSource(0))
                            return Comp.GetTargetAudioSource(0).volume;
                        return 0;
                    default:
                        return 0;
                }
            }
            set
            {
                switch (Comp.audioOutputMode)
                {
                    case VideoAudioOutputMode.Direct:
                        Comp.SetDirectAudioVolume(0, value); break;
                    case VideoAudioOutputMode.AudioSource:
                        if (Comp.GetTargetAudioSource(0))
                            Comp.GetTargetAudioSource(0).volume = value; break;
                }
            }
        }

#endif

        public BoolEvent onPlayPause;// Play/Pause
        public UnityEvent onInit;//Init Video(can be use to set external video source）
        public UnityEvent onPrepareCompleted;//适用于加载完成后显示图像
        public FloatEvent onVideoPercentChanged;
        public StringEvent onVideoTimeInfoChanged;//  00:00/99:00
        public UnityEvent onFinish;//play finish

        #endregion

#if Threeyes_VideoPlayer
        #region Public Method

        public void TogglePlayPause()
        {
            if (Comp.isPlaying)
                Pause();
            else
                Play();
        }

        public void RePlay()
        {
            StopVideoFunc();//ForceStop
            Play();
        }

        public void PlayPause(bool isPlay)
        {
            if (isPlay)
                Play();
            else
                Pause();
        }

        public void Pause()
        {
            PauseFunc();
        }

        /// <summary>
        /// Set the videoClip and pause to the first frame
        /// </summary>
        public void SetFirstFrame()
        {
            SetFrame(0);
        }

        public void SetLastFrame()
        {
            SetFrame((long)(Comp.frameCount - 1));
        }

        /// <summary>
        /// ToUpdate:功能无效，尝试其他解决方法
        /// </summary>
        /// <param name="frameIndex"></param>
        public void SetFrame(long frameIndex)
        {
            if (!Application.isPlaying)
                return;

            if (Comp)
            {
                StartCoroutine(IESetFrame(frameIndex));
            }
        }
        IEnumerator IESetFrame(long frameIndex)
        {
            Play();//Force update
            yield return null;
            if (Comp && Comp.frameCount > (ulong)frameIndex)
                Comp.frame = frameIndex;
            yield return null;

            Pause();
        }

        #endregion

        #region Inner Method

        protected override void PlayWithParamFunc(VideoClip value)
        {
            if (value == null)
            {
                Debug.LogError("The value is null!");
                return;
            }

            TryInitData();

            if (value != Value)
            {
                StopVideoFunc();//Try Stop pre video()
            }
            if (Comp)
                Comp.clip = value;//Set Data
            onPlayPause.Invoke(true);
            PlayVideoFunc();

            base.PlayWithParamFunc(value);
        }

        protected override void PlayFunc()
        {
            TryInitData();
            onPlayPause.Invoke(true);
            PlayVideoFunc();

            base.PlayFunc();
        }

        void PauseFunc()
        {
            UnityAction<EventPlayer_Video> actionCommon =
            (ep) =>
            {
                if (ep != null)
                    ep.Pause();
            };

            InvokeFunc(
                () =>
                {
                    PauseVideoFunc();
                    onPlayPause.Invoke(false);
                },
                actionCommon,
                actionCommon);
        }

        protected override void StopFunc()
        {
            StopVideoFunc();
            base.StopFunc();
        }

        bool hasVideoPlayerInit = false;//Has the VideoPlayer Event init?
        bool isSeeking = false;//Is the video seeking?
        //bool isPrepareCompleted = false;
        void TryInitData()
        {
            if (!Application.isPlaying)
                return;

            if (hasVideoPlayerInit)
                return;

            onInit.Invoke();

            //Bug: loopPointReached delegate for non-looping video wasn't triggered
            if (Comp)
            {
                Comp.loopPointReached += LoopPointReached;
                Comp.prepareCompleted += PrepareCompleted;
                Comp.seekCompleted += SeekCompleted;
                Comp.frameReady += FrameReady;//Invoked when a new frame is ready.
            }
            hasVideoPlayerInit = true;
        }

        protected virtual void LoopPointReached(VideoPlayer source)
        {
            onFinish.Invoke();
        }
        protected virtual void PrepareCompleted(VideoPlayer source)
        {
            onPrepareCompleted.Invoke();
            //isPrepareCompleted = true;
        }

        protected virtual void SeekCompleted(VideoPlayer source)
        {
            isSeeking = false;
        }
        protected virtual void FrameReady(VideoPlayer source, long frameIdx)
        {
        }

        //——Video Control (Check if null incase this EP is for group purpose)
        void PlayVideoFunc()
        {
            if (!Application.isPlaying)
                return;
            if (Comp)
                Comp.Play();
        }
        void StopVideoFunc()
        {
            if (!Application.isPlaying)
                return;
            if (Comp)
                Comp.Stop();
        }
        void PauseVideoFunc()
        {
            if (!Application.isPlaying)
                return;
            if (Comp)
                Comp.Pause();
        }

        #endregion

        #region Unity Method

        void Update()
        {
            if (!isSeeking)
            {
                onVideoPercentChanged.Invoke(VideoPercent);
                onVideoTimeInfoChanged.Invoke(VideoTimeInfo);
            }


            if (IsPlaying)
            {
#if UNITY_EDITOR
                RepaintHierarchyWindow();//UpdateInfo
#endif
            }
        }

        #endregion

#endif

        #region Define

#if Threeyes_VideoPlayer
        [System.Serializable]
        public class VideoClipEvent : UnityEvent<VideoClip>
        {

        }
#endif

        #endregion

        #region Editor Method
#if UNITY_EDITOR

        //——MenuItem——
        static string instName = "VideoEP ";
        [MenuItem(strMenuItem_Root_Extend + "VideoEventPlayer", false, intExtendMenuOrder + 1)]
        public static void CreateVideoEventPlayer()
        {
            EditorTool.CreateGameObjectAsChild<EventPlayer_Video>(instName);
        }

        //——Hierarchy GUI——
        public override string ShortTypeName { get { return "Video"; } }
        public override void SetHierarchyGUIProperty(StringBuilder sB)
        {
            base.SetHierarchyGUIProperty(sB);

#if Threeyes_VideoPlayer
            if (!Comp)
                return;
            if (!Comp.clip)
                return;

            sbCache.Length = 0;
            {
                sbCache.Append(VideoTimeInfo);//Time Info
                sbCache.Append("(").Append((VideoPercent * 100).ToString("#0.00")).Append("%").Append(")");//Video Percent（有问题，暂不显示）
            }
            AddSplit(sB, sbCache);
#endif
        }

        //——Inspector GUI——
        public override void SetInspectorGUIUnityEventProperty(GUIPropertyGroup group)
        {
            base.SetInspectorGUIUnityEventProperty(group);
            group.listProperty.Add(new GUIProperty(nameof(onPlayPause)));
            group.listProperty.Add(new GUIProperty(nameof(onInit)));
            group.listProperty.Add(new GUIProperty(nameof(onPrepareCompleted)));
            group.listProperty.Add(new GUIProperty(nameof(onVideoPercentChanged)));
            group.listProperty.Add(new GUIProperty(nameof(onVideoTimeInfoChanged)));
            group.listProperty.Add(new GUIProperty(nameof(onFinish)));
        }
        public override void SetInspectorGUICommonTextArea(StringBuilder sB)
        {
            base.SetInspectorGUICommonTextArea(sB);

#if Threeyes_VideoPlayer
#else
            sB.AppendWarningRichText("You need to open the Setting Window and active VideoPlayer support!");
            sB.Append("\r\n");
#endif
        }
#endif
        #endregion

        #region Definition

        #endregion

    }
}
