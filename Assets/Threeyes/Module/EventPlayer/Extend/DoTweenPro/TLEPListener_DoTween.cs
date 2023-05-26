using System.Collections.Generic;
using UnityEngine;
using Threeyes.Base;

#if Threeyes_DoTweenPro
using DG.Tweening;
using DG.Tweening.Core;
#endif

namespace Threeyes.EventPlayer
{
    /// <summary>
    /// PS: 版本1：
    /// ——增加编辑器模式下预览的选项，标注为测试版
    /// ——暂不支持DoTweenPath
    /// ——isFlip有bug
    /// </summary>
    public class TLEPListener_DoTween :
#if Threeyes_DoTweenPro
        TLEPListenerForCompBase<ABSAnimationComponent>
#else
        TLEPListenerForCompBaseDummy
#endif
    {

#if Threeyes_DoTweenPro

        public static bool isDebugLog =
true;
        //false;

        public override void OnPlayWithParam(PlayableInfo value)
        {
            if (!Comp || !value)
                return;

#if UNITY_EDITOR //Editor Preview
            if (value.clipState == ClipState.PlayableDestroy)//切换场景
            {
                if (!Application.isPlaying)
                {
                    StopAllPreviews();
                }
            }
#endif

            if (value.clipState == ClipState.BehaviourPlay)
            {
                OnPlayBegin(value);
                SetUpConfig(value);

#if UNITY_EDITOR //Editor Preview
                if (!Application.isPlaying)
                {

                    ////PS:DOTweenPreviewManagerEx会在Editor Play时重置所有Tween
                    StartPreview(value);
                }
                else
#endif
                {
                    //Todo：运行时不销毁创建的Tween，除非属性变化
                    //PS: 在哪一帧初始化Tween不重要，因为会通过SetDuration跳到对应位置

                    if (doTweenAnimation)
                    {
                        //PS:不要删除已有的Tween，因为旧的Tween可能与其他Behaviour有绑定
                        if (CacheTween != null)
                        {
                            //ResetAndKillTween();//PS:如果isFlip已经勾选，那么起始值与终值就会一致，导致动画无法正常云栖谷
                        }
                        else
                        {
                            doTweenAnimation.tween = null;
                            doTweenAnimation.CreateTween();
                            CacheTween = doTweenAnimation.tween;
                            if (CacheTween != null)
                            {
                                CacheTween.SetAutoKill(false);
                                CacheTween.SetUpdate(UpdateType.Manual);
                            }
                        }
                    }
                }
            }

            switch (value.clipState)
            {
                case ClipState.BehaviourPlay:
                case ClipState.ProcessFrame:
                case ClipState.BehaviourPause:
                    Evaluate(value);
                    break;
            }
        }

        public override void OnReset(PlayableInfo value)
        {
#if UNITY_EDITOR
            if (CacheTween == null)//优先利用BehaviourPlay创建的Tween，如果没有再自行创建
            {
                if (!value.isEditorPreparePlay)//如果准备Play，那就不新建Preview
                    StartPreview(value);
            }
#endif

            if (CacheTween != null)
            {
                Evaluate(value);

                if (value.isEditorPreparePlay)
                {
                    if (isDebugLog)
                        Debug.Log("OnReset for " + Comp.GetTarget().name + "  " + doTweenAnimation.animationType);
                    if (value.isResetOnPlay)
                        CacheTween.Rewind();
                    CacheTween.Kill();
                    CacheTween = null;
                }
            }
        }

        public override void OnComplete(PlayableInfo value)
        {
#if UNITY_EDITOR
            if (CacheTween == null)
                StartPreview(value);
#endif

            if (CacheTween != null)
            {
                Evaluate(value);
                if (isDebugLog)
                    Debug.Log("OnComplete for " + Comp.GetTarget().name + "  " + doTweenAnimation.animationType);
            }
        }

        #region Inner Method

        Tween CacheTween
        {
            get
            {
                return CacheBehaviourInfo ? CacheBehaviourInfo.tween : null;
            }
            set
            {
                if (CacheBehaviourInfo)
                    CacheBehaviourInfo.tween = value;
            }
        }

        GameObject CacheTarget
        {
            get
            {
                return CacheBehaviourInfo ? CacheBehaviourInfo.target : null;
            }
        }

        BehaviourInfo CacheBehaviourInfo
        {
            get
            {
                return HasKey(playableInfoInst) ? dicEditorPreviewBehaviourInfo[playableInfoInst] : null;
            }
            set
            {
                dicEditorPreviewBehaviourInfo[playableInfoInst] = value;
            }
        }
        public PlayableInfo playableInfoInst;//缓存当前作用的PlayableInfo


        public Dictionary<PlayableInfo, BehaviourInfo> dicEditorPreviewBehaviourInfo = new Dictionary<PlayableInfo, BehaviourInfo>();//编辑器预览时使用的Tween
        bool HasKey(PlayableInfo playableInfo)
        {
            return playableInfo.NotNull() && dicEditorPreviewBehaviourInfo.ContainsKey(playableInfo);
        }


        private void Awake()
        {
            //Reset
            //Todo：移动到Init的第一次初始化位置
            dicEditorPreviewBehaviourInfo.Clear();
        }

        /// <summary>
        /// （PS: 因为SetUpConfig可能会被调用多次，因此另外新建此方法
        /// </summary>
        /// <param name="value"></param>
        private void OnPlayBegin(PlayableInfo value)
        {
            if (CacheTween != null)
            {
                //检测Target是否更换
                isTargetChanged = GetTarget(value) != CacheBehaviourInfo.target;
            }
        }


        //PS:以下引用与EventPlayerBehaviour无关，只与当前运行Tween有关，不需要存在字典中
        DOTweenAnimation doTweenAnimation;//Real
        DOTweenPath doTweenPath;
        bool hasLoop;
        bool isTargetChanged = false;
        void SetUpConfig(PlayableInfo value)
        {
            playableInfoInst = value;//缓存每个EventPlayerBehaviour对应的PlayableInfo（ToUpdate：移动到父物体）
            if (CacheBehaviourInfo == null)
                CacheBehaviourInfo = new BehaviourInfo();

            //Try convert to real type
            doTweenAnimation = Comp as DOTweenAnimation;
            doTweenPath = Comp as DOTweenPath;

            GameObject target = GetTarget(value);
            Comp.SetAutoPlay(false);
            Comp.SetAutoKill(false);
            Comp.SetTarget(target, value.binding ? false : true, isTargetChanged);

            hasLoop = value.subClipCount > 1;
            Comp.SetLoops(value.subClipCount);
            //ToUpdate: 增加针对DoTweenAnimation的LoopType的设置方式
            Comp.SetDuration((float)value.subDuration);//Directly set is faster than compare(PS: 即使不设置Duration也不影响效果，因为Goto传入的是百分比)

            CacheBehaviourInfo.indexInTrack = value.indexInTrack;
            CacheBehaviourInfo.target = target;
            CacheBehaviourInfo.doTweenAnimation = doTweenAnimation;
        }

        GameObject GetTarget(PlayableInfo value)
        {
            return value.binding ? value.binding : Comp.gameObject;
        }

        void Evaluate(PlayableInfo value)
        {
            if (CacheTween != null)
            {
                float toTime = (float)(value.subPercent * value.subDuration);
                CacheTween.Goto(toTime);
            }
        }

        [ContextMenu("ResetAndKillTween")]
        public void ResetAndKillTween()
        {
            //参考：DOTweenPreviewManagerEx.StartupGlobalPreview
            if (CacheTween != null)
            {
                CacheTween.Rewind();
                CacheTween.Kill();
            }
        }

        #endregion

        #region Defines


        /// <summary>
        /// 每个Behaviour对应的信息
        /// </summary>
        [System.Serializable]
        public class BehaviourInfo : DataObjectBase
        {
            public int indexInTrack = -1;
            public GameObject target;
            public Tween tween;
            public DOTweenAnimation doTweenAnimation;

            public BehaviourInfo()
            {
            }
            public BehaviourInfo(int indexInTrack, GameObject target, Tween tween = null)
            {
                this.indexInTrack = indexInTrack;
                this.target = target;

                if (tween != null)
                    this.tween = tween;
            }
        }

        #endregion

        #region Editor Method
#if UNITY_EDITOR

        void StartPreview(PlayableInfo value)
        {
            if (!SOEventPlayerSettingManager.Instance.activeDoTweenProPreview)
                return;

            //Todo:针对Index缓存Tween，避免重用TLEP时跳转会覆盖同一个Tween的问题（向传入StartPreviewAction,让其在Play时执行清空List）（或者将Tween回传给Behaviour）
            //Todo:要等前一个Tween开启Preview后，才开启下一个Preview，这样才能基于上一个Clip的结尾状态开启Tween
            //基于当前的参数生成Tween
            if (doTweenAnimation)
            {
                //在更换Target后重置所有预览
                if (CacheTween == null || isTargetChanged)
                    CacheTween = DOTweenPreviewManagerEx.StartPreviewForTarget(CacheBehaviourInfo.target, CacheBehaviourInfo, value.isResetOnPlay);
            }
        }

        [ContextMenu("StopPreview")]
        void StopPreview()
        {
            if (!SOEventPlayerSettingManager.Instance.activeDoTweenProPreview)
                return;

            if (CacheBehaviourInfo)
            {
                DOTweenPreviewManagerEx.StopPreview(CacheBehaviourInfo);
            }
            if (CacheBehaviourInfo)
                CacheBehaviourInfo.tween = null;
        }
        [ContextMenu("StopTargetPreview")]
        void StopTargetPreview()
        {
            if (!SOEventPlayerSettingManager.Instance.activeDoTweenProPreview)
                return;

            if (CacheTarget)
            {
                DOTweenPreviewManagerEx.StopPreview(CacheTarget);
            }
        }
        void StopPreview(GameObject target)
        {
            if (!SOEventPlayerSettingManager.Instance.activeDoTweenProPreview)
                return;

            DOTweenPreviewManagerEx.StopPreview(target);
        }

        //PS: 重载场景时需要调用
        //参考：DOTweenAnimationInspector.OnDisable
        [ContextMenu("StopAllPreviews")]
        public void StopAllPreviews()
        {
            if (!SOEventPlayerSettingManager.Instance.activeDoTweenProPreview)
                return;

            DOTweenPreviewManagerEx.StopAllPreviews();
        }

#endif
        #endregion
#endif

    }
}
