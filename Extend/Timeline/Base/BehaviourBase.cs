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
namespace Threeyes.EventPlayer
{
    [System.Serializable]
    public abstract class BehaviourBase<TBinding> : PlayableBehaviour
    {
        public TBinding trackBinding;

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            //Prevent get call on Timeline Start https://forum.unity.com/threads/release-event-player-visual-play-and-organize-unityevent.536984/#post-3605916
            if (playable.GetRootTime() > 0)
                OnBehaviourPauseFunc(playable, info);
            else
            {
                OnBehaviourResetFunc(playable, info);
            }
        }

        public virtual void OnBehaviourPauseFunc(Playable playable, FrameData info)
        {

        }

        public virtual void OnBehaviourResetFunc(Playable playable, FrameData info)
        {

        }

    }

    //PS:放到这里方便重用宏定义
    public static class LazyExtension_Timeline
    {
        public static float GetPercent(this Playable playable)
        {
            return (float)(playable.GetTime() / playable.GetDuration());
        }

        /// <summary>
        /// 返回顶层的时间
        /// </summary>
        /// <param name="playable"></param>
        /// <returns></returns>
        public static double GetRootTime(this Playable playable)
        {
            return playable.GetGraph().GetRootPlayable(0).GetTime();
        }
        public static double GetRootDuration(this Playable playable)
        {
            return playable.GetGraph().GetRootPlayable(0).GetDuration();
        }

        public static PlayableDirector GetDirector(this Playable playable)
        {
            //https://forum.unity.com/threads/accessing-playable-director-from-a-track-to-get-the-current-time.503319/
            return playable.GetGraph().GetResolver() as PlayableDirector;
        }

        public static T TryResolve<T>(this ExposedReference<T> target, IExposedPropertyTable resolver) where T : Object
        {
            T result = default(T);
            //if (target != null)
            result = target.Resolve(resolver);
            return result;
        }
    }
}
#endif
