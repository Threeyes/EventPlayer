using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
#if true // Threeyes_VideoPlayer
using UnityEngine.Video;
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Threeyes.EventPlayer
{
    /// <summary>
    /// ToUpdate: Add VideoState
    /// </summary>
    public class VideoEventPlayer :
#if true // Threeyes_VideoPlayer
   EventPlayerForComponentWithParamBase<VideoPlayer, VideoEventPlayer, VideoEventPlayer.VideoClipEvent, VideoClip>
#else
   EventPlayer
#endif
    {
        #region Property & Field

#if true // Threeyes_VideoPlayer

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
#endif

        public BoolEvent onPlayPause;//播放、暂停
        public UnityEvent onInit;//初始化数据(可用于外部设置视频源）
        public UnityEvent onPrepareCompleted;//适用于加载完成后显示图像
        public UnityEvent onFinish;//播放完成（旧版本是用onStop，但是容易导致死循环）

        #endregion


#if true // Threeyes_VideoPlayer
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
            if (Comp && Comp.frameCount > (ulong)frameIndex)
                Comp.time = frameIndex;

            yield return null;
            Pause();
        }

        public float Time
        {
            get { return Comp ? (float)Comp.time : 0; }
            set { if (Comp != null) Comp.time = value; }
        }
        public float Percent
        {
            get { return Comp ? (float)Comp.time : 0; }
            set { if (Comp != null) Comp.time = value; }
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
            UnityAction<VideoEventPlayer> actionCommon =
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

        bool isInitEvent = false;
        void TryInitData()
        {
            if (!Application.isPlaying)
                return;

            if (isInitEvent)//Set once for the life time
                return;

            onInit.Invoke();

            //Bug: loopPointReached delegate for non-looping video wasn't triggered
            if (Comp)
            {
                Comp.loopPointReached += LoopPointReached;
                Comp.prepareCompleted += PrepareCompleted;
                Comp.seekCompleted += SeekCompleted;
                Comp.frameReady += FrameReady;
            }
            isInitEvent = true;
        }

        protected virtual void FrameReady(VideoPlayer source, long frameIdx)
        {
        }
        protected virtual void LoopPointReached(VideoPlayer source)
        {
            onFinish.Invoke();
        }
        protected virtual void PrepareCompleted(VideoPlayer source)
        {
            onPrepareCompleted.Invoke();
        }
        protected virtual void SeekCompleted(VideoPlayer source)
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
            if (IsPlaying)
            {
#if UNITY_EDITOR
                RepaintHierarchyWindow();
#endif
            }
        }

        #endregion


#endif

        #region Define

#if true // Threeyes_VideoPlayer
        [System.Serializable]
        public class VideoClipEvent : UnityEvent<VideoClip>
        {

        }
#endif

        #endregion

        #region Editor Method
#if UNITY_EDITOR

        static string instName = "VideoEP ";
        [MenuItem(strExtendMenuGroup + "VideoEventPlayer", false, intExtendMenuOrder + 2)]
        public static void CreateVideoEventPlayer()
        {
            EditorTool.CreateGameObject<VideoEventPlayer>(instName);
        }

        [MenuItem(strExtendMenuGroup + "VideoEventPlayer Child", false, intExtendMenuOrder + 3)]
        public static void CreateVideoEventPlayerChild()
        {
            EditorTool.CreateGameObjectAsChild<VideoEventPlayer>(instName);
        }


        public override void SetHierarchyGUIType(StringBuilder sB)
        {
            sB.Append("Video");
        }
        public override void SetHierarchyGUIProperty(StringBuilder sB)
        {
            base.SetHierarchyGUIProperty(sB);

#if true // Threeyes_VideoPlayer
            if (!Comp)
                return;

            sbCache.Length = 0;
            if (Comp.isPlaying)
            {
                sbCache.Append((Comp.time).ToString("00:00")).Append("/").Append((Comp.clip.length).ToString("00:00"));
                sbCache.Append("(").Append((Comp.time * 100 / Comp.clip.length).ToString("#0.00")).Append("%").Append(")");
            }
            AddSplit(sB, sbCache);
#endif
        }

        public override void SetInspectorGUIUnityEventProperty(GUIPropertyGroup group)
        {
            base.SetInspectorGUIUnityEventProperty(group);
            group.listProperty.Add(new GUIProperty("onPlayPause", "OnPlayPause"));
            group.listProperty.Add(new GUIProperty("onInit", "OnInit"));
            group.listProperty.Add(new GUIProperty("onPrepareCompleted", "OnPrepareCompleted"));
            group.listProperty.Add(new GUIProperty("onFinish", "OnFinish"));
        }

        public override void SetInspectorGUICommonTextArea(StringBuilder sB)
        {
            base.SetInspectorGUICommonTextArea(sB);

#if true // Threeyes_VideoPlayer
#else
            EditorDrawerTool.AppendWarningText(sB, "You need to open the Setting Window and active VideoPlayer support!");
            sB.Append("\r\n");
#endif
        }
#endif
        #endregion

        #region Definition

        #endregion

    }
}
