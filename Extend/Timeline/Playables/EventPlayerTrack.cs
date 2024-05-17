#if Threeyes_Timeline
using Threeyes.Core;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Threeyes.EventPlayer
{
    [TrackColor(0f, 1f, 0f)]//(0f, 0.8f, 0.8f)
    [TrackClipType(typeof(EventPlayerClip))]
    [TrackBindingType(typeof(GameObject))]
    public class EventPlayerTrack : TrackBase<EventPlayerTrack, EventPlayerBehaviour, EventPlayerMixerBehaviour, EventPlayerClip, GameObject>
    {
        //Update：不通过更改名字提示有无EP，便于用户设置Clip 名称
        protected override void InitClip(TimelineClip timelineClip, EventPlayerClip clip, PlayableGraph graph, GameObject go, int inputCount)
        {
            var resolver = graph.GetResolver();
            EventPlayer eventPlayer = clip.eventPlayer.Resolve(resolver);
            string suffixNull = " (Null)";//空引用名称标记

#if UNITY_2019_2_OR_NEWER
            if (eventPlayer)
            {
                timelineClip.displayName = eventPlayer.name;
            }
            else
            {
                timelineClip.displayName = timelineClip.displayName.Remove(suffixNull);//Unity2019.2：删掉名称标记，改为TrackColor标记  
            }
#else
            //https://forum.unity.com/threads/whats-the-mean-of-exposedreference-t.483864/：In the case of Timeline, all (non - prefab) GameObjects and Component references are stored inside the PlayableDirector that is playing the Timeline.The resolver is actually the PlayableDirector, and Resolve() is asking it to retrieve the assigned game object.
            //PS：在这里，resolver跟go都是PlayerDirector组件所在物体的对应组件
            //PS:不管是新旧版都需要与EP同名，避免引用丢失导致无法获取原意图
            //PS:Unity2018会有替换EP后不更新的问题
            if (eventPlayer)
            {
                timelineClip.displayName = eventPlayer.name;
            }
            else
            {
                //检查是否无引用
                if (!timelineClip.displayName.Contains(suffixNull))
                    timelineClip.displayName = timelineClip.displayName + suffixNull;
            }
#endif
        }

    }
}
#endif
