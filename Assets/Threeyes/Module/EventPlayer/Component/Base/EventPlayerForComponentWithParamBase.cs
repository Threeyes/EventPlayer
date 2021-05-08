using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Threeyes.EventPlayer
{
    /// <summary>
    /// Manage Sepcify Component
    /// </summary>
    /// <typeparam name="TComp"></typeparam>
    /// <typeparam name="TEP"></typeparam>
    /// <typeparam name="TUnityEvent"></typeparam>
    /// <typeparam name="TParam"></typeparam>
    public class EventPlayerForComponentWithParamBase<TComp, TEP, TUnityEvent, TParam> : EventPlayerWithParamBase<TEP, TUnityEvent, TParam>
        where TEP : EventPlayerWithParamBase<TEP, TUnityEvent, TParam>
        where TUnityEvent : UnityEvent<TParam>
        where TComp : Component
    {
        #region Property & Field

        public TComp Comp
        {
            get
            {
                if (Application.isPlaying && !comp)
                    comp = GetCompFunc();
                return comp;
            }
            set
            {
                comp = value;
            }
        }
        [SerializeField] protected TComp comp;//The relate Component

        #endregion

        #region Inner Method

        protected virtual TComp GetCompFunc()
        {
            if (this)//避免物体被销毁
                return this.GetComponent<TComp>();

            return null;
        }

        #endregion

        #region Editor Method
#if UNITY_EDITOR

        public override void SetInspectorGUISubProperty(GUIPropertyGroup group)
        {
            base.SetInspectorGUISubProperty(group);
            group.listProperty.Add(new GUIProperty("comp", "comp"));
        }
#endif
        #endregion
    }
}