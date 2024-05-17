#if Threeyes_DoTweenPro
#if UNITY_EDITOR

namespace Threeyes.EventPlayer
{
    using System;
    using System.Collections.Generic;
    using DG.Tweening;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;
    using System.Linq;
    using BehaviourInfo = TLEPListener_DoTween.BehaviourInfo;

    /// <summary>
    /// 功能：在Editor模式下预览Tween，并在程序运行前重置
    /// Copy from DOTweenPreviewManager
    /// </summary>
    public static class DOTweenPreviewManagerEx
    {
        //static readonly Dictionary<DOTweenAnimation, TweenInfo> _AnimationToTween = new Dictionary<DOTweenAnimation, TweenInfo>();
        //static readonly List<DOTweenAnimation> _TmpKeys = new List<DOTweenAnimation>();

        static readonly Dictionary<BehaviourInfo, TweenInfo> _AnimationToTween = new Dictionary<BehaviourInfo, TweenInfo>();
        static readonly List<BehaviourInfo> _TmpKeys = new List<BehaviourInfo>();//筛选后临时保存的list

        static bool isDeBugLog = false;


        #region Methods

        public static Tween StartPreviewForTarget(GameObject target, BehaviourInfo anim, bool isKillOnStop)
        {
            return StartPreview(target, isKillOnStop, new BehaviourInfo[] { anim }).FirstOrDefault();
        }

        public static List<Tween> StartPreview(GameObject target, bool isKillOnStop, params BehaviourInfo[] anims)
        {
            //Stop Previous preview
            foreach (BehaviourInfo anim in anims)
                StopPreview(anim);

            bool isPreviewing = _AnimationToTween.Count > 0;
            if (!isPreviewing) StartupGlobalPreview();

            //DOTweenAnimation[] anims = go.GetComponents<DOTweenAnimation>();
            List<Tween> listTween = new List<Tween>();
            foreach (BehaviourInfo anim in anims)
            {
                listTween.Add(AddAnimationToGlobalPreview(anim, isKillOnStop, target));//PS:不管是否为null都返回，便于index一一对应
            }
            return listTween;
        }

        public static void StopPreview(GameObject target)
        {
            _TmpKeys.Clear();
            foreach (KeyValuePair<BehaviourInfo, TweenInfo> kvp in _AnimationToTween)
            {
                if (kvp.Key == null) continue;

                if (kvp.Value.target == target)
                    _TmpKeys.Add(kvp.Key);
            }
            StopPreview(_TmpKeys);
            _TmpKeys.Clear();

            if (_AnimationToTween.Count == 0)
                StopAllPreviews();
            else
                InternalEditorUtility.RepaintAllViews();
        }
        public static void StopPreview(BehaviourInfo behaviourInfo)
        {
            _TmpKeys.Clear();
            foreach (KeyValuePair<BehaviourInfo, TweenInfo> kvp in _AnimationToTween)
            {
                if (kvp.Key == null) continue;

                if (kvp.Key == behaviourInfo)
                    _TmpKeys.Add(kvp.Key);
            }
            StopPreview(_TmpKeys);
            _TmpKeys.Clear();

            if (_AnimationToTween.Count == 0)
                StopAllPreviews();
            else
                InternalEditorUtility.RepaintAllViews();
        }

        static Tween AddAnimationToGlobalPreview(BehaviourInfo behaviourInfo, bool isKillOnStop, GameObject target = null)
        {
            DOTweenAnimation src = behaviourInfo.doTweenAnimation;
            if (!src.isActive)
                return null; // Ignore sources whose tweens have been set to inactive

            if (!_AnimationToTween.ContainsKey(behaviourInfo))//ToFix:是这里导致同一个DOTweenAnimation无法多次加入 ，改为传入PlayableInfo作为唯一标识
            {
                Tween t = src.CreateEditorPreview();
                _AnimationToTween.Add(behaviourInfo, new TweenInfo(src, t, src.isFrom, isKillOnStop, target));
                // Tween setup
                //DOTweenEditorPreviewEx.PrepareTweenForPreview(t, andPlay: false);
                return t;
            }
            return null;
        }

