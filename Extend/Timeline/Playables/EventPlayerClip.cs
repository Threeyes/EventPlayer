#if Threeyes_Timeline
using System;
using Threeyes.Core;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Threeyes.EventPlayer
{
    /// <summary>
    /// ToAdd:��������Ƶ�������������ã���BezierWalkerWithTime
    /// </summary>
    [Serializable]
    public class EventPlayerClip : ClipBase<EventPlayerTrack, EventPlayerBehaviour, GameObject>, ITimelineClipAsset
    {
        /// <summary>
        /// Specify each loop's length, default is Infinity
        /// </summary>
        public override double duration { get { return clipLength > 0 ? clipLength : base.duration; } }
        public override ClipCaps clipCaps { get { return ClipCaps.All; } }


        public ExposedReference<EventPlayer> eventPlayer;

        [Header("Clip Setting")]
        [SerializeField] protected ClipLoopType loopType = ClipLoopType.Restart;
        [Tooltip("Per Clip Length, Only work when larger than 0 and less than duration")]
        [SerializeField] protected double clipLength = -1;
        [Tooltip("Flip the percent value")]
        [SerializeField] protected bool isFlip = false;//Reverse the percent
        [Tooltip("Remap the percent value")]
        [SerializeField] protected AnimationCurve curvePercent = AnimationCurve.Linear(0, 0, 1, 1);//percent remapping

        [EnumMask]
        public ClipState resetOn = ClipState.None;//When to call Reset

        [Header("Editor Setting")]
        [Tooltip("Reset when you jump to any clip before this")]
        public bool isResetOnJump = true;
        [Tooltip("Complete when you jump to any clip after this")]
        public bool isCompleteOnJump = true;
        [Tooltip("Reset when Editor Play")]
        public bool isResetOnPlay = false;//(Set to true if this Clip is the first one in track)

        public override void InitClone(EventPlayerBehaviour clone, PlayableGraph graph, GameObject owner)
        {
            clone.eventPlayer = eventPlayer.Resolve(graph.GetResolver());
            clone.loopType = loopType;
            clone.clipLength = clipLength;
            clone.isFlip = isFlip;
            clone.curvePercent = curvePercent;
            clone.resetOn = resetOn;
            clone.isResetOnJump = isResetOnJump;
            clone.isCompleteOnJump = isCompleteOnJump;
            clone.isResetOnPlay = isResetOnPlay;
        }
    }
}
#endif
