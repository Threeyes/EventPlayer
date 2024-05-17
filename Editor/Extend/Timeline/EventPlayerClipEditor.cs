#if Threeyes_Timeline
#if UNITY_EDITOR
#if UNITY_2019_2_OR_NEWER//Unity2019.2及以上才有此接口
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEditor.Timeline;
using UnityEngine.Playables;

namespace Threeyes.EventPlayer.Editor
{
    /// <summary>
    /// Warning: Require Unity.Timeline.Editor reference
    /// </summary>
    [CustomTimelineEditor(typeof(EventPlayerClip))]
    public class EventPlayerClipEditor : ClipEditor
    {
        Dictionary<Object, bool> dicClipEPRef = new Dictionary<Object, bool>();//缓存EventPlayerClip的EP赋值状态
        public override ClipDrawOptions GetClipOptions(TimelineClip clip)
        {
            var clipOptions = base.GetClipOptions(clip);
            if (dicClipEPRef.ContainsKey(clip.asset))
                clipOptions.highlightColor = dicClipEPRef[clip.asset] ? Color.green * 0.8f : Color.red * 0.8f;//根据是否有引用，更新Clip的颜色
            return clipOptions;
        }

        public override void GetSubTimelines(TimelineClip clip, PlayableDirector director, List<PlayableDirector> subTimelines)
        {
            EventPlayerClip eventPlayerClip = clip.asset as EventPlayerClip;
            EventPlayer eventPlayer = eventPlayerClip.eventPlayer.Resolve(director);
            dicClipEPRef[clip.asset] = eventPlayer != null;
            base.GetSubTimelines(clip, director, subTimelines);
        }
    }
}
#endif
#endif
#endif
