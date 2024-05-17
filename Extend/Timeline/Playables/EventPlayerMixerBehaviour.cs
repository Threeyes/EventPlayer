#if Threeyes_Timeline
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Linq;
using Threeyes.Core;

namespace Threeyes.EventPlayer
{
    /// <summary>
    /// PS: These event will execute berore ClipBehaviour
    /// </summary>
    public class EventPlayerMixerBehaviour : MixerBehaviourBase<EventPlayerBehaviour>
    {

        #region Override Method

        public override void OnPlayableCreate(Playable playable)
        {
            UpdateBehaviourData(ClipState.PlayableCreate, playable);
        }
        public override void OnGraphStart(Playable playable)
        {
            UpdateBehaviourData(ClipState.GraphStart, playable);
        }
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            UpdateBehaviourData(ClipState.BehaviourPlay, playable, info);
        }
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            UpdateBehaviourData(ClipState.ProcessFrame, playable, info);
        }
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            UpdateBehaviourData(ClipState.BehaviourPause, playable, info);
        }
        public override void OnGraphStop(Playable playable)
        {
            UpdateBehaviourData(ClipState.GraphStop, playable);
        }
        public override void OnPlayableDestroy(Playable playable)
        {
            UpdateBehaviourData(ClipState.PlayableDestroy, playable);
        }

        #endregion 

        #region Inner Method

        int cacheInputCount;
        PlayableInfo cachePlayableInfo = new PlayableInfo();
        ScriptPlayable<EventPlayerBehaviour> cacheInputPlayable;
        List<EventPlayerBehaviour> listCacheActiveBehaviour = new List<EventPlayerBehaviour>();

        /// <summary>
        /// ʵʱ��������Clip��Weight��EventPlayerBehaviour�е�FrameData.weightֻ����ִ��ʱ���£����ܵ�����©��
        /// </summary>
        /// <param name="playable"></param>
        void UpdateBehaviourData(ClipState clipState, Playable playable, FrameData? info = null)
        {
            try
            {
                if (playable.Equals(Playable.Null))
                    return;

                List<EventPlayerBehaviour> listTempActiveBehaviour = new List<EventPlayerBehaviour>();

                cacheInputCount = playable.GetInputCount();
                for (int index = 0; index != cacheInputCount; index++)
                {
                    cacheInputPlayable = (ScriptPlayable<EventPlayerBehaviour>)playable.GetInput(index);
                    EventPlayerBehaviour behaviour = cacheInputPlayable.GetBehaviour();
                    float weight = playable.GetInputWeight(index);
                    behaviour.UpdateMixData(index, weight);

#if UNITY_EDITOR
                    //���浱ǰWeight>0��Behaviour
                    if (!Application.isPlaying)
                        if (weight > 0)
                            listTempActiveBehaviour.Add(behaviour);
#endif
                }

#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    if (clipState == ClipState.ProcessFrame)
                    {
                        DetectUserJump(playable, listTempActiveBehaviour);
                        listCacheActiveBehaviour = listTempActiveBehaviour;
                        cachePlayableInfo.UpdateData(playable, info);
                    }
                }
#endif
            }
            catch
            {
            }
        }

        double lastPlayTime = -1;
        /// <summary>
        /// �����û���ת���ض�֡
        /// </summary>
        /// <param name="playable"></param>
        /// <param name="listCurActiveBehaviour"></param>
        private void DetectUserJump(Playable playable, List<EventPlayerBehaviour> listCurActiveBehaviour)
        {
            PlayableGraph playableGraph = playable.GetGraph();
            Playable rootPlayable = playableGraph.GetRootPlayable(0);
            double curPlayTime = rootPlayable.GetTime();

            if (lastPlayTime == -1)
            {
                lastPlayTime = curPlayTime;
                return;
            }

            //���⵽��ת���������зǵ�ǰClip
            PlayableDirector playableDirector = playableGraph.GetResolver() as PlayableDirector;
            if (playableDirector)
            {
                if (curPlayTime != 0 && Mathd.Abs(curPlayTime - lastPlayTime) > 0.5f)
                {
                    //PS:��ΪTL�����򲥷ţ���ʹ���ص�����List����������1��ֻ��Ҫ�����ұߵ�Clip�Ƿ���ͬ����
                    if (listCurActiveBehaviour.Count != 0 && listCacheActiveBehaviour.Count != 0)
                    {
                        EventPlayerBehaviour curLast = listCurActiveBehaviour.Last();
                        EventPlayerBehaviour preLast = listCacheActiveBehaviour.Last();
                        if (curLast != preLast)
                        {
                            //�� �Ӻ���ǰ Reset������Clip
                            int curClipIndex = -1;
                            for (int index = playable.GetInputCount() - 1; index != -1; index--)
                            {
                                cacheInputPlayable = (ScriptPlayable<EventPlayerBehaviour>)playable.GetInput(index);
                                EventPlayerBehaviour behaviour = cacheInputPlayable.GetBehaviour();
                                if (behaviour != curLast)
                                {
                                    behaviour.ResetTimelineInfo();
                                    //Debug.Log("Reset: " + index);
                                }
                                else
                                {
                                    curClipIndex = index;
                                    break;
                                }
                            }
                            // ��ǰ���� Completeǰ����Clip������������ת������һ��Complete��
                            for (int index = 0; index != curClipIndex; index++)
                            {

                                cacheInputPlayable = (ScriptPlayable<EventPlayerBehaviour>)playable.GetInput(index);
                                EventPlayerBehaviour behaviour = cacheInputPlayable.GetBehaviour();
                                if (behaviour != curLast)
                                {
                                    behaviour.CompleteTimelineInfo();
                                    //Debug.Log("Complete: " + index);
                                }
                            }
                        }
                    }
                }
            }

            lastPlayTime = curPlayTime;
        }
        #endregion
    }
}
#endif
