using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if true // Threeyes_Timeline
using UnityEngine.Playables;
#endif

/// <summary>
/// 处理Timeline中Clip的事件
/// </summary>
public interface ITimelineProgress
{
    void OnClipUpdate(PlayableInfo playableInfo);
}

public enum ClipPlayState
{
    Play = 1,
    Update = 2,
    Pause = 3,
}

[System.Serializable]
public class PlayableInfo
{
#if true // Threeyes_Timeline
    public Playable playable;
    public FrameData info;
#endif
    public bool isFlip;

    public double time;//Current Clip Time
    public double duration;//Total Clip Time
    public float deltaTime;
    public float percent
    {
        get
        {
            if (duration > 0)
            {
                float tempValue = Mathf.Clamp01((float)(time / duration));
                if (isFlip)
                    tempValue = 1 - tempValue;
                return tempValue;
            }
            else
            {
                return 0;
            }
        }
    }

#if true // Threeyes_Timeline
    public void UpdateData(Playable playable, FrameData info)
    {
        this.playable = playable;
        this.info = info;

        //PS:该数据可能会被销毁，因此需要及时保存其值
        time = playable.GetTime();
        deltaTime = info.deltaTime;
        duration = playable.GetDuration();
    }
#endif
}


[System.Serializable]
public class PlayableInfoEvent : UnityEvent<PlayableInfo>
{
}
