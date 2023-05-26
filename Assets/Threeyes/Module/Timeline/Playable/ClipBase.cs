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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
namespace Threeyes.Timeline
{
    [System.Serializable]
    public class ClipBase<TTrack, TBehaviour, TBinding> : PlayableAsset, ITimelineClipAsset
where TBehaviour : BehaviourBase<TBinding>, new()
where TBinding : Object
    {
        public TTrack track { get; set; }
        public Object binding { get; set; }


        //如果不想太多Track，可以用bindingOverride
        public ExposedReference<TBinding> bindingOverride;//PS:Nullable

        public virtual ClipCaps clipCaps
        {
            get { return ClipCaps.All; }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            ScriptPlayable<TBehaviour> playable = ScriptPlayable<TBehaviour>.Create(graph);
            TBehaviour clone = playable.GetBehaviour();

            TBinding bindingOverrideObj = bindingOverride.Resolve(graph.GetResolver());
            clone.trackBinding = bindingOverrideObj ? bindingOverrideObj : binding as TBinding;
            InitClone(clone, graph, owner);
            return playable;
        }


        public virtual void InitClone(TBehaviour clone, PlayableGraph graph, GameObject owner)
        {

        }
    }
}
#endif