        static void StartupGlobalPreview()
        {
            //DOTweenEditorPreviewEx.Start();
            UnityEditor.EditorApplication.playModeStateChanged += StopAllPreviews;
        }

        public static void StopAllPreviews(PlayModeStateChange state)
        {
            //在编辑完成、进入/退出场景会后被调用
            StopAllPreviews();
        }

        public static void StopAllPreviews()
        {
            _TmpKeys.Clear();
            foreach (KeyValuePair<BehaviourInfo, TweenInfo> kvp in _AnimationToTween)
            {
                _TmpKeys.Add(kvp.Key);
            }

            if (isDeBugLog && _TmpKeys.Count() > 0)
                Debug.Log("StopAllPreviews " + _TmpKeys.Count());

            StopPreview(_TmpKeys);
            _TmpKeys.Clear();
            _AnimationToTween.Clear();

            //DOTweenEditorPreviewEx.Stop();
            UnityEditor.EditorApplication.playModeStateChanged -= StopAllPreviews;
            InternalEditorUtility.RepaintAllViews();
        }

        // Stops while iterating inversely, which deals better with tweens that overwrite each other
        static void StopPreview(List<BehaviourInfo> keys)
        {
            for (int i = keys.Count - 1; i > -1; --i)
            {
                BehaviourInfo anim = keys[i];
                try
                {
                    TweenInfo tInfo = _AnimationToTween[anim];
                    if (tInfo != null && tInfo.tween != null)
                    {
                        //if (tInfo.isKillOnStop)
                        {
                            //备注：不主动决定其是否重置，因为有的Tween是反向运动
                            if (tInfo.isFrom)
                            {
                                int totLoops = tInfo.tween.Loops();
                                if (totLoops < 0 || totLoops > 1)
                                {
                                    tInfo.tween.Goto(tInfo.tween.Duration(false));
                                }
                                else tInfo.tween.Complete();
                            }
                            else
                                tInfo.tween.Rewind();
                        }

                        tInfo.tween.Kill();
                    }

                    if (anim.doTweenAnimation != null)
                        EditorUtility.SetDirty(anim.doTweenAnimation); // Refresh views
                }
                catch
                {

                }
                _AnimationToTween.Remove(anim);
            }
        }

        //static void StopPreview(Tween t)
        //{
        //    TweenInfo tInfo = null;
        //    foreach (KeyValuePair<BehaviourInfo, TweenInfo> kvp in _AnimationToTween)
        //    {
        //        if (kvp.Value.tween != t) continue;
        //        tInfo = kvp.Value;
        //        _AnimationToTween.Remove(kvp.Key);
        //        break;
        //    }
        //    if (tInfo == null)
        //    {
        //        Debug.LogWarning("DOTween Preview ► Couldn't find tween to stop");
        //        return;
        //    }
        //    if (tInfo.isFrom)
        //    {
        //        int totLoops = tInfo.tween.Loops();
        //        if (totLoops < 0 || totLoops > 1)
        //        {
        //            tInfo.tween.Goto(tInfo.tween.Duration(false));
        //        }
        //        else
        //            tInfo.tween.Complete();
        //    }
        //    else
        //        tInfo.tween.Rewind();
        //    tInfo.tween.Kill();
        //    EditorUtility.SetDirty(tInfo.animation); // Refresh views

        //    if (_AnimationToTween.Count == 0) StopAllPreviews();
        //    else InternalEditorUtility.RepaintAllViews();
        //}

        #endregion

        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███ INTERNAL CLASSES ████████████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

