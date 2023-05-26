using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Threeyes.EventPlayer
{
    /// <summary>
    /// Work with Component that don't need to pass extra param
    /// </summary>
    public class EventPlayerForCompBase<TComp> : EventPlayer
        where TComp : Component
    {
        #region Property & Field

        public TComp comp;//待绑定的组件
        public TComp Comp
        {
            get
            {
                if (!comp)
                    comp = GetCompFunc();
                return comp;
            }
            set
            {
                comp = value;
            }
        }

        #endregion

        #region Inner Method

        protected virtual TComp GetCompFunc()
        {
            if (this)//避免物体被销毁
                return this.GetComponent<TComp>();

            return null;
        }

        #endregion
    }
}