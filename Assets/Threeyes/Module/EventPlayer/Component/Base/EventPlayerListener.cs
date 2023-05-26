using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Threeyes.EventPlayer
{
    /// <summary>
    /// For custom coding
    /// </summary>
    public class EventPlayerListener : MonoBehaviour, IEventPlayerHandler
    {
        public virtual void OnActiveDeactive(bool isActive)
        {
        }

        public virtual void OnPlay()
        {
        }

        public virtual void OnPlayStop(bool isPlay)
        {
        }

        public virtual void OnStop()
        {
        }
    }

    public interface IEventPlayerHandler
    {
        void OnPlayStop(bool isPlay);
        void OnPlay();
        void OnStop();
        void OnActiveDeactive(bool isActive);
    }



}