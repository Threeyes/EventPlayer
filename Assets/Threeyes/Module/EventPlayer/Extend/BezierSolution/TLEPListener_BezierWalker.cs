using UnityEngine;
#if Threeyes_BezierSolution
using BezierSolution;
#endif
namespace Threeyes.EventPlayer
{
    public class TLEPListener_BezierWalker :
#if Threeyes_BezierSolution
        TLEPListenerForCompBase<BezierWalkerWithTime>
#else
        TLEPListenerForCompBaseDummy
#endif
    {

        #region Property & Field

        public GameObject goSplineOverride;//Override the Default BezierSpline

        #endregion

#if Threeyes_BezierSolution

        Transform cacheTarget;
        public override void OnPlayWithParam(PlayableInfo value)
        {
            if (value.clipState == ClipState.BehaviourPlay)
            {
                if (Comp)
                {
                    if (Comp.enabled)
                    {
                        Comp.enabled = false;//Stop Update
                    }
                    cacheTarget = GetTarget(value);

                    if (goSplineOverride)
                        Comp.spline = goSplineOverride.GetComponent<BezierSpline>();
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
            Evaluate(value);
        }

        public override void OnComplete(PlayableInfo value)
        {
            Evaluate(value);
        }

        #region Inner Method


        Transform GetTarget(PlayableInfo value)
        {
            return value.binding ? value.binding.transform : Comp.transform;
        }

        void Evaluate(PlayableInfo value)
        {
            Evaluate(value.subPercent, !value.isFlip);
        }

        void Evaluate(float normalizedT, bool isForward)
        {
            if (!Comp)
                return;
            if (!Comp.spline)
                return;
            if (!Comp.spline.gameObject.activeInHierarchy)//隐藏时不调用
                return;

            if (!cacheTarget)
                return;
            if (!cacheTarget.gameObject.activeInHierarchy)
                return;

            float _normalizedT = Comp.highQuality ? Comp.spline.evenlySpacedPoints.GetNormalizedTAtPercentage(normalizedT) : normalizedT;//Smoothing

            //Position
            cacheTarget.position = Comp.spline.GetPoint(_normalizedT);

            //Rotation
            if (Comp.lookAt == LookAtMode.Forward)
            {
                BezierSpline.Segment segment = Comp.spline.GetSegmentAt(_normalizedT);
                Quaternion targetRotation;
                if (isForward)
                    targetRotation = Quaternion.LookRotation(segment.GetTangent(), segment.GetNormal());
                else
                    targetRotation = Quaternion.LookRotation(-segment.GetTangent(), segment.GetNormal());

                cacheTarget.rotation = targetRotation;
            }
            else if (Comp.lookAt == LookAtMode.SplineExtraData)//Wait for extraDataLerpAsQuaternionFunction to expost
                cacheTarget.rotation = Comp.spline.GetExtraData(_normalizedT, extraDataLerpAsQuaternionFunction);
        }

        protected static readonly ExtraDataLerpFunction extraDataLerpAsQuaternionFunction = InterpolateExtraDataAsQuaternion;
        private static BezierPoint.ExtraData InterpolateExtraDataAsQuaternion(BezierPoint.ExtraData data1, BezierPoint.ExtraData data2, float normalizedT)
        {
            return Quaternion.LerpUnclamped(data1, data2, normalizedT);
        }

        #endregion

#else
        const string tipsInfo = "You need to open the Setting Window and active BezierSolution support!";
        [Header(tipsInfo)]
        public string tips = tipsInfo;
#endif
    }
}