        class TweenInfo
        {
            public DOTweenAnimation animation;
            public Tween tween;
            public bool isFrom;
            public bool isKillOnStop = true;
            public GameObject target;
            public TweenInfo(DOTweenAnimation animation, Tween tween, bool isFrom, bool isKillOnStop, GameObject target = null)
            {
                this.animation = animation;
                this.tween = tween;
                this.isFrom = isFrom;
                this.isKillOnStop = isKillOnStop;
                this.target = target;
            }
        }

    }

    //public static class DOTweenEditorPreviewEx
    //{
    //    public static bool isPreviewing { get; private set; }

    //    static double _previewTime;
    //    static Action _onPreviewUpdated;
    //    static readonly List<Tween> _Tweens = new List<Tween>();

    //    #region Public Methods

    //    /// <summary>
    //    /// Starts the update loop of tween in the editor. Has no effect during playMode.
    //    /// </summary>
    //    /// <param name="onPreviewUpdated">Eventual callback to call after every update</param>
    //    public static void Start(Action onPreviewUpdated = null)
    //    {
    //        if (isPreviewing || EditorApplication.isPlayingOrWillChangePlaymode) return;

    //        isPreviewing = true;
    //        _onPreviewUpdated = onPreviewUpdated;
    //        _previewTime = EditorApplication.timeSinceStartup;
    //        //EditorApplication.update += PreviewUpdate;
    //    }

    //    /// <summary>
    //    /// Stops the update loop and clears the onPreviewUpdated callback.
    //    /// </summary>
    //    /// <param name="resetTweenTargets">If TRUE also resets the tweened objects to their original state.
    //    /// Note that this works by calling Rewind on all tweens, so it will work correct ly
    //    /// only if you have a single tween type per object and it wasn't killed</param>
    //    /// <param name="clearTweens">If TRUE also kills any cached tween</param>
    //    public static void Stop(bool resetTweenTargets = false, bool clearTweens = true)
    //    {
    //        isPreviewing = false;
    //        //EditorApplication.update -= PreviewUpdate;
    //        _onPreviewUpdated = null;
    //        if (resetTweenTargets)
    //        {
    //            foreach (Tween t in _Tweens)
    //            {
    //                try
    //                {
    //                    //备注：不建议使用From
    //                    //if (t.isFrom)
    //                    //    t.Complete();
    //                    //else
    //                    t.Rewind();
    //                }
    //                catch
    //                {
    //                    // Ignore
    //                }
    //            }
    //        }
    //        if (clearTweens) _Tweens.Clear();
    //        else ValidateTweens();
    //    }

    //    /// <summary>
    //    /// Readies the tween for editor preview by setting its UpdateType to Manual plus eventual extra settings.
    //    /// </summary>
    //    /// <param name="t">The tween to ready</param>
    //    /// <param name="clearCallbacks">If TRUE (recommended) removes all callbacks (OnComplete/Rewind/etc)</param>
    //    /// <param name="preventAutoKill">If TRUE prevents the tween from being auto-killed at completion</param>
    //    /// <param name="andPlay">If TRUE starts playing the tween immediately</param>
    //    public static void PrepareTweenForPreview(Tween t, bool clearCallbacks = true, bool preventAutoKill = true, bool andPlay = true)
    //    {
    //        _Tweens.Add(t);
    //        t.SetUpdate(UpdateType.Manual);
    //        if (preventAutoKill) t.SetAutoKill(false);
    //        if (clearCallbacks)
    //        {
    //            t.OnComplete(null)
    //                .OnStart(null).OnPlay(null).OnPause(null).OnUpdate(null).OnWaypointChange(null)
    //                .OnStepComplete(null).OnRewind(null).OnKill(null);
    //        }
    //        if (andPlay) t.Play();
    //    }

    //    #endregion

    //    #region Methods

    //    static void ValidateTweens()
    //    {
    //        for (int i = _Tweens.Count - 1; i > -1; --i)
    //        {
    //            if (_Tweens[i] == null || !_Tweens[i].active) _Tweens.RemoveAt(i);
    //        }
    //    }

    //    #endregion
    //}

}
#endif
#endif
