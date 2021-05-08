#if true // Threeyes_Timeline
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Threeyes.EventPlayer
{
    public class EventPlayerBehaviour : PlayableBehaviour
    {
        public EventPlayer eventPlayer;

        public double clipLength = -1;
        public bool isFlip = false;//Reverse the process

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (eventPlayer)
                eventPlayer.Play();

            listFramePlayed.Clear();
        }

        //PS: ProcessFrame and OnBehaviourPlay execute at the same frame
        public List<int> listFramePlayed = new List<int>();
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (!eventPlayer)
                return;

            //If reach next loop start, inovke Play method
            if (clipLength > 0 && info.evaluationType == FrameData.EvaluationType.Playback)
            {
                int loopIndex = 0;
//#if UNITY_EDITOR
//                //Editor Mode (May cause frame event lost due to Timeline frame missing ((https://forum.unity.com/threads/animation-events-on-last-frame-arent-fired-in-timeline-when-its-the-last-frame-of-the-timeline.791258/))
//                if (Application.isEditor && !Application.isPlaying)
//                {
//                    if (clipLength > 0)
//                    {
//                        double time = playable.GetTime();
//                        double leftTime = time;
//                        //Count the loop
//                        while (leftTime > 0)
//                        {
//                            leftTime -= clipLength;
//                            loopIndex++;
//                        }
//                        //if not the end of clip && not the first time && close to next loop start time
//                        if (loopIndex * clipLength < playable.GetDuration() && (loopIndex != 0) && (0.019 + leftTime) > 0)//
//                        {
//                            eventPlayer.Play();
//                        }
//                    }
//                }
//                else
//#endif
                {
                    //RunTime
                    loopIndex = Mathf.CeilToInt((float)(playable.GetTime() / clipLength));
                    if (loopIndex > 0 && !listFramePlayed.Contains(loopIndex))
                    {
                        eventPlayer.Play();
                        listFramePlayed.Add(loopIndex);
                    }
                }
            }
            UpdateTimeLineInfo(playable, info);
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            //Prevent get call on Tineline Start https://forum.unity.com/threads/release-event-player-visual-play-and-organize-unityevent.536984/#post-3605916
            double time = playable.GetGraph().GetRootPlayable(0).GetTime();
            if (time > 0)
            {
                UpdateTimeLineInfo(playable, info);

                //Stop after UpdateTimeLineInfo
                if (eventPlayer)
                    eventPlayer.Stop();
            }
            listFramePlayed.Clear();
        }

        PlayableInfo cachePlayableInfo = new PlayableInfo();
        void UpdateTimeLineInfo(Playable playable, FrameData info)
        {
            ITimelineProgress iEventPlayerTimeLineInfo = eventPlayer as ITimelineProgress;

            if (iEventPlayerTimeLineInfo != null)
            {
                cachePlayableInfo.UpdateData(playable, info);
                cachePlayableInfo.isFlip = isFlip;
                iEventPlayerTimeLineInfo.OnClipUpdate(cachePlayableInfo);
            }
        }
    }
}
#endif
