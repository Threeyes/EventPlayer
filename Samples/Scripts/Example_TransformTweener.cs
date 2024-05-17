using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Threeyes.EventPlayer.Example
{
    public class Example_TransformTweener : MonoBehaviour
    {
        public Transform target;

        public Transform tfStart;
        public Transform tfEnd;
        public AnimationCurve animTranslateCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        public AnimationCurve animRotateCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);


        public Transform tfThis
        {
            get
            {
                if (!_tfThis)
                    _tfThis = transform;
                return _tfThis;

            }
        }
        protected Transform _tfThis;


        public void TranslatePercent(float percent)
        {
            if (!tfStart || !tfEnd)
                return;

            if (!target)
                target = tfThis;
            float tweenProgress = animTranslateCurve.Evaluate(percent);
            target.position = Vector3.LerpUnclamped(tfStart.position, tfEnd.position, tweenProgress);
        }

        public void RotatePercent(float percent)
        {
            if (!tfStart || !tfEnd)
                return;

            if (!target)
                target = tfThis;

            float tweenProgress = animRotateCurve.Evaluate(percent);
            target.rotation = Quaternion.LerpUnclamped(tfStart.rotation, tfEnd.rotation, tweenProgress);
        }
    }
}