#if true // Threeyes_Timeline
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Threeyes.EventPlayer
{
    [TrackColor(0f, 1f, 0f)]
    [TrackClipType(typeof(EventPlayerClip))]
    public class EventPlayerTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            //检查引用是否为空
            foreach (TimelineClip clip in GetClips())
            {
                EventPlayerClip eventPlayerClip = clip.asset as EventPlayerClip;
                EventPlayer eventPlayer = eventPlayerClip.eventPlayer.Resolve(graph.GetResolver());
                if (eventPlayer)
                {
                    clip.displayName = eventPlayer.name;
                }
                else
                {
                    //检查是否无引用
                    string suffix = " (Null)";
                    if (!clip.displayName.Contains(suffix))
                        clip.displayName = clip.displayName + suffix;
                }
            }
            return ScriptPlayable<EventPlayerMixerBehaviour>.Create(graph, inputCount);
        }


#if UNITY_EDITOR && (UNITY_2018_2 || UNITY_2018_3 || UNITY_2018_4)
        /// <summary>
        ///功能：如果用户调节Clip的duration，则主动更新
        /// #ToUpdate!
        ///Warning: 2019及以上不支持！需要研究新方法
        /// </summary>
        protected override void UpdateDuration()
        {
            foreach (TimelineClip clip in GetClips())
            {
                EventPlayerClip eventPlayerClip = clip.asset as EventPlayerClip;
                ITimelineProgress epTLI = eventPlayerClip.cacheEPInst as ITimelineProgress;
                if (epTLI != null)
                {
                    //更新Hierarchy的属性显示(如Duration）
                    double curClipDuration = clip.duration;
                    if (eventPlayerClip.cacheClipDuration != curClipDuration)
                    {
                        eventPlayerClip.cacheClipDuration = curClipDuration;
                    }
                }
            }
            base.UpdateDuration();
        }
#endif
    }
}
#endif
