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
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Threeyes.Timeline
{
    public abstract class TrackBase<TTrack, TBehaviour, TMixBehaviour, TClip, TBinding> : TrackAsset
        where TMixBehaviour : class, IPlayableBehaviour, new()
        where TBehaviour : BehaviourBase<TBinding>, new()
        where TClip : ClipBase<TTrack, TBehaviour, TBinding>
    where TTrack : TrackAsset
    where TBinding : Object
    {

        /// <summary>
        /// Get Call when you change the content of Track:
        /// 1.Drag/Create/Delete Clip
        /// 2.Change binding object
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="go"></param>
        /// <param name="inputCount"></param>
        /// <returns></returns>
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            InitTrack(graph, go, inputCount);
            return ScriptPlayable<TMixBehaviour>.Create(graph, inputCount);
        }

        /// <summary>
        /// 初始化Track
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="go"></param>
        /// <param name="inputCount"></param>
        protected virtual void InitTrack(PlayableGraph graph, GameObject go, int inputCount)
        {
            Object binding = go.GetComponent<PlayableDirector>().GetGenericBinding(this);

            foreach (TimelineClip timelineClip in GetClips())
            {
                var clip = timelineClip.asset as TClip;
                if (clip != null)
                {
                    clip.binding = binding;
                    InitClip(timelineClip, clip, graph, go, inputCount);
                }
            }
        }

        /// <summary>
        /// 初始化单个Clip
        /// </summary>
        /// <param name="timelineClip"></param>
        /// <param name="clip"></param>
        /// <param name="graph"></param>
        /// <param name="go"></param>
        /// <param name="inputCount"></param>
        protected virtual void InitClip(TimelineClip timelineClip, TClip clip, PlayableGraph graph, GameObject go, int inputCount)
        {
        }

        ///// <summary>
        ///// 创建一个Clip时调用
        ///// </summary>
        ///// <param name="clip"></param>
        //protected override void OnCreateClip(TimelineClip clip)
        //{
        //    base.OnCreateClip(clip);
        //}
    }
}
#endif
