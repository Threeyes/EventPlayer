using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Threeyes.EventPlayer
{
    public class EventPlayerWithParamListener<TParam> : EventPlayerListener, IEventPlayerWithParamHandler<TParam>
    {

        public virtual void OnPlayWithParam(TParam value)
        {
        }

        public virtual void OnStopWithParam(TParam value)
        {
        }

    }

    public interface IEventPlayerWithParamHandler<TParam>
    {
        void OnPlayWithParam(TParam value);
        void OnStopWithParam(TParam value);
    }

}