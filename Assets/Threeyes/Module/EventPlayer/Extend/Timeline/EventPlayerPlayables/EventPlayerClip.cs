#if true // Threeyes_Timeline
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Threeyes.EventPlayer
{
    [Serializable]
    public class EventPlayerClip : PlayableAsset, ITimelineClipAsset
    {
        /// <summary>
        /// This decide each loop's length, default is Infinity
        /// </summary>
        public override double duration
        {
            get
            {
                return clipLength > 0 ? clipLength : base.duration;
            }
        }
        public ClipCaps clipCaps
        {
            get { return ClipCaps.All; }
        }

        [SerializeField]
        [Tooltip("Per Clip Length, Only work when larger than 0")]
        protected double clipLength = -1;

        [SerializeField]
        [Tooltip("Flip the percent value")]
        protected bool isFlip = false;//Reverse the process

        public ExposedReference<EventPlayer> eventPlayer;

        [HideInInspector]
        public double cacheClipDuration;
        [HideInInspector]
        public EventPlayer cacheEPInst;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<EventPlayerBehaviour>.Create(graph);
            EventPlayerBehaviour clone = playable.GetBehaviour();
            clone.eventPlayer = eventPlayer.Resolve(graph.GetResolver());
            clone.clipLength = duration;
            clone.isFlip = isFlip;
            cacheEPInst = clone.eventPlayer;
            return playable;
        }
    }
}
#endif
