#if Threeyes_DoTweenPro
using DG.Tweening;
using DG.Tweening.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LazyExtension_DoTween
{
    static DOTweenAnimation tweenAnimation;//Real
    static DOTweenPath tweenPath;
    static void TryConvert(this ABSAnimationComponent aBSAnimationComponent)
    {
        //tweenAnimation = null;
        tweenAnimation = aBSAnimationComponent as DOTweenAnimation;

        //tweenPath = null;
        tweenPath = aBSAnimationComponent as DOTweenPath;
    }

#region Tween

    public static bool Equal(this Tween tween1, Tween tween2)
    {
        if (tween1 == null || tween2 == null)
            return false;

        if (tween1.Duration() != tween2.Duration())
            return false;
        if (tween1.isBackwards != tween2.isBackwards)
            return false;

        return true;
    }

#endregion

#region Set


    public static Tween CreateEditorPreview(this ABSAnimationComponent aBSAnimationComponent)
    {
        aBSAnimationComponent.TryConvert();
        if (tweenAnimation)
        {
            return tweenAnimation.CreateEditorPreview();
        }
        if (tweenPath)
        {
            tweenPath.Invoke("Awake", 0);//Create Tween ( 参考 DoTweenPro source Code）
        }
        return null;
    }


    public static void ReCreateTween(this ABSAnimationComponent aBSAnimationComponent)
    {
        aBSAnimationComponent.TryConvert();
        if (tweenAnimation)
        {
            //tweenAnimation.DOKill();//Warning:Kills all tweens with the given ID or target and returns the number of actual
            if (tweenAnimation.tween != null)
                tweenAnimation.tween.Kill();
            tweenAnimation.tween = null;
            tweenAnimation.CreateTween();
        }
        //if (tweenPath)
        //{
        //    //参考DOTweenModuleUtils.CreateDOTweenPathTween()
        //    Comp.tween = motionTarget.transform.DOPath(tweenPath.path, tweenPath.duration, tweenPath.pathMode);
        //    tweenPath.Invoke("Awake", 0);//Create Tween ( 参考 DoTweenPro source Code）
        //}
    }

    public static void SetAutoPlay(this ABSAnimationComponent aBSAnimationComponent, bool isOn)
    {
        aBSAnimationComponent.TryConvert();
        if (tweenAnimation)
            tweenAnimation.autoPlay = isOn;
        if (tweenPath)
            tweenPath.autoPlay = isOn;
    }

    public static void SetAutoKill(this ABSAnimationComponent aBSAnimationComponent, bool isOn)
    {
        aBSAnimationComponent.TryConvert();
        if (tweenAnimation)
            tweenAnimation.autoKill = isOn;
        if (tweenPath)
            tweenPath.autoKill = isOn;
    }

    public static void SetTarget(this ABSAnimationComponent aBSAnimationComponent, GameObject target, bool isSelf, bool isTargetChanged)
    {
        aBSAnimationComponent.TryConvert();
        if (tweenAnimation)
        {
            tweenAnimation.targetIsSelf = isSelf;
            tweenAnimation.targetGO = target;

            //PS:DOTweenAnimation中类型为Component的target才是目标
            //Bug&&ToFix： 因为只有DOTweenAnimationInspector中才会更新target的值，所以只有选中的Clip才会看到更新
            //解决办法：
            //1.问作者等回复：https://github.com/Demigiant/dotween/issues/500
            //2.每次SetTarget后强制调用DOTweenAnimationInspector的调用方法
            //参考DOTweenAnimationInspector.Validate的650行， 需要手动获取Component
            //#ToReplace：并不能更新全部类型，只是临时使用，后期使用作者提供的方案
            bool isValid = Validate(tweenAnimation, target);//作用是更新target的值

//            //ToDelete(无效): 尝试调用DOTweenAnimationInspector的OnInspectorGUI()
//#if UNITY_EDITOR
//            if (!Application.isPlaying && !isValid && isTargetChanged)
//            {
//                tweenAnimation.target = null;
//                GameObject cacheObj = UnityEditor.Selection.activeGameObject;
//                UnityEditor.Selection.activeGameObject = tweenAnimation.gameObject;//强制更新

//                var cacheType = tweenAnimation.animationType;
//                tweenAnimation.animationType = DOTweenAnimation.AnimationType.None;
//                UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
//                UnityEditor.SceneView.RepaintAll();
//                UnityEditor.Selection.activeGameObject = null;//强制更新
//                tweenAnimation.animationType = cacheType;
//                UnityEditor.Selection.activeGameObject = tweenAnimation.gameObject;//强制更新
//                UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
//                UnityEditor.SceneView.RepaintAll();

//                //EditorTool.RepaintAllViews();
//                //EditorTool.RepaintAllViews();
//                //UnityEditor.Selection.activeGameObject = cacheObj;

//                Debug.Log("Target Changed");
//            }
//#endif
        }
        ////To Impl: 作为其子物体
        //if (tweenPath)
        //{
        //}
    }


    public static void SetDuration(this ABSAnimationComponent aBSAnimationComponent, float duration)
    {
        aBSAnimationComponent.TryConvert();
        if (tweenAnimation)
            tweenAnimation.duration = duration;
        if (tweenPath)
            tweenPath.duration = duration;
    }

    public static void SetLoops(this ABSAnimationComponent aBSAnimationComponent, int value)
    {
        aBSAnimationComponent.TryConvert();
        if (tweenAnimation)
            tweenAnimation.loops = value;
        if (tweenPath)
            tweenPath.loops = value;
    }

    public static void SetLoopType(this ABSAnimationComponent aBSAnimationComponent, LoopType value)
    {
        aBSAnimationComponent.TryConvert();
        if (tweenAnimation)
            tweenAnimation.loopType = value;
        if (tweenPath)
            tweenPath.loopType = value;
    }

#endregion

#region Get

    public static GameObject GetTarget(this ABSAnimationComponent aBSAnimationComponent)
    {
        aBSAnimationComponent.TryConvert();
        if (tweenAnimation)
            return tweenAnimation.targetGO;
        return null;
    }

    public static Tween GetTween(this ABSAnimationComponent aBSAnimationComponent)
    {
        if (!aBSAnimationComponent)
            return null;
        aBSAnimationComponent.TryConvert();
        return aBSAnimationComponent.tween;
    }

    public static int GetLoops(this ABSAnimationComponent aBSAnimationComponent)
    {
        aBSAnimationComponent.TryConvert();
        if (tweenAnimation)
            return tweenAnimation.loops;
        if (tweenPath)
            return tweenPath.loops;
        LogNullError();
        return 0;
    }

    public static LoopType GetLoopType(this ABSAnimationComponent aBSAnimationComponent)
    {
        aBSAnimationComponent.TryConvert();
        if (tweenAnimation)
            return tweenAnimation.loopType;
        if (tweenPath)
            return tweenPath.loopType;

        LogNullError();
        return default(LoopType);
    }

    public static float GetDuration(this ABSAnimationComponent aBSAnimationComponent)
    {
        aBSAnimationComponent.TryConvert();
        if (tweenAnimation)
            return tweenAnimation.duration;
        if (tweenPath)
            return tweenPath.duration;
        //Debug.LogError("Null!");
        return 0;
    }

    public static void Goto(this ABSAnimationComponent aBSAnimationComponent, float time)
    {
        aBSAnimationComponent.TryConvert();

        if (aBSAnimationComponent && aBSAnimationComponent.tween != null)
        {
            aBSAnimationComponent.tween.Goto(time);
        }
    }


    static void LogNullError()
    {
        Debug.LogError("Null ABSAnimationComponent !");
    }

#endregion


#region Common Define


#endregion


#region Copy From DoTweenAnimationInspector

    /// <summary>
    ///  if a Component that can be animated with the given animationType is attached to the src
    ///  PS: Ref from: 
    /// </summary>
    /// <param name="_src"></param>
    /// <param name="targetGO"></param>
    /// <returns></returns>
    // Checks
    static bool Validate(DOTweenAnimation _src, GameObject targetGO)
    {
        if (_src.animationType == DOTweenAnimation.AnimationType.None) return false;

        Component srcTarget;
        // First check for external plugins
#if false // TK2D_MARKER
            if (_Tk2dAnimationTypeToComponent.ContainsKey(_src.animationType)) {
                foreach (Type t in _Tk2dAnimationTypeToComponent[_src.animationType]) {
                    srcTarget = targetGO.GetComponent(t);
                    if (srcTarget != null) {
                        _src.target = srcTarget;
                        _src.targetType = DOTweenAnimation.TypeToDOTargetType(t);
                        return true;
                    }
                }
            }
#endif
#if false // TEXTMESHPRO_MARKER
            if (_TMPAnimationTypeToComponent.ContainsKey(_src.animationType)) {
                foreach (Type t in _TMPAnimationTypeToComponent[_src.animationType]) {
                    srcTarget = targetGO.GetComponent(t);
                    if (srcTarget != null) {
                        _src.target = srcTarget;
                        _src.targetType = DOTweenAnimation.TypeToDOTargetType(t);
                        return true;
                    }
                }
            }
#endif
        // Then check for regular stuff
        if (_AnimationTypeToComponent.ContainsKey(_src.animationType))
        {
            foreach (Type t in _AnimationTypeToComponent[_src.animationType])
            {
                srcTarget = targetGO.GetComponent(t);
                if (srcTarget != null)
                {
                    _src.target = srcTarget;
                    _src.targetType = DOTweenAnimation.TypeToDOTargetType(t);
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// PS:只是临时测试使用，因为在DoTweenUtilityPanel里面更新支持或者升级到新版后，可能会导致引用丢失，所以一律禁用可能不存在的
    /// </summary>
    static readonly Dictionary<DOTweenAnimation.AnimationType, Type[]> _AnimationTypeToComponent = new Dictionary<DOTweenAnimation.AnimationType, Type[]>() {
            {
                DOTweenAnimation.AnimationType.Move,
                new[]
                {
    //#if true // PHYSICS_MARKER
    //                typeof(Rigidbody),
    //#endif
    //#if true // PHYSICS2D_MARKER
    //                typeof(Rigidbody2D),
    //#endif
    //#if true // UI_MARKER
    //                typeof(RectTransform),
    //#endif
                    typeof(Transform)
                }},
                {
            DOTweenAnimation.AnimationType.Rotate,
            new[]
            {
#if true // PHYSICS_MARKER
                    typeof(Rigidbody),
#endif
#if true // PHYSICS2D_MARKER
                    typeof(Rigidbody2D),
#endif
                    typeof(Transform)
                }
            },
            { DOTweenAnimation.AnimationType.LocalMove, new[] { typeof(Transform) } },
            { DOTweenAnimation.AnimationType.LocalRotate, new[] { typeof(Transform) } },
            { DOTweenAnimation.AnimationType.Scale, new[] { typeof(Transform) } },
            { DOTweenAnimation.AnimationType.Color, new[] {
                typeof(Light),
//#if true // SPRITE_MARKER
//                typeof(SpriteRenderer),
//#endif
//#if true // UI_MARKER
//                typeof(Image), typeof(Text), typeof(RawImage), typeof(Graphic),
//#endif
                typeof(Renderer),
            }},
            { DOTweenAnimation.AnimationType.Fade, new[] {
                typeof(Light),
//#if true // SPRITE_MARKER
//                typeof(SpriteRenderer),
//#endif
//#if true // UI_MARKER
//                typeof(Image), typeof(Text), typeof(CanvasGroup), typeof(RawImage), typeof(Graphic),
//#endif
                typeof(Renderer),
            }},
//#if true // UI_MARKER
//            { DOTweenAnimation.AnimationType.Text, new[] { typeof(Text) } },
//#endif
            { DOTweenAnimation.AnimationType.PunchPosition, new[] {
//#if true // UI_MARKER
//                typeof(RectTransform),
//#endif
                typeof(Transform)
            }},
            { DOTweenAnimation.AnimationType.PunchRotation, new[] { typeof(Transform) } },
            { DOTweenAnimation.AnimationType.PunchScale, new[] { typeof(Transform) } },
            { DOTweenAnimation.AnimationType.ShakePosition, new[] {
//#if true // UI_MARKER
//                typeof(RectTransform),
//#endif
                typeof(Transform)
            }},
            { DOTweenAnimation.AnimationType.ShakeRotation, new[] { typeof(Transform) } },
            { DOTweenAnimation.AnimationType.ShakeScale, new[] { typeof(Transform) } },
            { DOTweenAnimation.AnimationType.CameraAspect, new[] { typeof(Camera) } },
            { DOTweenAnimation.AnimationType.CameraBackgroundColor, new[] { typeof(Camera) } },
            { DOTweenAnimation.AnimationType.CameraFieldOfView, new[] { typeof(Camera) } },
            { DOTweenAnimation.AnimationType.CameraOrthoSize, new[] { typeof(Camera) } },
            { DOTweenAnimation.AnimationType.CameraPixelRect, new[] { typeof(Camera) } },
            { DOTweenAnimation.AnimationType.CameraRect, new[] { typeof(Camera) } },
//#if true // UI_MARKER
//            { DOTweenAnimation.AnimationType.UIWidthHeight, new[] { typeof(RectTransform) } },
//#endif
        };



#endregion
}
#endif
