//Compate with Company Plugin
#if UNITY_2020_1_OR_NEWER
#define NewVersion
#else//旧版本直接激活
#define Active
#endif
#if NewVersion && USE_Timeline
#define Active
#endif

#if Threeyes_Timeline
#define Active
#endif
#if Active
using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Threeyes.EventPlayer
{
    public class MixerBehaviourBase<TBehaviour> : PlayableBehaviour where TBehaviour : class, IPlayableBehaviour, new()
    {
        //// NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
        //public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        //{
        //    int inputCount = playable.GetInputCount();

        //    for (int i = 0; i < inputCount; i++)
        //    {
        //        float inputWeight = playable.GetInputWeight(i);
        //        ScriptPlayable<TBehaviour> inputPlayable = (ScriptPlayable<TBehaviour>)playable.GetInput(i);
        //        TBehaviour input = inputPlayable.GetBehaviour();

        //        // Use the above variables to process each frame of this playable.
        //    }
        //}
    }
}
#endif
