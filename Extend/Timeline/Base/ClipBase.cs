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
namespace Threeyes.EventPlayer
{
    [System.Serializable]
    public class ClipBase<TTrack, TBehaviour> : PlayableAsset, ITimelineClipAsset
where TBehaviour : BehaviourBase, new()
    {
        public TTrack track { get; set; }

        public virtual ClipCaps clipCaps
        {
            get { return ClipCaps.All; }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            ScriptPlayable<TBehaviour> playable = ScriptPlayable<TBehaviour>.Create(graph);
            TBehaviour clone = playable.GetBehaviour();

            InitClone(clone, graph, owner);
            return playable;
        }

        public virtual void InitClone(TBehaviour clone, PlayableGraph graph, GameObject owner)
        {
        }
    }

    public class ClipBase<TTrack, TBehaviour, TBinding> : ClipBase<TTrack, TBehaviour>
        where TBehaviour : BehaviourBase<TBinding>, new()
        where TBinding : Object
    {
        public Object binding { get; set; }

        public override void InitClone(TBehaviour clone, PlayableGraph graph, GameObject owner)
        {
            clone.trackBinding = binding as TBinding;
        }
    }

    [System.Serializable]
    public class ClipBaseWithOverride<TTrack, TBehaviour, TBinding> : ClipBase<TTrack, TBehaviour, TBinding>
        where TBehaviour : BehaviourBase<TBinding>, new()
        where TBinding : Object
    {

        //如果不想太多Track，可以用bindingOverride（Bug：在创建Clip时，Editor只针对第一个ExposedReference自动获取，所以改为由Clip自己定义该字段而不是使用共用的）
        public ExposedReference<TBinding> bindingOverride;//PS:Nullable

        public override void InitClone(TBehaviour clone, PlayableGraph graph, GameObject owner)
        {
            TBinding bindingOverrideObj = bindingOverride.Resolve(graph.GetResolver());
            clone.trackBinding = bindingOverrideObj ? bindingOverrideObj : binding as TBinding;
        }
    }
}
#endif
